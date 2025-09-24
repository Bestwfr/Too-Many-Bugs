using FlamingOrange.Combat.Damage;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public class EnemyProjectile : MonoBehaviour
    {
        public float Damage { get; set; }
        public float Lifetime { get; set; }
        public GameObject Source { get; set; }

        private void Start()
        {
            Destroy(gameObject, Lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            var damageable = other.GetComponent<IDamageable>();
            damageable?.Damage(new DamageData(Damage, Source));
            
            Destroy(gameObject);
        }
    }
}