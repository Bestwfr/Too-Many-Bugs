using FlamingOrange.Tools.Components;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    public class ToolSpriteData : ComponentData<AttackSprites>
    {
        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(ToolSprite);
        }
    }
}