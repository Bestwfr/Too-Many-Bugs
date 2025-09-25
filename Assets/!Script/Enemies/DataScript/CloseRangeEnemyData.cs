using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.Enemies
{
    [CreateAssetMenu(fileName = "newCloseRangeEnemy", menuName = "Data/Enemy Data/Close Range Enemy", order = 0)]
    public class CloseRangeEnemyData : EnemyData
    {
        private void OnEnable()
        {
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            if (health == 0f) health = 40f;
            if (playerAggroDistance == 0f) playerAggroDistance = 2f;
            if (playerDeaggroDistance == 0f) playerDeaggroDistance = 7f;
            if (movementVelocity == 0f) movementVelocity = 5f;
            if (attackDamage == 0f) attackDamage = 1f;
            if (attackDistance == 0f) attackDistance = 1.5f;
            if (attackFrequencySecond == 0f) attackFrequencySecond = 1f;
            if (interruptMultiplier == 0f) interruptMultiplier = 0.7f;
        }
    }
}