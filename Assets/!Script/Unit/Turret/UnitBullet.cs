using UnityEngine;

namespace FlamingOrange
{
    public class UnitBullet : MonoBehaviour
    {
        private float speed;
        private float life;
        private string targetTag;
        private float damage;
        private float t;

        public void Initialize(float speed, float life, string targetTag, float damage)
        {
            this.speed = speed;
            this.life = life;
            this.targetTag = targetTag;
            this.damage = damage;
        }

        void Update()
        {
            t += Time.deltaTime;
            if (t >= life) Destroy(gameObject);
            if (GetComponent<Rigidbody2D>() == null) transform.position += transform.up * speed * Time.deltaTime;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(targetTag)) return;
            var hp = other.GetComponent<UnitHealth>();
            if (hp) hp.ApplyDamage(damage);
            Destroy(gameObject);
        }
    }
}
