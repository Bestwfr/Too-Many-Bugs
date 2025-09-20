using System;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class CoreComponent : MonoBehaviour
    {
        protected Core core;

        protected virtual void Awake()
        {
            core = transform.parent.GetComponent<Core>();
            
            if (core == null) { Debug.LogError("Core component not found"); }
            core.AddComponent(this);
        }
        
        public virtual void LogicUpdate() { }
        public virtual void PhysicsUpdate() { }
    }
}