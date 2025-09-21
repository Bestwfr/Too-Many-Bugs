using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.Enemies
{
    [CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data/Base Enemy", order = 0)]
    public class EnemyData : ScriptableObject
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
    }
}