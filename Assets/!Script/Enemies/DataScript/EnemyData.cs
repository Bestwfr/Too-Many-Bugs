using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.Enemies
{
    public abstract class EnemyData : ScriptableObject
    {
        [Header("Properties")]
        public float health;
        public float playerAggroDistance;
        public float playerDeaggroDistance;
        public LayerMask whatIsPlayer;
        public LayerMask whatIsBase;
        
        [Header("Chase State")]
        public float movementVelocity;

        [Header("Attack State")] 
        public float attackDamage;
        public float attackDistance;
        public float attackFrequencySecond;
        
        [Header("Interrupted State")]
        public float interruptMultiplier;

        private void OnEnable()
        {
            whatIsPlayer = LayerMask.GetMask("Player");
            whatIsBase = LayerMask.GetMask("Base");
        }
    }
}