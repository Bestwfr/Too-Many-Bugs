using UnityEngine;

namespace FlamingOrange.Enemies.StateMachine
{
    public class AttackState : State
    {
        private readonly Enemy _enemy;
        
        private bool _isPlayerInAggroRange;
        private bool _isPlayerOutAggroRange;
        private bool _isTargetInAttackRange;
        
        public AttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Enemy enemy) : base(entity, stateMachine, animBoolName)
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

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Attack " + _enemy.Target);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!_isTargetInAttackRange)
            {
                stateMachine.ChangeState(_enemy.ChaseState);
            }
        }
    }
}