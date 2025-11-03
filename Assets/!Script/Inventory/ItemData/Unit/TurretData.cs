using NaughtyAttributes;
using UnityEngine;

namespace FlamingOrange
{
    [CreateAssetMenu(fileName = "newTurretData", menuName = "Data/TurretData")]
    public class TurretData : ScriptableObject
    {
        public float health;
        public float damage;
        public float range = 5f;
        public float fireInterval = 0.5f;
        public float bulletSpeed = 8f;
        public float bulletLifetime = 3f;
        
        [field: Header("Projectile Properties")]
        [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 7f;
        [field: SerializeField] public float ProjectileLifeTime { get; private set; } = 2f;
        [field: SerializeField] public float ProjectileKnockBack { get; private set; } = 2f;
    }
}