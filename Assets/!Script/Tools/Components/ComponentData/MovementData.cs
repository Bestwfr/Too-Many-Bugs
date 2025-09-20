using FlamingOrange.Tools.Components;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    public class MovementData : ComponentData<AttackMovement>
    {
        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(Movement);
        }
    }
}