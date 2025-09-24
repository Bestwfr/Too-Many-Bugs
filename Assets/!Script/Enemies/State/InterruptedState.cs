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
        
        private Timer _interruptDurationTimer;
        
        public InterruptedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _interruptDurationTimer = new Timer(0.05f * Entity.InterruptMultiplier);
            _interruptDurationTimer.OnTimerDone += Entity.StateMachine.RevertToPreviousState;
            
            _interruptDurationTimer.StartTimer();
        }

        public override void Exit()
        {
            base.Exit();
            
            _interruptDurationTimer.StopTimer();
            
            _interruptDurationTimer.OnTimerDone -= Entity.StateMachine.RevertToPreviousState;
            
            Entity.AttackCooldown.StartTimer();
        }

        public override void LogicUpdate()
        {
            _interruptDurationTimer.Tick();
        }
    }
}