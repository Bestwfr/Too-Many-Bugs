namespace FlamingOrange.Tools.Components
{
    public class DamageData: ComponentData<AttackDamage>
    {
        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(Damage);
        }
    }
}