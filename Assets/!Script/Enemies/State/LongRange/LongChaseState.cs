using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies;

namespace FlamingOrange.Enemies.StateMachine
{
    public class LongChaseState : State
    {
        private readonly EnemyLongRange _enemyLongRange;
        
        private Movement Movement { get => _movement ?? core.GetCoreComponent(ref _movement); }
        private Movement _movement;
        
        public LongChaseState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyLongRange enemyLongRange) : base(entity, stateMachine, animBoolName)
        {
            _enemyLongRange = enemyLongRange;
        }

        public override void DoChecks()
        {
            base.DoChecks();
            _enemyLongRange.AllRangeCheck();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            var target = _enemyLongRange.Target.transform;

            Movement.MoveTowards(target.position, _enemyLongRange.Data.movementVelocity);

            if (_enemyLongRange.IsTargetInAttackRange)
            {
                stateMachine.ChangeState(_enemyLongRange.LongAttackState);
            }
        }
        
    }
}