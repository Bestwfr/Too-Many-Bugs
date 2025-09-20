using System;
using FlamingOrange.CoreSystem;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    public abstract class ToolComponent : MonoBehaviour
    {
        protected Tool tool;
        
        // protected AnimationEventHandler EventHandler => tool.EventHandler;
        protected AnimationEventHandler eventHandler;
        protected Core Core => tool.Core;

        protected bool isAttackActive;

        public virtual void Init() { }

        protected virtual void Awake()
        {
            tool = GetComponent<Tool>();

            eventHandler = GetComponentInChildren<AnimationEventHandler>();
        }

        protected virtual void Start() 
        {
            tool.OnEnter += HandleEnter;
            tool.OnExit += HandleExit;
        }

        protected virtual void HandleEnter()
        {
            isAttackActive = true;
        }
        
        protected virtual void HandleExit()
        {
            isAttackActive = false;
        }
        
        protected virtual void OnDestroy()
        {
            tool.OnEnter -= HandleEnter;
            tool.OnExit -= HandleExit;
        }
    }

    public abstract class ToolComponent<T1, T2> : ToolComponent where T1 : ComponentData<T2> where T2 : AttackData
    {
        protected T1 data;
        protected T2 currentAttackData;

        protected override void HandleEnter()
        {
            base.HandleEnter();

            currentAttackData = data.AttackData[tool.CurrentAttackCounter];
        }

        public override void Init()
        {
            base.Init();
            
            data = tool.Data.GetData<T1>();
        }
    }
}