using System;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    [Serializable]
    public abstract class ComponentData
    {
        [SerializeField, HideInInspector] private string name;
        
        public Type ComponentDependency { get; protected set; }

        public ComponentData()
        {
            SetComponentName();
            SetComponentDependency();
        }
        
        public void SetComponentName() => name = GetType().Name;
        
        protected abstract void SetComponentDependency();

        public virtual void SetAttackDataName() {}
        
        public virtual void InitializeAttackData(int numberOfAttacks) {}
    }

    [Serializable]
    public abstract class ComponentData<T> : ComponentData where T : AttackData
    {
        [SerializeField] private T[] attackData;
       public T[] AttackData { get => attackData; private set => attackData = value; }

       public override void SetAttackDataName()
       {
           for (var i = 0; i < AttackData.Length; i++)
           {
               AttackData[i].SetAttackName(i + 1);
           }
       }

       public override void InitializeAttackData(int numberOfAttacks)
       {
           base.InitializeAttackData(numberOfAttacks);
           
           var oldLength = attackData?.Length ?? 0;

           if (oldLength == numberOfAttacks) return;
           
           Array.Resize(ref attackData, numberOfAttacks);

           if (oldLength < numberOfAttacks)
           {
               for (var i = oldLength; i < attackData.Length; i++)
               {
                   var newObj = Activator.CreateInstance(typeof(T)) as T;
                   attackData[i] = newObj;
               }
           }
           SetAttackDataName();
       }
    }
}