using UnityEngine;

namespace FlamingOrange
{
    public interface IUnit
    {
        void InitializeFromSO(SO_Unit data);
        void Activate();
    }
}


