using FlamingOrange.Combat.KnockBack;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    public class KnockBack : ToolComponent<KnockBackData, AttackKnockBack>
    {
        private ActionHitBox _hitBox;
        
        private CoreSystem.Movement _movement;

        private void HandleDetectCollider2D(Collider2D[] colliders)
        {
            foreach (var item in colliders)
            {
                if (item.TryGetComponent(out IKnockBackable knockBackable))
                {
                    knockBackable.KnockBack(
                        new Combat.KnockBack.KnockBackData(
                            _movement.GetFacingDirection(),
                            currentAttackData.Strength,
                            Core.Root));
                }
            }
        }
        
        protected override void Start()
        {
            base.Start();
            
            _hitBox = GetComponent<ActionHitBox>();
            _movement = Core.GetCoreComponent<CoreSystem.Movement>();

            _hitBox.OnDetectedCollider2D += HandleDetectCollider2D;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _hitBox.OnDetectedCollider2D -= HandleDetectCollider2D;
        }
    }
}