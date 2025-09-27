using FlamingOrange.Combat.Damage;
using FlamingOrange.Combat.KnockBack;
using FlamingOrange.CoreSystem;
using FlamingOrange.Utilities;
using UnityEngine;

namespace FlamingOrange.Enemies.StateMachine
{
    public class CloseAttackState : State
    {
        private readonly EnemyCloseRange _enemyCloseRange;
        
        public CloseAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyCloseRange enemyCloseRange) : base(entity, stateMachine, animBoolName)
        {
            _enemyCloseRange = enemyCloseRange;
        }
        
        public override void DoChecks()
        {
            base.DoChecks();
            
            _enemyCloseRange.AllRangeCheck();
        }

        public override void Enter()
        {
            base.Enter();

            _enemyCloseRange.AttackCooldown.OnTimerDone += Attack;
            
            if (!_enemyCloseRange.AttackCooldown.IsActive)
            {
                Attack();
            }
        }

        public override void Exit()
        {
            base.Exit();
            _enemyCloseRange.AttackCooldown.OnTimerDone -= Attack;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            _enemyCloseRange.AttackCooldown.Tick();
            
            if (!_enemyCloseRange.IsTargetInAttackRange)
                stateMachine.ChangeState(_enemyCloseRange.ChaseState);
        }

        private void Attack()
        {
            var direction = _enemyCloseRange.Target.value.transform.position - core.Root.transform.position;
            
            var damageable = _enemyCloseRange.Target.value.GetComponent<IDamageable>();
            damageable?.Damage(new DamageData(_enemyCloseRange.Data.AttackDamage, core.Root));
            
            var knockBackable = _enemyCloseRange.Target.value.GetComponent<IKnockBackable>();
            knockBackable?.KnockBack(new KnockBackData(direction.normalized, _enemyCloseRange.Data.KnockBack, core.Root));
            
            _enemyCloseRange.Anim.SetTrigger("AttackTrigger");

            _enemyCloseRange.AttackCooldown.StartTimer();
        }
    }
}