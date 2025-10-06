using FlamingOrange.Combat.Damage;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    public class Damage : ToolComponent<DamageData, AttackDamage>
    {
        private ActionHitBox _hitBox;
        
        private CoreSystem.Movement _movement;

        private void HandleDetectDamage2D(Collider2D[] colliders)
        {
            foreach (var item in colliders)
            {
                if (item.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(new Combat.Damage.DamageData(currentAttackData.Amount, Core.Root,_movement.GetFacingDirection()));
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            _hitBox = GetComponent<ActionHitBox>();
            _movement = Core.GetCoreComponent<CoreSystem.Movement>();
            
            _hitBox.OnDetectedCollider2D += HandleDetectDamage2D;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _hitBox.OnDetectedCollider2D -= HandleDetectDamage2D;
        }
}
}