using FlamingOrange.CoreSystem;
using UnityEngine;

namespace FlamingOrange.Enemies.StateMachine
{
    public class State
    {
        protected FiniteStateMachine stateMachine;
        protected Entity Entity;

        protected Core core;
        
        protected float startTime;
        
        protected string animBoolName;

        public State(Entity entity, FiniteStateMachine stateMachine, string animBoolName)
        {
            this.Entity = entity;
            this.stateMachine = stateMachine;
            this.animBoolName = animBoolName;
            core = entity.Core;
        }

        public virtual void Enter()
        {
           startTime = Time.time; 
           Entity.Anim.SetBool(animBoolName, true);
        }

        public virtual void Exit()
        {
            Entity.Anim.SetBool(animBoolName, false);
        }

        public virtual void LogicUpdate()
        {
            
        }

        public virtual void PhysicsUpdate()
        {
            DoChecks();
        }

        public virtual void DoChecks()
        {
            
        }
    }
}