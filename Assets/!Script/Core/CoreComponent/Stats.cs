using System;
using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class Stats : CoreComponent
    {
        public event Action OnHealthZero;
    
        [SerializeField] private float maxHealth;
        private float _currentHealth;

        protected override void Awake()
        {
            base.Awake();

            _currentHealth = maxHealth;
        }

        public void DecreaseHealth(float amount)
        {
            _currentHealth -= amount;

            if(_currentHealth <= 0)
            {
                _currentHealth = 0;
            
                OnHealthZero?.Invoke();
            
                Debug.Log("Health is zero!!");
            }
        }

        public void IncreaseHealth(float amount)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, maxHealth);
        }
    }
}
