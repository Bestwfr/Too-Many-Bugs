using FlamingOrange.Enemies.StateMachine;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public class Enemy : Entity
    {
        [field: SerializeField] public EnemyData Data { get; private set; }
        
        public ChaseState ChaseState { get; private set;}
        public AttackState AttackState { get; private set; }
        
        public GameObject Target { get; private set; }
        public GameObject BaseObject { get; private set; }

        public override void Awake()
        {
            base.Awake();
            
            BaseObject = GameObject.FindGameObjectWithTag("Base");
            
            ChaseState = new ChaseState(this, StateMachine, "Chase", this);
            AttackState = new AttackState(this, StateMachine, "Attack", this);
        }

        private void Start()
        {
            Target = BaseObject;
            StateMachine.Initialize(ChaseState);
        }

        public bool CheckPlayerInAggroRange()
        {
            var player = Physics2D.OverlapCircle(transform.position, Data.playerAggroDistance, Data.whatIsPlayer);
            if (player != null)
            {
                Target = player.gameObject;
            }
            return player;
        }

        public bool CheckPlayerOutAggroRange()
        {
            var player = Physics2D.OverlapCircle(transform.position, Data.playerDeaggroDistance, Data.whatIsPlayer);
            if (player == null)
            {
                Target = BaseObject;
            }
            return player;
        }

        public bool CheckInAttackRange()
        {
            var attackLayer = Data.whatIsPlayer | Data.whatIsBase;
            return Physics2D.OverlapCircle(transform.position, Data.attackDistance, attackLayer);
        }
    }
}