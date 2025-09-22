using UnityEngine;

namespace FlamingOrange
{
    public class UnitHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 10f;
        private float current;

        void Awake()
        {
            current = maxHealth;
        }

        public void ApplyDamage(float amount)
        {
            current -= amount;
            if (current <= 0f) Destroy(gameObject);
        }
    }
}
