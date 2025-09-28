using FlamingOrange.Combat.Damage;
using FlamingOrange.Combat.KnockBack;
using PurrNet;

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
            _enemyCloseRange.Attack();
            
            _enemyCloseRange.Anim.SetTrigger("AttackTrigger");

            _enemyCloseRange.AttackCooldown.StartTimer();
        }
    }
}