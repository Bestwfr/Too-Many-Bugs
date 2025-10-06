using FlamingOrange.Combat.Damage;
using FlamingOrange.CoreSystem;
using FlamingOrange.Utilities;
using UnityEngine;

namespace FlamingOrange.Enemies.StateMachine
{
    public class InterruptedState : State
    {
        private Movement Movement { get => _movement ?? core.GetCoreComponent(ref _movement); }
        private Movement _movement;
        
        private Entity _entity;
        
        public InterruptedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
        {
            _entity = entity;
        }

        public override void Enter()
        {
            base.Enter();
            _entity.Anim.ResetTrigger("OnHit");
            
            _entity.InterruptDurationTimer.OnTimerDone += Entity.StateMachine.RevertToPreviousState;
            
            _entity.InterruptDurationTimer.StartTimer();
        }

        public override void Exit()
        {
            base.Exit();
            
            _entity.InterruptDurationTimer.StopTimer();
            
            _entity.InterruptDurationTimer.OnTimerDone -= Entity.StateMachine.RevertToPreviousState;
            
            Entity.AttackCooldown.StartTimer();
        }

        public override void LogicUpdate()
        {
            _entity.InterruptDurationTimer.Tick();
        }
    }
}