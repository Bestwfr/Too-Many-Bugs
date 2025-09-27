using FlamingOrange.Combat.Damage;
using FlamingOrange.Combat.KnockBack;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public class EnemyProjectile : MonoBehaviour
    {
        public float Damage { get; set; }
        public float Lifetime { get; set; }
        public float Knockback { get; set; }
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
            
            var direction = other.transform.position - transform.position;
            
            var knockBackable = other.GetComponent<IKnockBackable>();
            knockBackable?.KnockBack(new KnockBackData(direction.normalized, Knockback, Source));
            
            Destroy(gameObject);
        }
    }
}