using System;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    [Serializable]
    public class ActionHitboxData : ComponentData<AttackActionHitBox>
    {
        [field: SerializeField] public LayerMask DetectableLayer { get; private set; }

        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(ActionHitBox);
        }
    }
}