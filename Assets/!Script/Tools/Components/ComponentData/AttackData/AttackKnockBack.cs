using System;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    [Serializable]
    public class AttackKnockBack : AttackData
    {
        [field: SerializeField] public float Strength { get; private set; }
    }
}