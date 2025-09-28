using System;
using PurrNet;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class CoreComponent : NetworkBehaviour
    {
        protected Core core;

        protected virtual void Awake()
        {
            core = transform.parent.GetComponent<Core>();
            
            if (core == null) { Debug.LogError("Core component not found"); }
            core.AddComponent(this);
        }
        
        protected bool DetermineAuthority() { return hasOwner ? isOwner : isServer; }
        
        public virtual void LogicUpdate() { if (!DetermineAuthority()) return; }
        public virtual void PhysicsUpdate() { if (!DetermineAuthority()) return; }
    }
}