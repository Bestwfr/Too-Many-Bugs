using System;
using UnityEngine;

namespace FlamingOrange.Tools.Components
{
    [Serializable]
    public class AttackSprites: AttackData
    {
        [field: SerializeField] public Sprite[] Sprites { get; private set; }
    }
}