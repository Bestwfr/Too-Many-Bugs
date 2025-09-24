using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies;

namespace FlamingOrange.Enemies.StateMachine
{
    public class CloseChaseState : State
    {
        private readonly EnemyCloseRange _enemyCloseRange;
        
        private Movement Movement { get => _movement ?? core.GetCoreComponent(ref _movement); }
        private Movement _movement;
        
        public CloseChaseState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyCloseRange enemyCloseRange) : base(entity, stateMachine, animBoolName)
        {
            _enemyCloseRange = enemyCloseRange;
        }

        public override void DoChecks()
        {
            base.DoChecks();

            _enemyCloseRange.AllRangeCheck();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            var target = _enemyCloseRange.Target.transform;

            Movement.MoveTowards(target.position, _enemyCloseRange.Data.movementVelocity);

            if (_enemyCloseRange.IsTargetInAttackRange)
            {
                stateMachine.ChangeState(_enemyCloseRange.CloseAttackState);
            }
        }
        
    }
}