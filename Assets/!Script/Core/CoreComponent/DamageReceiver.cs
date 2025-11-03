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
            
            if (!damageParticles) return;
            
            ShowDamageParticles(data);
        }

        [ObserversRpc(requireServer:false)]
        private void ShowDamageParticles(DamageData data)
        {
            if (data.Direction == Vector2.zero) _particleManager.Comp?.StartParticlesOppositeTo(data.Source,damageParticles);
            else _particleManager.Comp?.StartParticlesInDirection(damageParticles, data.Direction);
        }

        protected override void Awake()
        {
            base.Awake();
            
            _stats = new CoreComp<Stats>(core);
            _particleManager = new CoreComp<ParticleManager>(core);
        }
    }
}