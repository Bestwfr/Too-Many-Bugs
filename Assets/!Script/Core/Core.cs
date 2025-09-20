    using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class Core : MonoBehaviour
    {
        [field: SerializeField] public GameObject Root { get; private set; }
        
        private readonly List<CoreComponent> _coreComponents = new List<CoreComponent>();

        private void Awake()
        {
            Root = Root ? Root : transform.parent.gameObject;
        }

        public void LogicUpdate()
       {
           foreach (var coreComponent in _coreComponents)
           {
               coreComponent.LogicUpdate();
           }
       }

       public void PhysicsUpdate()
       {
           foreach (var coreComponent in _coreComponents)
           {
               coreComponent.PhysicsUpdate();
           } 
       }

       public void AddComponent(CoreComponent coreComponent)
       {
           if (!_coreComponents.Contains(coreComponent))
           {
               _coreComponents.Add(coreComponent);
           }
       }

       public T GetCoreComponent<T>() where T : CoreComponent
       {
           var comp = _coreComponents.OfType<T>().FirstOrDefault();
           
           if (comp) return comp;

           comp = GetComponentInChildren<T>();
           
           if (comp) return comp;
           
           Debug.LogWarning($"{typeof(T)} could not be found on {transform.parent.name}");
           return null;
       }

       public T GetCoreComponent<T>(ref T value) where T : CoreComponent
       {
           value = GetCoreComponent<T>();
           return value;
       }
    }
}