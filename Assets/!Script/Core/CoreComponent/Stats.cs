using System;
using System.Collections;
using NaughtyAttributes;
using PurrNet;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class Stats : CoreComponent
    {
        public event Action OnHealthZero;
        public event Action<float> OnHealthDecreased;
    
        [SerializeField] private SyncVar<float> maxHealth = new();
        [field: SerializeField, ReadOnly] public SyncVar<float> CurrentHealth { get; private set; } = new();

        [Header("Damage Flash")]
        [SerializeField] private bool damageFlash = true;
        
        [SerializeField, ShowIf("damageFlash")] 
        private Color flashColor = Color.white;
        [SerializeField, ShowIf("damageFlash")] 
        private float flashDuration = 0.2f;

        private SpriteRenderer[] _spriteRenderers;
        private Material[] _materials;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CurrentHealth.onChangedWithOld -= HandleDamageFlash;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();
            InitializeMaterial();
            
            if (isServer) 
                CurrentHealth.value = maxHealth.value;
            
            CurrentHealth.onChangedWithOld += HandleDamageFlash;
        }

        private void InitializeMaterial()
        {
            _spriteRenderers = core.Root.GetComponentsInChildren<SpriteRenderer>();
            _materials = new Material[_spriteRenderers.Length];

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                var sr = _spriteRenderers[i];
                
                var mat = new Material(Shader.Find("Shader Graphs/DamageFlash"));
                mat.SetFloat("_FlashAmount", 0f);
                mat.SetColor("_BaseColor", sr.color);
                
                _materials[i] = mat;
                sr.material = mat;
                sr.color = Color.white;
            }
        }
        
        public void InitializeHealth(float health)
        {
            if (!DetermineAuthority()) return;
            
            maxHealth.value = health;
            CurrentHealth.value = maxHealth.value;
        }


        public void DecreaseHealth(float amount)
        {
            CurrentHealth.value -= amount;
            OnHealthDecreased?.Invoke(amount);

            if (CurrentHealth.value <= 0)
            {
                CurrentHealth.value = 0;
                OnHealthZero?.Invoke();
            }
        }
        
        public void ApplyDamage(float amount)
        {
            if (!isServer) 
                DamageServerRpc(amount);
            else
                DecreaseHealth(amount);
        }

        [ServerRpc]
        private void DamageServerRpc(float amount)
        {
            DecreaseHealth(amount);
        }

        private void HandleDamageFlash(float oldValue, float newValue)
        {
            if (newValue > oldValue)  return;
            
            if (damageFlash && _materials != null && isActiveAndEnabled)
                StartCoroutine(DamageFlash());
        }

        private IEnumerator DamageFlash()
        {
            SetFlashColor();

            var elapsedTime = 0f;
            while (elapsedTime < flashDuration)
            {
                elapsedTime += Time.deltaTime;
                
                var currentFlashAmount = Mathf.Lerp(3f, 0f, elapsedTime / flashDuration);
                SetFlashAmount(currentFlashAmount);
                
                yield return null;
            }
        }

        private void SetFlashAmount(float amount)
        {
            foreach (var mat in _materials)
                mat.SetFloat("_FlashAmount", amount);
        }

        private void SetFlashColor()
        {
            foreach (var mat in _materials)
                mat.SetColor("_FlashColor", flashColor);
        }
        
        public void IncreaseHealth(float amount)
        {
            if (!DetermineAuthority()) return;
            
            CurrentHealth.value = Mathf.Clamp(CurrentHealth.value + amount, 0, maxHealth.value);
        }
    }
}