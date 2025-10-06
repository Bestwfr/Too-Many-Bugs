using System;
using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies.StateMachine;
using FlamingOrange.Utilities;
using PurrNet;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public abstract class Entity : NetworkBehaviour
    {
        public FiniteStateMachine StateMachine { get; private set; }
        public InterruptedState InterruptedState { get; private set; }
        
        public Core Core { get; private set; }
        public NetworkAnimator Anim { get; private set; }
        
        public Timer AttackCooldown { get; protected set; }
        public Timer InterruptDurationTimer { get; protected set; }
        public float InterruptMultiplier { get; protected set; }
        
        protected Stats Stats { get; private set; }

        public virtual void Awake()
        {
            Core = GetComponentInChildren<Core>();
            Anim = GetComponent<NetworkAnimator>();

            Stats = Core.GetCoreComponent<Stats>();

            StateMachine = new FiniteStateMachine();
            InterruptedState = new InterruptedState(this, StateMachine, "Interrupted");

            AttackCooldown ??= new Timer(1f);
        }

        private void OnEnable()
        {
            if (Stats != null) Stats.OnHealthDecreased += HandleInterrupt;
        }

        private void OnDisable()
        {
            if (Stats != null) Stats.OnHealthDecreased -= HandleInterrupt;
        }

        private void HandleInterrupt(float multiplier)
        {
            if (!isServer) return;
            InterruptDurationTimer ??= new Timer(0.05f * multiplier);
            Interrupt(multiplier);
        }

        protected virtual void Interrupt(float multiplier)
        {
            if (StateMachine.CurrentState == InterruptedState)
            {
                Anim.SetTrigger("OnHit");
                InterruptDurationTimer.StartTimer();
                return;
            }

            InterruptMultiplier = multiplier;
            StateMachine.ChangeState(InterruptedState);
            AttackCooldown?.StopTimer();
        }

        public virtual void Update()
        {
            if (!isServer) return;

            Core.LogicUpdate();
            StateMachine.CurrentState?.LogicUpdate();
        }

        public virtual void FixedUpdate()
        {
            if (!isServer) return;

            Core.PhysicsUpdate();
            StateMachine.CurrentState?.PhysicsUpdate();
        }
    }
}
