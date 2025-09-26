using System;
using System.Collections;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class Invincibility : CoreComponent
    {
        [SerializeField] private float duration = 1f;

        private Stats _stats;
        private DamageReceiver _damageReceiver;
        private KnockBackReceiver _knockBackReceiver;
        private Coroutine _iFrameCoroutine;

        protected override void Awake()
        {
            base.Awake();
            _stats = core.GetCoreComponent<Stats>();
            _damageReceiver = core.GetCoreComponent<DamageReceiver>();
            _knockBackReceiver = core.GetCoreComponent<KnockBackReceiver>();
        }

        private void OnEnable() => _stats.OnHealthDecreased += ActivateHitIFrame;
        private void OnDisable() => _stats.OnHealthDecreased -= ActivateHitIFrame;
        
        private void ActivateHitIFrame(float _)
        {
            if (_iFrameCoroutine != null) StopCoroutine(_iFrameCoroutine);

            _iFrameCoroutine = StartCoroutine(StartHitIFrame());
        }

        private IEnumerator StartHitIFrame()
        {
            _damageReceiver.CanTakeDamage = false;
            yield return new WaitForEndOfFrame();
            _knockBackReceiver.CanTakeKnockBack = false;

            yield return new WaitForSeconds(duration);
            _damageReceiver.CanTakeDamage = true;
            _knockBackReceiver.CanTakeKnockBack = true;
        }
    }
}