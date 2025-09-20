using UnityEngine;
using NaughtyAttributes;

namespace FlamingOrange
{
    public abstract class ItemData : ScriptableObject
    {
        public Sprite Icon;
        public string ItemName;
        public string Description;
        public RarityRating Rarity;
        public bool IsStackable = false;

        [ShowIf("IsStackable")]
        public int MaxStack; 

        public enum RarityRating
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary,
            Mythic,
            Limited
        }
    }
}
