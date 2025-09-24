using FlamingOrange.Combat.Damage;
using FlamingOrange.Enemies.StateMachine;
using FlamingOrange.Utilities;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public class EnemyCloseRange : Entity
    {
        [field: SerializeField] public CloseRangeEnemyData Data { get; private set; }
        
        public CloseChaseState CloseChaseState { get; private set;}
        public CloseAttackState CloseAttackState { get; private set; }
        
        [field: SerializeField] public GameObject Target { get; private set; }
        public GameObject BaseObject { get; private set; }
        
        public bool IsPlayerInAggroRange { get; private set; }
        public bool IsPlayerOutAggroRange { get; private set; }
        public bool IsTargetInAttackRange { get; private set; }
        
        public LayerMask AttackLayer { get; private set; }

        public override void Awake()
        {
            base.Awake();
            
            BaseObject = GameObject.FindGameObjectWithTag("Base");
            
            AttackCooldown = new Timer(Data.attackFrequencySecond);
            
            CloseChaseState = new CloseChaseState(this, StateMachine, "Chase", this);
            CloseAttackState = new CloseAttackState(this, StateMachine, "Attack", this);
        }

        private void Start()
        {
            AttackLayer = Data.whatIsPlayer | Data.whatIsBase;
            
            Target = BaseObject;
            StateMachine.Initialize(CloseChaseState);
        }
        
        protected override void Interrupt(float damageTaken)
        {
            base.Interrupt(damageTaken * Data.interruptMultiplier);
        }

        public void AllRangeCheck()
        {
            IsPlayerInAggroRange = CheckPlayerInAggroRange();
            IsPlayerOutAggroRange = CheckPlayerOutAggroRange();
            IsTargetInAttackRange = CheckInAttackRange();
        }

        private bool CheckPlayerInAggroRange()
        {
            var player = Physics2D.OverlapCircle(transform.position, Data.playerAggroDistance, Data.whatIsPlayer);

            if (player == null) return false;
            
            var hurtBox = player.gameObject.GetComponent<IDamageable>();
            var isHurtBox = false;
            
            if (hurtBox != null)
            {
                isHurtBox = true;
                Target = player.gameObject;
            }
            return isHurtBox;
        }

        private bool CheckPlayerOutAggroRange()
        {
            var player = Physics2D.OverlapCircle(transform.position, Data.playerDeaggroDistance, Data.whatIsPlayer);
            if (player == null)
            {
                Target = BaseObject;
            }
            return player;
        }

        private bool CheckInAttackRange()
        {
            var layerMask = 1 << Target.layer;
            return Physics2D.OverlapCircle(transform.position, Data.attackDistance, layerMask) != null;
        }
    }
}