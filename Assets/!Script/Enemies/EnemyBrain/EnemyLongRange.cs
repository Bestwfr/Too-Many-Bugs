using FlamingOrange.Enemies.StateMachine;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public class EnemyLongRange : Enemy<LongRangeEnemyData>
    {
        public ChaseState<EnemyLongRange, LongRangeEnemyData> ChaseState { get; private set; }
        public override State AttackState { get; protected set; }
        
        public bool IsInPreferAttackDistance { get; private set; }

        public override void Awake()
        {
            base.Awake();
            ChaseState = new ChaseState<EnemyLongRange, LongRangeEnemyData> (this, StateMachine, "Chase", this);
            AttackState = new LongAttackState(this, StateMachine, "Attack", this);
        }

        protected override void Start()
        {
            base.Start();
            StateMachine.Initialize(ChaseState);
        }

        public override void AllRangeCheck()
        {
            base.AllRangeCheck();
            IsInPreferAttackDistance = CheckPreferDistance();
        }

        private bool CheckPreferDistance()
        {
            return Physics2D.OverlapCircle(transform.position, Data.distanceToMaintain, AttackLayer);
        }

        public void ShootAtTarget()
        {
            Vector2 direction = Target.transform.position - transform.position;
            GameObject projectile = Object.Instantiate(Data.projectilePrefab, transform.position, Quaternion.identity);

            if (projectile.TryGetComponent(out EnemyProjectile proj))
            {
                proj.Damage = Data.attackDamage;
                proj.Lifetime = Data.projectileLifeTime;
                proj.Source = gameObject;
            }

            if (projectile.TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = direction.normalized * Data.projectileSpeed;
            }
        }
    }
}