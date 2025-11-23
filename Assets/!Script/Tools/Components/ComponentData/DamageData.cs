using UnityEngine;
using System;
using FirstGearGames.SmoothCameraShaker;

namespace FlamingOrange.Tools.Components
{
    [Serializable]
    public class DamageData: ComponentData<AttackDamage>
    {
        [field: SerializeField] public ShakeData ShakeData { get; private set; }
        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(Damage);
        }
    }
}