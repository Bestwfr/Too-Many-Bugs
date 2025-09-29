using FlamingOrange.Combat.Damage;
using FlamingOrange.Combat.KnockBack;
using PurrNet;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public class EnemyProjectile : NetworkBehaviour
    {
        public float Damage { get; set; }
        public float Lifetime { get; set; }
        public float Knockback { get; set; }
        public GameObject Source { get; set; }
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }

        protected override void OnSpawned()
        {
            base.OnSpawned();
        }
        
        private void FixedUpdate()
        {
            if (!isServer) return;
            if (TryGetComponent<Rigidbody2D>(out var rb))
                rb.linearVelocity = Direction * Speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            if (isServer)
            {
                var damageable = other.GetComponent<IDamageable>();
                damageable?.Damage(new DamageData(Damage, Source));
            }
            
            var direction = other.transform.position - transform.position;
            
            var knockBackable = other.GetComponent<IKnockBackable>();
            knockBackable?.KnockBack(new KnockBackData(direction.normalized, Knockback, Source));
            
            Destroy(gameObject);
        }

        public void Initialize(float damage,float lifetime, float knockback,GameObject source, Vector2 direction, float speed)
        {
            Damage = damage;
            Lifetime = lifetime;
            Knockback = knockback;
            Source = source;
            Direction = direction;
            Speed = speed;
        }
    }
}