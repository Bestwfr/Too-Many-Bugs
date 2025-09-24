using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.Enemies
{
    [CreateAssetMenu(fileName = "newLongRangeEnemy", menuName = "Data/Enemy Data/Long Range Enemy", order = 1)]
    public class LongRangeEnemyData : ScriptableObject
    {
        [Header("Properties")]
        public float health = 50f;
        public float playerAggroDistance = 3f;
        public float playerDeaggroDistance = 8f;
        public LayerMask whatIsPlayer;
        public LayerMask whatIsBase;
        
        [Header("Chase State")]
        public float movementVelocity = 3f;
        public float retreatVelocity = 5f;
        public float distanceToMaintain = 4f;

        [Header("Attack State")] 
        public float attackDamage = 1f;
        public float attackDistance = 5f;
        public float attackFrequencySecond = 1f; 
        [Space(5)]
        public GameObject projectilePrefab;
        public float projectileSpeed = 5f;
        public float projectileLifeTime = 5f;
        
        [Header("Interrupted State")]
        public float interruptMultiplier = 1.2f;
    }
}