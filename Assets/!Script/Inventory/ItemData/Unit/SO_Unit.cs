using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Serialization;

namespace FlamingOrange
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/SO_Unit")]
    public class SO_Unit : ItemData
    {
        public GameObject unitGameObject;
        public TurretData turretData;

        public override void Use(IItemUseContext context)
        {
            context.EnterBuildMode(this);
        }
    }
}
