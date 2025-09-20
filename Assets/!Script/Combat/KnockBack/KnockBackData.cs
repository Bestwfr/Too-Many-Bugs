using UnityEngine;

namespace FlamingOrange.Combat.KnockBack
{
    public class KnockBackData
    {
        public Vector2 Angle { get; private set; }
        public float Strength { get; private set; }
        public GameObject Source { get; private set; }

        public KnockBackData(Vector2 angle, float strength, GameObject source)
        {
            Angle = angle;
            Strength = strength;
            Source = source;
        }
    }
}