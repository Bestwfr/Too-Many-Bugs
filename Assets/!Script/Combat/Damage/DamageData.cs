using UnityEngine;

namespace FlamingOrange.Combat.Damage
{
    public class DamageData
    {
        public float Amount { get; private set; }
        public GameObject Source { get; private set; }
        public Vector2 Direction { get; private set; }

        public DamageData(float amount, GameObject source, Vector2? dir = null)
        {
            Amount = amount;
            Source = source;
            Direction = dir ?? Vector2.zero;
        }
        
        public void SetAmount(float amount)
        {
            Amount = amount;
        }
    }
}