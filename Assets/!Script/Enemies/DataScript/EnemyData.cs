using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.Enemies
{
    public abstract class EnemyData : ScriptableObject
    {
        [field: Header("Properties")]
        [field: SerializeField] public float Health { get; protected set; }
        [field: SerializeField] public float PlayerAggroDistance { get; protected set; }
        [field: SerializeField] public float PlayerDeaggroDistance { get; protected set; }
        [field: SerializeField] public LayerMask WhatIsPlayer { get; protected set; }
        [field: SerializeField] public LayerMask WhatIsBase { get; protected set; }
        
        [field: Header("Chase State")] 
        [field: SerializeField] public float MovementVelocity { get; protected set; }

        [field: Header("Attack State")]  
        [field: SerializeField] public float AttackDamage { get; protected set; }
        [field: SerializeField] public float AttackDistance { get; protected set; }
        [field: SerializeField] public float AttackFrequencySecond { get; protected set; }
        
        [field: Header("Interrupted State")]
        [field: SerializeField] public float InterruptMultiplier { get; protected set; }

        protected virtual void OnEnable()
        {
            WhatIsPlayer = LayerMask.GetMask("Player");
            WhatIsBase = LayerMask.GetMask("Base");
        }
    }
}