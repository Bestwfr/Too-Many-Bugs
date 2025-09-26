using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.Enemies
{
    [CreateAssetMenu(fileName = "newLongRangeEnemy", menuName = "Data/Enemy Data/Long Range Enemy", order = 1)]
    public class LongRangeEnemyData : EnemyData
    {
        [field: Space(5)]
        [field: HorizontalLine]
        [field: Header("Long Range Specific")]
        [field: SerializeField] public float RetreatVelocity { get; private set; } = 4f;
        [field: SerializeField] public float DistanceToMaintain { get; private set; } = 3f;
        
        [field: Header("Projectile Properties")]
        [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 7f;
        [field: SerializeField] public float ProjectileLifeTime { get; private set; } = 2f;
        [field: SerializeField] public float ProjectileKnockBack { get; private set; } = 2f;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            if (Health == 0f) Health = 50f;
            if (PlayerAggroDistance == 0f) PlayerAggroDistance = 3f;
            if (PlayerDeaggroDistance == 0f) PlayerDeaggroDistance = 8f;
            if (MovementVelocity == 0f) MovementVelocity = 3f;
            if (AttackDamage == 0f) AttackDamage = 1f;
            if (AttackDistance == 0f) AttackDistance = 5f;
            if (AttackFrequencySecond == 0f) AttackFrequencySecond = 0.7f;
            if (InterruptMultiplier == 0f) InterruptMultiplier = 1.2f;
        }
    }
}