using FlamingOrange.Combat.KnockBack;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class KnockBackReceiver : CoreComponent, IKnockBackable
    {
        [SerializeField] private float maxKnockbackTime = 0.2f;
        
        private bool _isKnockBackActive;
        private float _knockBackStartTime;
        
        private CoreComp<Movement> _movement;

        protected override void Awake()
        {
            base.Awake();

            _movement = new CoreComp<Movement>(core);
        }

        public override void LogicUpdate()
        {
            CheckKnockBack();
        }

        public void KnockBack(KnockBackData data)
        {
            _movement.Comp.SetVelocity(data.Angle * data.Strength);
            _movement.Comp.CanSetVelocity = false;
            _isKnockBackActive = true;
            _knockBackStartTime = Time.time;
        }

        private void CheckKnockBack()
        {
            if (_isKnockBackActive && Time.time >= _knockBackStartTime + maxKnockbackTime)
            {
                _isKnockBackActive =  false;
                _movement.Comp.CanSetVelocity = true;
            }
        }
    }
}