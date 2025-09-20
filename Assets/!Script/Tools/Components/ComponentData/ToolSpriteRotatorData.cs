namespace FlamingOrange.Tools.Components
{
    public class ToolSpriteRotatorData: ComponentData
    {
        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(ToolSpriteRotator);
        }
    }
}