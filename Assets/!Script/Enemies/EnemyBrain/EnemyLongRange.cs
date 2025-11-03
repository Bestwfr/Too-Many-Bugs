using System.Collections;
using System.Collections.Generic;
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
        
        public override void OnReusedFromPool()
        {
            if (!isServer) return;

            StateMachine.Initialize(ChaseState);
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
        
        public void ShootAtTarget()
        {
            if (Target.value == null) return;
            var direction = Target.value.transform.position - transform.position;
            var projectile = Instantiate(Data.ProjectilePrefab.GetComponent<EnemyProjectile>(), transform.position, Quaternion.identity);
            
            Debug.Log("Shoot");

            projectile.Initialize(
                Data.AttackDamage, 
                Data.ProjectileLifeTime, 
                Data.ProjectileKnockBack, 
                gameObject, 
                direction.normalized, 
                Data.ProjectileSpeed);
        }
    }
}