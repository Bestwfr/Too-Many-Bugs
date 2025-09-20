using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlamingOrange.Tools.Components
{
    [Serializable] 
    public class AttackMovement: AttackData
    {
        [field: SerializeField] public float MoveForce { get; private set; }
    }
}