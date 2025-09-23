using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class Stats : CoreComponent
    {
        public event Action OnHealthZero;
    
        [SerializeField] private float maxHealth;
        [field: SerializeField] public float CurrentHealth { get; private set; }

        [SerializeField] private bool damageFlash = true;
        
        [SerializeField, ShowIf("damageFlash")] 
        private Color flashColor = Color.white;
        [SerializeField, ShowIf("damageFlash")] 
        private float flashDuration = 0.25f;

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Material _material;

        protected override void Awake()
        {
            base.Awake();
            CurrentHealth = maxHealth;
            
            _spriteRenderer = GetComponentInParent<SpriteRenderer>();
            _originalColor = _spriteRenderer.color;
            
            InitializeMaterial();
        }

        private void InitializeMaterial()
        {
            _material = new Material(Shader.Find("Shader Graphs/DamageFlash"));
            _material.SetFloat("_FlashAmount", 0f);
            _material.SetColor("_BaseColor", _originalColor);
            _spriteRenderer.color = Color.white;
            _spriteRenderer.material = _material;
        }

        public void DecreaseHealth(float amount)
        {
            CurrentHealth -= amount;
            
            if (_spriteRenderer && damageFlash)
                StartCoroutine(FlashWhite());

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                OnHealthZero?.Invoke();
                Debug.Log("Health is zero!!");
            }
        }

        private IEnumerator FlashWhite()
        {
            SetFlashColor();

            var elapsedTime = 0f;
            while (elapsedTime < flashDuration)
            {
                elapsedTime += Time.deltaTime;
                
                var currentFlashAmount = Mathf.Lerp(2f, 0f, elapsedTime / flashDuration);
                SetFlashAmount(currentFlashAmount);
                
                yield return null;
            }
        }

        private void SetFlashAmount(float amount)
        {
            _material.SetFloat("_FlashAmount", amount);
        }
        
        private void SetFlashColor()
        {
            _material.SetColor("_FlashColor", flashColor);
        }


        public void IncreaseHealth(float amount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
        }
    }
}