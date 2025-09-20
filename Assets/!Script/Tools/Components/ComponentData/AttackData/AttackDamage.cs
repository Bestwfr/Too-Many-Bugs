using System;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    [Serializable]
    public class AttackDamage: AttackData
    {
        [field: SerializeField] public float Amount { get; private set; }
    }
}