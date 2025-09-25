using FlamingOrange.Combat.Damage;
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

            BaseObject = GameObject.FindGameObjectWithTag("Base");
            AttackCooldown = new Timer(Data.attackFrequencySecond);
        }

        protected override void Interrupt(float damageTaken)
        {
            base.Interrupt(damageTaken * Data.interruptMultiplier);
        }

        protected virtual void Start()
        {
            AttackLayer = Data.whatIsPlayer | Data.whatIsBase;
            Target = BaseObject;
        }
        
        protected bool CheckPlayerInAggroRange()
        {
            var player = Physics2D.OverlapCircle(transform.position, Data.playerAggroDistance, Data.whatIsPlayer);
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
            var player = Physics2D.OverlapCircle(transform.position, Data.playerDeaggroDistance, Data.whatIsPlayer);
            if (player == null)
                Target = BaseObject;

            return player;
        }

        protected bool CheckInAttackRange()
        {
            var layerMask = 1 << Target.layer;
            return Physics2D.OverlapCircle(transform.position, Data.attackDistance, layerMask) != null;
        }
        
        public virtual void AllRangeCheck()
        {
            IsPlayerInAggroRange = CheckPlayerInAggroRange();
            IsPlayerOutAggroRange = CheckPlayerOutAggroRange();
            IsTargetInAttackRange = CheckInAttackRange();
        }
    }
}
