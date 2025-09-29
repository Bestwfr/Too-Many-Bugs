using FlamingOrange.Combat.Damage;
using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies.StateMachine;
using FlamingOrange.Utilities;
using PurrNet;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public abstract class Enemy<TData> : Entity where TData : EnemyData
    {
        [field: SerializeField] public TData Data { get; private set; }

        public abstract State AttackState { get; protected set; }

        public SyncVar<GameObject> Target = new(ownerAuth: true);
        public GameObject BaseObject { get; protected set; }
        public LayerMask AttackLayer { get; protected set; }

        public bool IsPlayerInAggroRange { get; protected set; }
        public bool IsPlayerOutAggroRange { get; protected set; }
        public bool IsTargetInAttackRange { get; protected set; }

        public override void Awake()
        {
            base.Awake();
            AttackCooldown = new Timer(Data.AttackFrequencySecond);
        }

        private void Start()
        {
            if (!BaseObject)
                BaseObject = GameObject.FindGameObjectWithTag("Base");
            
            AttackLayer = Data.WhatIsPlayer | Data.WhatIsBase;
            Target.value = BaseObject;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();
            if (!isServer) return;
            Core.GetCoreComponent<Stats>().InitializeHealth(Data.Health);
        }

        protected override void Interrupt(float damageTaken)
        {
            if (!isServer) return;
            base.Interrupt(damageTaken * Data.InterruptMultiplier);
        }
        
        public virtual void AllRangeCheck()
        {
            if (!isServer) return;

            TargetValidation();
            
            IsPlayerInAggroRange = CheckPlayerInAggroRange();
            IsPlayerOutAggroRange = CheckPlayerOutAggroRange();
            IsTargetInAttackRange = CheckInAttackRange();
        }
        
        protected void TargetValidation()
        {
            if (Target == null || Target.value == null || Target.value == BaseObject)
                return;
            
            if (((1 << Target.value.layer) & Data.WhatIsPlayer) != 0)
            {
                float distance = Vector2.Distance(transform.position, Target.value.transform.position);
                
                if (distance > Data.AttackDistance)
                {
                    FindNewTarget();
                }
            }
        }

        private void FindNewTarget()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, Data.AttackDistance, Data.WhatIsPlayer);

            float closestDistance = float.MaxValue;
            GameObject closestPlayer = null;

            foreach (var hit in hits)
            {
                if (hit == null) continue;

                var candidate = hit.gameObject;
                var damageable = candidate.GetComponent<IDamageable>();
                if (damageable == null) continue;

                float distance = Vector2.Distance(transform.position, candidate.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = candidate;
                }
            }

            if (closestPlayer != null)
            {
                Target.value = closestPlayer;
            }
        }
        
        private bool CheckPlayerInAggroRange()
        {
            if (!isServer) return false;

            var player = Physics2D.OverlapCircle(transform.position, Data.PlayerAggroDistance, Data.WhatIsPlayer);
            if (player == null) return false;

            if (player.gameObject.GetComponent<IDamageable>() != null)
            {
                Target.value = player.gameObject;
                return true;
            }

            return false;
        }

        private bool CheckPlayerOutAggroRange()
        {
            if (!isServer) return false;

            var player = Physics2D.OverlapCircle(transform.position, Data.PlayerDeaggroDistance, Data.WhatIsPlayer);
            if (!player) Target.value = BaseObject;
            
            return player;
        }

        private bool CheckInAttackRange()
        {
            if (!isServer && !Target.value) return false;
            
            var layerMask = 1 << Target.value.layer;
            return Physics2D.OverlapCircle(transform.position, Data.AttackDistance, layerMask) != null;
        }
    }
}
