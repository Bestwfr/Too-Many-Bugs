using System;
using FlamingOrange.CoreSystem;
using NaughtyAttributes;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    public class ActionHitBox : ToolComponent<ActionHitboxData, AttackActionHitBox>
    {
        public event Action<Collider2D[]> OnDetectedCollider2D;
        
        [SerializeField] private bool debugShowRuntimeHitbox = true;
        [SerializeField] private float debugRuntimeDuration = 0.2f;

        private Vector2 _pivotUser;
        private float _angleDegree;
        private Vector2 _hitBoxCenterLocal;
        private Vector2 _hitBoxCenterWorld;
        
        private Vector2 _dbgSize;
        private float _dbgShowUntil;

        
        private CoreComp<CoreSystem.Movement> _movement;
        
        private Collider2D[] _detected;
        private void HandleAttackAction()
        {
            Vector2 facing = _movement.Comp.GetFacingDirection().normalized;
            
            _angleDegree  = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
            _pivotUser = Core.Root.transform.position;
            _hitBoxCenterLocal = currentAttackData.HitBox.center;
            
            _hitBoxCenterWorld = _pivotUser + (Vector2)(Quaternion.Euler(0,0,_angleDegree) * _hitBoxCenterLocal);
            
            _detected = Physics2D.OverlapBoxAll(
                _hitBoxCenterWorld,
                currentAttackData.HitBox.size,
                _angleDegree,
                data.DetectableLayer
            );
            
            if (debugShowRuntimeHitbox)
            {
                _dbgSize   = currentAttackData.HitBox.size;
                _dbgShowUntil = Time.time + debugRuntimeDuration;
            }

            
            if (_detected.Length == 0) return;
            
            OnDetectedCollider2D?.Invoke(_detected);
        }

        protected override void Start()
        {
            base.Start();

            _movement = new CoreComp<CoreSystem.Movement>(Core);
            
            eventHandler.OnAttackAction += HandleAttackAction;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            eventHandler.OnAttackAction -= HandleAttackAction;
        }

        private void OnDrawGizmosSelected()
        {
            if (data?.AttackData == null) return;

            foreach (var item in data.AttackData)
            {
                if (!item.Debug) continue;

                Gizmos.DrawWireCube(
                    transform.position + (Vector3)item.HitBox.center,
                    item.HitBox.size
                );
            }
        }
        private void OnDrawGizmos()
        {
            if (!debugShowRuntimeHitbox || !Application.isPlaying) return;
            if (Time.time > _dbgShowUntil) return;

            Gizmos.color = Color.cyan;
            Matrix4x4 prev = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(_hitBoxCenterWorld, Quaternion.Euler(0, 0, _angleDegree), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _dbgSize);
            Gizmos.matrix = prev;
        }
    }
}