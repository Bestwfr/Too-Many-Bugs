using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies;

namespace FlamingOrange.Enemies.StateMachine
{
    public class ChaseState<TEnemy, TData> : State
        where TEnemy : Enemy<TData>
        where TData : EnemyData
    {
        private readonly TEnemy _enemy;

        private Movement Movement { get => _movement ?? core.GetCoreComponent(ref _movement); }
        private Movement _movement;

        public ChaseState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, TEnemy enemy)
            : base(entity, stateMachine, animBoolName)
        {
            _enemy = enemy;
        }

        public override void DoChecks()
        {
            base.DoChecks();
            _enemy.AllRangeCheck();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            var target = _enemy.Target.value.transform;

            Movement.MoveTowards(target.position, _enemy.Data.MovementVelocity);

            if (_enemy.IsTargetInAttackRange)
            {
                stateMachine.ChangeState(_enemy.AttackState);
            }
        }
    }
}