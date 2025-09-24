using FlamingOrange.Combat.Damage;
using FlamingOrange.CoreSystem;
using FlamingOrange.Utilities;
using UnityEngine;

namespace FlamingOrange.Enemies.StateMachine
{
    public class LongAttackState : State
    {
        private readonly EnemyLongRange _enemyLongRange;
        
        private Movement Movement { get => _movement ?? core.GetCoreComponent(ref _movement); }
        private Movement _movement;
        
        public LongAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyLongRange enemyLongRange) : base(entity, stateMachine, animBoolName)
        {
            _enemyLongRange = enemyLongRange;
        }
        
        public override void DoChecks()
        {
            base.DoChecks();
            
            _enemyLongRange.AllRangeCheck();
            
            if (_enemyLongRange.IsInPreferAttackDistance)
            {
                AdjustPosition();
            }
        }

        public override void Enter()
        {
            base.Enter();

            _enemyLongRange.AttackCooldown.OnTimerDone += Attack;
            
            if (!_enemyLongRange.AttackCooldown.IsActive)
            {
                Attack();
            }
        }

        public override void Exit()
        {
            base.Exit();
            _enemyLongRange.AttackCooldown.OnTimerDone -= Attack;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            _enemyLongRange.AttackCooldown.Tick();
            
            if (!_enemyLongRange.IsTargetInAttackRange)
            {
                stateMachine.ChangeState(_enemyLongRange.LongChaseState);
            }
        }

        private void Attack()
        {
            _enemyLongRange.ShootAtTarget();
            
            _enemyLongRange.Anim.SetTrigger("AttackTrigger");

            _enemyLongRange.AttackCooldown.StartTimer();
        }

        private void AdjustPosition()
        {
            var target = Physics2D.OverlapCircle(
                _enemyLongRange.transform.position, 
                _enemyLongRange.Data.distanceToMaintain, 
                _enemyLongRange.AttackLayer
            );
            
            Vector2 awayDir = (_enemyLongRange.transform.position - target.transform.position).normalized;
            
            Movement.SetVelocity(awayDir * _enemyLongRange.Data.retreatVelocity);
        }

    }
}