using FlamingOrange.Enemies;
using UnityEngine;

namespace FlamingOrange
{
    public class TurretUnit : MonoBehaviour, IUnit
    {
        [SerializeField] private string targetTag = "Enemy";
        [SerializeField] private float rescanInterval = 0.2f;

        private TurretData data;
        private float nextShotTime;
        private bool isActive;
        private Transform currentTarget;
        private float nextScanTime;

        public void InitializeFromSO(TurretData so)
        {
            data = so;
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

            var direction = worldPos - transform.position;

            var b = Instantiate(data.ProjectilePrefab, transform.position, Quaternion.identity);

            var proj = b.GetComponent<UnitProjectile>();

            proj.Initialize(
                data.damage, 
                data.ProjectileLifeTime, 
                data.ProjectileKnockBack, 
                gameObject, 
                direction.normalized, 
                data.ProjectileSpeed);
        }

        void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, data.range);
        }
    }
}
