using System;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class Movement : CoreComponent
    {
        public Animator Anim { get; private set;}
        public Rigidbody2D Rb { get; private set; }
        public int FacingX { get; private set; }
        public Vector2 LastFacing { get; private set; }
        public bool CanSetVelocity { get; set; }
        public Vector2 CurrentVelocity {  get; private set; }
        
        private Vector2 _workspace;

        protected override void Awake()
        {
            base.Awake();
            
            Rb = GetComponentInParent<Rigidbody2D>();
            Anim = GetComponentInParent<Animator>();
        }

        private void Start()
        {
            FacingX = 1;
            LastFacing = Vector2.right;
            CanSetVelocity = true;
        }

        public override void PhysicsUpdate()
        {
            CurrentVelocity = Rb.linearVelocity;
        }
        
        public void SetVelocity(Vector2 velocity)
        {
            if (!DetermineAuthority()) return;
            
            _workspace.Set(velocity.x, velocity.y);
            
            if (!CanSetVelocity) return;
            
            Rb.linearVelocity = _workspace;
            CurrentVelocity = _workspace;
        }
        
        public void MoveTowards(Vector2 target, float speed)
        {
            if (!DetermineAuthority()) return;
            
            Vector2 newPos = Vector2.MoveTowards(Rb.position, target, speed * Time.fixedDeltaTime);
            Rb.MovePosition(newPos);
        }

        public void AddForce(Vector2 force)
        {
            if (!DetermineAuthority()) return;
            
            Rb.AddForce(force);
        }
        
        
        public void UpdateFacing(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.1f * 0.1f)
                return;
            
            LastFacing =  direction;
            
            Anim.SetFloat("Y", direction.y);

            FlipX(direction);
        }
        
        public Vector2 GetFacingDirection()
        {
            if (LastFacing.sqrMagnitude < 0.0001f)
                return FacingX >= 0 ? Vector2.right : Vector2.left;

            return LastFacing.normalized;
        }
        
        public int GetFacingX(Vector2 direction)
        {
            if (direction.x > 0.01f) return 1;
            if (direction.x < -0.01f) return -1;
            return FacingX;
        }
        private void FlipX(Vector2 direction)
        {
            int signX = GetFacingX(direction);
            if (signX != FacingX)
            {
                FacingX = signX;

                var s = transform.parent.parent.localScale;
                s.x = Mathf.Abs(s.x) * FacingX;
                transform.parent.parent.localScale = s;
            }
        }

    }
}