using System;
using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies;
using TMPro;
using UnityEngine;

    public class EnemyStateDebugger : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;
        [SerializeField] private TextMeshProUGUI debugText;
        
        private Stats _stats;

        private void Awake()
        {
            if (enemy == null) enemy = GetComponent<Enemy>();
        }

        private void Start()
        {
            _stats = enemy.Core.GetCoreComponent<Stats>();
        }

        private void Update()
        {
            if (enemy == null || debugText == null) return;

            string timerText = enemy.AttackCooldown.RemainingTime <= 0f
                ? "Ready"
                : $"{enemy.AttackCooldown.RemainingTime:F1}s";
            
            debugText.text = $"HP: {_stats.CurrentHealth}" +
                             $"\nState: {enemy.StateMachine.CurrentState.StateName}" +
                             $"\nAttack: {timerText}";
        }
    }