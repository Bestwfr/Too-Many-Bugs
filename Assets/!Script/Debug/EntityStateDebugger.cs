using System;
using FlamingOrange.CoreSystem;
using FlamingOrange.Enemies;
using PurrNet;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityStateDebugger : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    
    public SyncVar<GameObject> entityObj = new(ownerAuth: true);
    public SyncVar<float> health = new(ownerAuth: true);
    public SyncVar<float> remainingTime = new(ownerAuth: true);
    public SyncVar<string> stateName = new(ownerAuth: true);

    private Entity _entity;
    private Stats _stats;

    protected override void OnSpawned()
    {
        base.OnSpawned();
        
        entityObj.onChanged += OnEntityObjChanged;

        if (isServer)
        {
            entityObj.value = gameObject;
            _entity = GetComponent<Entity>();
            _stats = _entity.Core.GetCoreComponent<Stats>();
        }
    }

    private void OnEntityObjChanged(GameObject newVal)
    {
        if (!newVal) return;

        _entity = newVal.GetComponent<Entity>();
        if (_entity)
        {
            _stats = _entity.Core.GetCoreComponent<Stats>();
        }
    }

    private void Update()
    {
        if (!debugText) return;

        if (isServer && _entity)
        {
            health.value = _stats.CurrentHealth;
            remainingTime.value = _entity.AttackCooldown.RemainingTime;
            stateName.value = _entity.StateMachine.CurrentState.StateName;
        }
        
        string timerText = remainingTime.value <= 0f
            ? "Ready"
            : $"{remainingTime.value:F1}s";

        debugText.text = $"HP: {health.value}" +
                         $"\nState: {stateName.value}" +
                         $"\nAttack: {timerText}";
    }
}