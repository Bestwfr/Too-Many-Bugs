using FlamingOrange.CoreSystem;
using PurrNet;

namespace FlamingOrange
{
    public class Base : NetworkBehaviour
    {
        private Core Core { get; set; }
        public Stats Stats { get; private set; }

        private void Awake()
        {
            Core = GetComponentInChildren<Core>();

            Stats = Core.GetCoreComponent<Stats>();
        }

        private void Update()
        {
            if (!isServer) return;

            Core.LogicUpdate();
        }

        private void FixedUpdate()
        {
            if (!isServer) return;

            Core.PhysicsUpdate();
        }
    }
}