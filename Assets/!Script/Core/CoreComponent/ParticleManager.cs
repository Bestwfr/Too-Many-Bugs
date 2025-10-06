using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class ParticleManager : CoreComponent
    {
        private Transform particleContainer;

        protected override void Awake()
        {
            base.Awake();
            
            particleContainer = GameObject.FindGameObjectWithTag("ParticleContainer").transform;
        }

        public GameObject StartParticles(GameObject particlePrefab, Vector2 position, Quaternion rotation)
        {
            return Instantiate(particlePrefab, position, rotation, particleContainer);
        }

        public void StartParticles(GameObject particlePrefab)
        {
            StartParticles(particlePrefab, transform.position, Quaternion.identity);
        }

        public GameObject StartParticlesWithRandomRotation(GameObject particlePrefab)
        {
            var randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            return StartParticles(particlePrefab, transform.position, randomRotation);
        }
        
        public GameObject StartParticlesOppositeTo(GameObject source, GameObject particlePrefab)
        {
            if (source == null)
            {
                Debug.LogWarning("Source GameObject is null.");
                return null;
            }
            
            Vector2 direction = transform.position - source.transform.position;
            
            var rotation = Quaternion.FromToRotation(Vector2.up, direction);

            return StartParticles(particlePrefab, transform.position, rotation);
        }
        
        public GameObject StartParticlesInDirection(GameObject particlePrefab, Vector2 direction)
        {
            if (direction == Vector2.zero)
                direction = Vector2.up;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

            return StartParticles(particlePrefab, transform.position, rotation);
        }
    }
}
