using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.Enemies
{
    [CreateAssetMenu(fileName = "newLongRangeEnemy", menuName = "Data/Enemy Data/Long Range Enemy", order = 1)]
    public class LongRangeEnemyData : EnemyData
    {
        [Space(5)]
        [HorizontalLine]
        [Header("Long Range Specific")]
        public float retreatVelocity = 4f;
        public float distanceToMaintain = 3f;
        
        [Header("Projectile Properties")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 7f;
        public float projectileLifeTime = 2f;
        
        private void OnEnable()
        {
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            if (health == 0f) health = 50f;
            if (playerAggroDistance == 0f) playerAggroDistance = 3f;
            if (playerDeaggroDistance == 0f) playerDeaggroDistance = 8f;
            if (movementVelocity == 0f) movementVelocity = 3f;
            if (attackDamage == 0f) attackDamage = 1f;
            if (attackDistance == 0f) attackDistance = 5f;
            if (attackFrequencySecond == 0f) attackFrequencySecond = 0.7f;
            if (interruptMultiplier == 0f) interruptMultiplier = 1.2f;
        }
    }
}