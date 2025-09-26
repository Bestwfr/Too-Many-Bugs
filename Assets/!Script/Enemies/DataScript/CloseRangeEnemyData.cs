using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.Enemies
{
    [CreateAssetMenu(fileName = "newCloseRangeEnemy", menuName = "Data/Enemy Data/Close Range Enemy", order = 0)]
    public class CloseRangeEnemyData : EnemyData
    {
        [field: Header("Close Range Specific")]
        [field: SerializeField] 
        public float KnockBack { get; private set; } = 3f;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            if (Health == 0f) Health = 40f;
            if (PlayerAggroDistance == 0f) PlayerAggroDistance = 2f;
            if (PlayerDeaggroDistance == 0f) PlayerDeaggroDistance = 7f;
            if (MovementVelocity == 0f) MovementVelocity = 5f;
            if (AttackDamage == 0f) AttackDamage = 1f;
            if (AttackDistance == 0f) AttackDistance = 1.5f;
            if (AttackFrequencySecond == 0f) AttackFrequencySecond = 1f;
            if (InterruptMultiplier == 0f) InterruptMultiplier = 0.7f;
        }
    }
}