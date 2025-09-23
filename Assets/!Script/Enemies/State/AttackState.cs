using FlamingOrange.Combat.Damage;
using FlamingOrange.Utilities;
using UnityEngine;

namespace FlamingOrange.Enemies.StateMachine
{
    public class AttackState : State
    {
        private readonly Enemy _enemy;
        
        private bool _isPlayerInAggroRange;
        private bool _isPlayerOutAggroRange;
        private bool _isTargetInAttackRange;

        private bool _hasAttackedOnce;
        
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

            _enemy.AttackCooldown.OnTimerDone += Attack;
            
            if (!_enemy.AttackCooldown.IsActive)
            {
                Attack();
            }
        }

        public override void Exit()
        {
            base.Exit();
            _enemy.AttackCooldown.OnTimerDone -= Attack;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            _enemy.AttackCooldown.Tick();

            if (!_isTargetInAttackRange)
                stateMachine.ChangeState(_enemy.ChaseState);
        }

        private void Attack()
        {
            var target = Physics2D.OverlapCircle(_enemy.transform.position, _enemy.Data.attackDistance, _enemy.AttackLayer);

            var damageable = target?.GetComponentInParent<IDamageable>();
            damageable?.Damage(new DamageData(_enemy.Data.attackDamage, _enemy.gameObject));
            
            _enemy.Anim.SetTrigger("AttackTrigger");

            _enemy.AttackCooldown.StartTimer();
        }
    }
}