using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.CoreSystem
{
    public class Invincibility : CoreComponent
    {
        [SerializeField] private float hitIframeDuration = 0.15f;
        [field: SerializeField, ReadOnly] public bool IsIframeActive { get; private set; }
        
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

        private void OnEnable() => _stats.OnHealthDecreased += OnHitIFrame;
        private void OnDisable() => _stats.OnHealthDecreased -= OnHitIFrame;
        
        public void ActivateIFrame(float duration)
        {
            if (!DetermineAuthority()) return;
            
            if (_iFrameCoroutine != null) StopCoroutine(_iFrameCoroutine);
            
            _iFrameCoroutine = StartCoroutine(StartIFrame(duration));
        }
        
        private void OnHitIFrame(float _)
        {
            if (!DetermineAuthority()) return;
            
            if (_iFrameCoroutine != null) StopCoroutine(_iFrameCoroutine);

            _iFrameCoroutine = StartCoroutine(StartIFrame(hitIframeDuration));
        }

        private IEnumerator StartIFrame(float duration)
        {
            IsIframeActive = true;
            
            _damageReceiver.CanTakeDamage = false;
            yield return new WaitForEndOfFrame();
            if (_knockBackReceiver)
                _knockBackReceiver.CanTakeKnockBack = false;

            yield return new WaitForSeconds(duration);
            
            IsIframeActive = false;
            
            _damageReceiver.CanTakeDamage = true;
            if (_knockBackReceiver)
                _knockBackReceiver.CanTakeKnockBack = true;
        }
    }
}