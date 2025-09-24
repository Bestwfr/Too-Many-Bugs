using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.Enemies
{
    [CreateAssetMenu(fileName = "newCloseRangeEnemy", menuName = "Data/Enemy Data/Close Range Enemy", order = 0)]
    public class CloseRangeEnemyData : ScriptableObject
    {
        [Header("Properties")]
        public float health = 40f;
        public float playerAggroDistance = 2f;
        public float playerDeaggroDistance = 2f;
        public LayerMask whatIsPlayer;
        public LayerMask whatIsBase;
        
        [Header("Chase State")]
        public float movementVelocity = 10f;

        [Header("Attack State")] 
        public float attackDamage = 2f;
        public float attackDistance = 1f;
        public float attackFrequencySecond = 1f;
        
        [Header("Interrupted State")]
        public float interruptMultiplier = 0.7f;
    }
}