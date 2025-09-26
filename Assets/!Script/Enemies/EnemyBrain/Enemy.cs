using FlamingOrange.Combat.Damage;
using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies.StateMachine;
using FlamingOrange.Utilities;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public abstract class Enemy<TData> : Entity where TData : EnemyData
    {
        [field: SerializeField] public TData Data { get; private set; }
        
        public abstract State AttackState { get; protected set; }
        
        public GameObject Target { get; protected set; }
        public GameObject BaseObject { get; protected set; }
        public LayerMask AttackLayer { get; protected set; }

        public bool IsPlayerInAggroRange { get; protected set; }
        public bool IsPlayerOutAggroRange { get; protected set; }
        public bool IsTargetInAttackRange { get; protected set; }

        public override void Awake()
        {
            base.Awake();
            Core.GetCoreComponent<Stats>().InitializeHealth(Data.Health);

            BaseObject = GameObject.FindGameObjectWithTag("Base");
            AttackCooldown = new Timer(Data.AttackFrequencySecond);
        }

        protected override void Interrupt(float damageTaken)
        {
            base.Interrupt(damageTaken * Data.InterruptMultiplier);
        }

        protected virtual void Start()
        {
            AttackLayer = Data.WhatIsPlayer | Data.WhatIsBase;
            Target = BaseObject;
        }
        
        protected bool CheckPlayerInAggroRange()
        {
            var player = Physics2D.OverlapCircle(transform.position, Data.PlayerAggroDistance, Data.WhatIsPlayer);
            if (player == null) return false;

            var hurtBox = player.gameObject.GetComponent<IDamageable>();
            if (hurtBox != null)
            {
                Target = player.gameObject;
                return true;
            }
            return false;
        }

        protected bool CheckPlayerOutAggroRange()
        {
            var player = Physics2D.OverlapCircle(transform.position, Data.PlayerDeaggroDistance, Data.WhatIsPlayer);
            if (player == null)
                Target = BaseObject;

            return player;
        }

        protected bool CheckInAttackRange()
        {
            var layerMask = 1 << Target.layer;
            return Physics2D.OverlapCircle(transform.position, Data.AttackDistance, layerMask) != null;
        }
        
        public virtual void AllRangeCheck()
        {
            IsPlayerInAggroRange = CheckPlayerInAggroRange();
            IsPlayerOutAggroRange = CheckPlayerOutAggroRange();
            IsTargetInAttackRange = CheckInAttackRange();
        }
    }
}
