using FlamingOrange.Combat.KnockBack;
using NaughtyAttributes;
using PurrNet;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class KnockBackReceiver : CoreComponent, IKnockBackable
    {
        [SerializeField] private float maxKnockbackTime = 0.2f;
        [field: SerializeField, ReadOnly] public bool CanTakeKnockBack { get; set; } = true;
        [field: SerializeField, ReadOnly] public bool IsKnockBackActive { get; private set; }
        
        private float _knockBackStartTime;
        
        private CoreComp<Movement> _movement;

        protected override void Awake()
        {
            base.Awake();

            _movement = new CoreComp<Movement>(core);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            CheckKnockBack();
        }

        public void KnockBack(KnockBackData data)
        {
            //TODO: Fix delayed knockBack via predicted rigidbody
            if (DetermineAuthority())
            {
                ApplyKnockBack(data);
            }
            else
            {
                KnockBackServerRpc(data);
            }
        }
        
        [ObserversRpc]
        private void KnockBackServerRpc(KnockBackData data)
        {
            ApplyKnockBack(data);
        }

        private void ApplyKnockBack(KnockBackData data)
        {
            if (!CanTakeKnockBack) return;

            _movement.Comp.SetVelocity(data.Angle * data.Strength);
            _movement.Comp.CanSetVelocity = false;
            IsKnockBackActive = true;
            _knockBackStartTime = Time.time;
        }

        private void CheckKnockBack()
        {
            if (IsKnockBackActive && Time.time >= _knockBackStartTime + maxKnockbackTime)
            {
                IsKnockBackActive =  false;
                _movement.Comp.CanSetVelocity = true;
                _movement.Comp.SetVelocity(Vector2.zero);
            }
        }
    }
}