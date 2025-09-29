using PurrNet;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class Death : CoreComponent
    {
        [SerializeField] private GameObject[] deathParticles;

        private ParticleManager ParticleManager =>
            particleManager ? particleManager : core.GetCoreComponent(ref particleManager);
    
        private ParticleManager particleManager;

        private Stats Stats => stats ? stats : core.GetCoreComponent(ref stats);
        private Stats stats;
    
        [ObserversRpc(requireServer:false)]
        public void Die()
        {
            foreach (var particle in deathParticles)
            {
                ParticleManager.StartParticles(particle);
            }
        
            core.transform.parent.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Stats.OnHealthZero += Die;
        }

        private void OnDisable()
        {
            Stats.OnHealthZero -= Die;
        }
    }
}