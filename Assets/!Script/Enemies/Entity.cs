using System;
using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies.StateMachine;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public abstract class Entity : MonoBehaviour
    {
        
        public FiniteStateMachine StateMachine { get; private set; }
        
        public Core Core { get; private set; }
        
        public Animator Anim { get; private set;}

        public virtual void Awake()
        {
            Core = GetComponentInChildren<Core>();
            Anim = GetComponent<Animator>();

            StateMachine = new FiniteStateMachine();
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