using UnityEngine;

namespace FlamingOrange.Enemies.StateMachine
{
    public class FiniteStateMachine
    {
        public State CurrentState { get; private set; }
        private State previousState;

        public void Initialize(State startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public void ChangeState(State newState)
        {
            CurrentState.Exit();
            
            previousState = CurrentState;
            CurrentState = newState;
            
            CurrentState.Enter();
        }
        
        public void RevertToPreviousState()
        {
            if (previousState != null)
                ChangeState(previousState);
        }
}
}