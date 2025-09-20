using UnityEngine;
using FlamingOrange.Combat.Damage;


namespace FlamingOrange.CoreSystem
{
    public class DamageReceiver : CoreComponent, IDamageable
    {
        [SerializeField] private GameObject damageParticles;
        
        private CoreComp<Stats> _stats;
        private CoreComp<ParticleManager> _particleManager;
        
        public void Damage(DamageData data)
        {
            Debug.Log(core.transform.parent.name + "Damage!");
            _stats.Comp?.DecreaseHealth(data.Amount);
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