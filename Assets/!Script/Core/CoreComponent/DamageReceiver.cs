using UnityEngine;
using FlamingOrange.Combat.Damage;
using NaughtyAttributes;
using PurrNet;


namespace FlamingOrange.CoreSystem
{
    public class DamageReceiver : CoreComponent, IDamageable
    {
        [SerializeField] private GameObject damageParticles;
        [field: SerializeField, ReadOnly] public bool CanTakeDamage { get; set; } = true;
        
        private CoreComp<Stats> _stats;
        private CoreComp<ParticleManager> _particleManager;
        
        public void Damage(DamageData data)
        {
            if (!CanTakeDamage) return;
            
            _stats.Comp?.ApplyDamage(data.Amount);
            ShowDamageParticles();
        }

        [ObserversRpc(requireServer:false)]
        private void ShowDamageParticles()
        {
            _particleManager.Comp?.StartParticlesWithRandomRotation(damageParticles);
        }

        protected override void Awake()
        {
            base.Awake();
            
            _stats = new CoreComp<Stats>(core);
            _particleManager = new CoreComp<ParticleManager>(core);
        }
    }
}