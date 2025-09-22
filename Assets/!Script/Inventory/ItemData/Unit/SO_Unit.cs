using UnityEngine;
using NaughtyAttributes;

namespace FlamingOrange
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/SO_Unit")]
    public class SO_Unit : ItemData
    {
        public GameObject UnitSprite;
        public float health;
        public float damage;

        public override void Use(IItemUseContext context)
        {
            context.EnterBuildMode(this);
        }
    }
}
