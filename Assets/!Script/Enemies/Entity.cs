using System;
using FlamingOrange.CoreSystem;
using UnityEngine;

namespace FlamingOrange.Enemies
{
    public class Entity : MonoBehaviour
    {
        public Core Core { get; private set; }

        private void Awake()
        {
            Core = GetComponentInChildren<Core>();
        }

        private void Update()
        {
            Core.LogicUpdate();
        }

        private void FixedUpdate()
        {
            Core.PhysicsUpdate();
        }
    }
}