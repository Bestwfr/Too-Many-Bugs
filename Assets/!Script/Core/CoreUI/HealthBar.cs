using PurrNet;
using UnityEngine;
using UnityEngine.UI;

namespace FlamingOrange.CoreSystem
{
    public class HealthBar : NetworkBehaviour
    {
        [SerializeField] private Stats stats;
        [SerializeField] private Slider slider;

        protected override void OnSpawned()
        {
            base.OnSpawned();
            InitializeSlider();
        }

        private void OnEnable()
        {
            if (stats != null)
            {
                stats.OnHealthDecreased += UpdateHealthBar;
                stats.OnHealthZero += OnHealthZero;
                InitializeSlider();
            }
        }

        private void OnDisable()
        {
            if (stats != null)
            {
                stats.OnHealthDecreased -= UpdateHealthBar;
                stats.OnHealthZero -= OnHealthZero;
            }
        }

        private void InitializeSlider()
        {
            if (slider == null || stats == null) return;

            slider.maxValue = GetMaxHealth();
            slider.value = stats.CurrentHealth.value;
        }

        private void UpdateHealthBar(float _)
        {
            if (slider == null || stats == null) return;

            slider.value = stats.CurrentHealth.value;
        }

        private void OnHealthZero()
        {
            if (slider != null)
                slider.value = 0f;
        }

        private float GetMaxHealth()
        {
            // Access private maxHealth via reflection (or you can expose it in Stats)
            var maxHealthField = stats.GetType().GetField("maxHealth",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (maxHealthField?.GetValue(stats) is PurrNet.SyncVar<float> max)
                return max.value;

            return 1f; // fallback
        }
    }
}