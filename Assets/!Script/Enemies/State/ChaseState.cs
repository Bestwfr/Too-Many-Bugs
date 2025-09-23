using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies;

namespace FlamingOrange.Enemies.StateMachine
{
    public class ChaseState : State
    {
        private readonly Enemy _enemy;

        private bool _isPlayerInAggroRange;
        private bool _isPlayerOutAggroRange;
        private bool _isTargetInAttackRange;
        
        private Movement Movement { get => _movement ?? core.GetCoreComponent(ref _movement); }
        private Movement _movement;
        
        public ChaseState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Enemy enemy) : base(entity, stateMachine, animBoolName)
        {
            _enemy = enemy;
        }

        public override void DoChecks()
        {
            base.DoChecks();

            _isPlayerInAggroRange = _enemy.CheckPlayerInAggroRange();
            _isPlayerOutAggroRange = _enemy.CheckPlayerOutAggroRange();
            _isTargetInAttackRange = _enemy.CheckInAttackRange();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            var target = _enemy.Target.transform;

            Movement.MoveTowards(target.position, _enemy.Data.movementVelocity);

            if (_isTargetInAttackRange)
            {
                stateMachine.ChangeState(_enemy.AttackState);
            }
        }
        
    }
}