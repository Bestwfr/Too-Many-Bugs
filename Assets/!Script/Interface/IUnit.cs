using UnityEngine;

namespace FlamingOrange
{
    public interface IUnit
    {
        void InitializeFromSO(TurretData data);
        void Activate();
    }
}


