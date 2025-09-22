using UnityEngine;

namespace FlamingOrange
{
    public class TurretUnit : MonoBehaviour, IUnit
    {
        [SerializeField] private string targetTag = "Enemy";
        [SerializeField] private float rescanInterval = 0.2f;

        private SO_TurretUnit data;
        private float nextShotTime;
        private bool isActive;
        private Transform currentTarget;
        private float nextScanTime;

        public void InitializeFromSO(SO_Unit so)
        {
            data = so as SO_TurretUnit;
        }

        public void Activate()
        {
            isActive = true;
            nextScanTime = 0f;
        }

        void Update()
        {
            if (!isActive || data == null) return;

            if (Time.time >= nextScanTime)
            {
                currentTarget = AcquireTarget(currentTarget);
                nextScanTime = Time.time + rescanInterval;
            }

            if (currentTarget == null) return;

            TryShoot(currentTarget.position);
        }

        Transform AcquireTarget(Transform preferred)
        {
            if (IsValidTarget(preferred)) return preferred;

            var hits = Physics2D.OverlapCircleAll(transform.position, data.range);
            Transform best = null;
            float bestSqr = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i].CompareTag(targetTag)) continue;
                float d = (hits[i].transform.position - transform.position).sqrMagnitude;
                if (d < bestSqr)
                {
                    bestSqr = d;
                    best = hits[i].transform;
                }
            }
            return best;
        }

        bool IsValidTarget(Transform t)
        {
            if (t == null) return false;
            if (!t.gameObject.activeInHierarchy) return false;
            if (!t.CompareTag(targetTag)) return false;
            float sqr = (t.position - transform.position).sqrMagnitude;
            return sqr <= data.range * data.range;
        }

        void TryShoot(Vector3 worldPos)
        {
            if (Time.time < nextShotTime) return;
            nextShotTime = Time.time + data.fireInterval;

            Vector2 dir = (worldPos - transform.position).normalized;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f + data.rotationOffset;

            var b = Instantiate(data.bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, ang));
            if (data.bulletScale != Vector2.zero) b.transform.localScale = new Vector3(data.bulletScale.x, data.bulletScale.y, 1f);

            var sr = b.GetComponentInChildren<SpriteRenderer>();
            if (sr && data.bulletSprite) sr.sprite = data.bulletSprite;

            var rb = b.GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = b.transform.up * data.bulletSpeed;

            var bullet = b.GetComponent<UnitBullet>();
            if (bullet) bullet.Initialize(data.bulletSpeed, data.bulletLifetime, targetTag, data.damage);
        }

        void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, data.range);
        }
    }
}
