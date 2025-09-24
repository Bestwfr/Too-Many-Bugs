using System;
using FlamingOrange.Combat.Damage;
using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies.StateMachine;
using FlamingOrange.Utilities;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public class EnemyLongRange : Entity
    {
        [field: SerializeField] public LongRangeEnemyData Data { get; private set; }
        
        public LongChaseState LongChaseState { get; private set;}
        public LongAttackState LongAttackState { get; private set; }
        
        public GameObject Target { get; private set; }
        public GameObject BaseObject { get; private set; }
        
        public bool IsPlayerInAggroRange { get; private set; }
        public bool IsPlayerOutAggroRange { get; private set; }
        public bool IsTargetInAttackRange { get; private set; }
        public bool IsInPreferAttackDistance { get; private set; }
        
        public LayerMask AttackLayer { get; private set; }
        

        public override void Awake()
        {
            base.Awake();
            
            BaseObject = GameObject.FindGameObjectWithTag("Base"); ;
            
            AttackCooldown = new Timer(Data.attackFrequencySecond);

            LongChaseState = new LongChaseState(this, StateMachine, "Chase", this);
            LongAttackState = new LongAttackState(this, StateMachine, "Attack", this);
        }
        
        private void Start()
        {
            AttackLayer = Data.whatIsPlayer | Data.whatIsBase;
            
            Target = BaseObject;
            StateMachine.Initialize(LongChaseState);
        }

        protected override void Interrupt(float damageTaken)
        {
            base.Interrupt(damageTaken * Data.interruptMultiplier);
        }
        
        public void ShootAtTarget()
        {
            Vector2 direction = Target.transform.position - transform.position;
            
            GameObject projectile = Instantiate(Data.projectilePrefab, transform.position, Quaternion.identity);
            EnemyProjectile proj = projectile.GetComponent<EnemyProjectile>();
            
            proj.Damage = Data.attackDamage;
            proj.Lifetime = Data.projectileLifeTime;
            proj.Source = gameObject;
            
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction.normalized * Data.projectileSpeed;
            }
        }

        public void AllRangeCheck()
        {
            IsPlayerInAggroRange = CheckPlayerInAggroRange();
            IsPlayerOutAggroRange = CheckPlayerOutAggroRange();
            IsTargetInAttackRange = CheckInAttackRange();
            IsInPreferAttackDistance = CheckPreferDistance();
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

        private bool CheckPreferDistance()
        {
            return Physics2D.OverlapCircle(transform.position, Data.distanceToMaintain, AttackLayer);
        }
    }
}