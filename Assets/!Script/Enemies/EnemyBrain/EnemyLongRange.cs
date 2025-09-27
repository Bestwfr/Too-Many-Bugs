using FlamingOrange.Enemies.StateMachine;
using PurrNet;
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

        protected override void OnSpawned()
        {
            base.OnSpawned();
            if (isServer) StateMachine.Initialize(ChaseState);
        }

        public override void AllRangeCheck()
        {
            base.AllRangeCheck();
            IsInPreferAttackDistance = CheckPreferDistance();
        }

        private bool CheckPreferDistance()
        {
            return Physics2D.OverlapCircle(transform.position, Data.DistanceToMaintain, AttackLayer);
        }

        [ObserversRpc]
        public void ShootAtTarget()
        {
            var direction = Target.value.transform.position - transform.position;
            var projectile = GameObject.Instantiate(Data.ProjectilePrefab, transform.position, Quaternion.identity);

            if (projectile.TryGetComponent(out EnemyProjectile proj))
            {
                proj.Damage = Data.AttackDamage;
                proj.Lifetime = Data.ProjectileLifeTime;
                proj.Knockback = Data.ProjectileKnockBack;
                proj.Source = gameObject;
            }

            if (projectile.TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = direction.normalized * Data.ProjectileSpeed;
            }
        }
    }
}