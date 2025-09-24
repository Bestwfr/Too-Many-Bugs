using System;
using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityStateDebugger : MonoBehaviour
    {
        [FormerlySerializedAs("enemyCloseRange")][SerializeField] private Entity entity;
        [SerializeField] private TextMeshProUGUI debugText;
        
        private Stats _stats;

        private void Awake()
        {
            if (entity == null) entity = GetComponent<EnemyCloseRange>();
        }

        private void Start()
        {
            _stats = entity.Core.GetCoreComponent<Stats>();
        }

        private void Update()
        {
            if (entity == null || debugText == null) return;

            string timerText = entity.AttackCooldown.RemainingTime <= 0f
                ? "Ready"
                : $"{entity.AttackCooldown.RemainingTime:F1}s";
            
            debugText.text = $"HP: {_stats.CurrentHealth}" +
                             $"\nState: {entity.StateMachine.CurrentState.StateName}" +
                             $"\nAttack: {timerText}";
        }
    }