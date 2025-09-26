using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class Stats : CoreComponent
    {
        public event Action OnHealthZero;
        public event Action<float> OnHealthDecreased;
    
        [SerializeField] private float maxHealth;
        [field: SerializeField, ReadOnly] public float CurrentHealth { get; private set; }

        [Header("Damage Flash")]
        [SerializeField] private bool damageFlash = true;
        
        [SerializeField, ShowIf("damageFlash")] 
        private Color flashColor = Color.white;
        [SerializeField, ShowIf("damageFlash")] 
        private float flashDuration = 0.2f;

        private SpriteRenderer[] _spriteRenderers;
        private Material[] _materials;

        protected override void Awake()
        {
            base.Awake();
            CurrentHealth = maxHealth;
        }

        private void Start()
        {
            InitializeMaterial();
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

        public void DecreaseHealth(float amount)
        {
            CurrentHealth -= amount;
            OnHealthDecreased?.Invoke(amount);
            
            if (damageFlash)
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
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
        }
    }
}