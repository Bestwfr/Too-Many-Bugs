using System;
using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies.StateMachine;
using FlamingOrange.Utilities;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public abstract class Entity : MonoBehaviour
    {
        
        public FiniteStateMachine StateMachine { get; private set; }
        
        public InterruptedState InterruptedState { get; private set; }
        
        public Core Core { get; private set; }
        
        public Animator Anim { get; private set;}
        
        public Timer AttackCooldown { get; protected set; }
        
        public float InterruptMultiplier { get; protected set; }
        
        private Stats _stats;

        public virtual void Awake()
        {
            Core = GetComponentInChildren<Core>();
            Anim = GetComponent<Animator>();
            
            _stats = Core.GetCoreComponent<Stats>();

            StateMachine = new FiniteStateMachine();
            InterruptedState = new InterruptedState(this, StateMachine, "Interrupted");
        }

        private void OnEnable() => _stats.OnHealthDecreased += Interrupt;
        private void OnDisable() => _stats.OnHealthDecreased -= Interrupt;

        protected virtual void Interrupt(float multiplier)
        {
            if (StateMachine.CurrentState == InterruptedState) return;
            
            InterruptMultiplier = multiplier;
            StateMachine.ChangeState(InterruptedState);
            
            AttackCooldown.StopTimer();
        }

        public virtual void Update()
        {
            Core.LogicUpdate();
            StateMachine.CurrentState.LogicUpdate();
        }

        public virtual void FixedUpdate()
        {
            Core.PhysicsUpdate();
            StateMachine.CurrentState.PhysicsUpdate();
        }
    }
}