using UnityEngine;

namespace FlamingOrange.CoreSystem
{
    public class CoreComp<T> where T : CoreComponent
    {
        private Core _core;
        private T _comp;
        
        public T Comp => _comp ? _comp : _core.GetCoreComponent(ref _comp);

        public CoreComp(Core core)
        {
            if (core == null)
            {
                Debug.LogWarning($"Core is Null for component {typeof(T)}");
            }
            
            _core = core;
        }
    }
}