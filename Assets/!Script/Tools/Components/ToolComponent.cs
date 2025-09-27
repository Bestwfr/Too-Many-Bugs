using System;
using FlamingOrange.CoreSystem;
using PurrNet;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    public abstract class ToolComponent : MonoBehaviour
    {
        protected Tool tool;
        
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
        
        public virtual void UpdateAttackState(bool active, int counter, Vector2? direction)
        {
            isAttackActive = active;

            if (direction.HasValue)
                OnDirectionChanged(direction.Value);
        }
        public void UpdateAttackState(bool active) => UpdateAttackState(active, 0, null);
        public void UpdateAttackState(bool active, int counter) => UpdateAttackState(active, counter, null);
        protected virtual void OnDirectionChanged(Vector2 dir) { }

    }

    public abstract class ToolComponent<T1, T2> : ToolComponent where T1 : ComponentData<T2> where T2 : AttackData
    {
        protected T1 data;
        protected T2 currentAttackData;
        
        public override void Init()
        {
            base.Init();
            
            data = tool.Data.GetData<T1>();
        }

        protected override void HandleEnter()
        {
            base.HandleEnter();

            currentAttackData = data.AttackData[tool.CurrentAttackCounter];
        }
        
        public override void UpdateAttackState(bool active, int counter, Vector2? direction)
        {
            base.UpdateAttackState(active, counter, direction);

            if (active && data != null)
            {
                currentAttackData = data.AttackData[counter];
            }
        }
    }
}