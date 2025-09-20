using UnityEngine;
using NaughtyAttributes;

namespace FlamingOrange
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/Shop")]
    public abstract class ItemData : ScriptableObject
    {
        public Sprite Icon;
        public string ItemName;
        public int ItemCost;
        public int HealthBoost;
        public int DamageBoost;
        public int WaveRequirement;
        public bool IsStackable = false;

        [ShowIf("IsStackable")]
        public int MaxStack;

    }
}