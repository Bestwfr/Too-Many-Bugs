using FlamingOrange.CoreSystem;
using PurrNet;
using UnityEngine;
using TMPro;

public class PlayerStateDebugger : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    
    public SyncVar<GameObject> PlayerObj = new(ownerAuth: true);
    public SyncVar<float> health = new(ownerAuth: true);
    public SyncVar<float> speed = new(ownerAuth: true);
    public SyncVar<string> stateName = new(ownerAuth: true);
    public SyncVar<int> facingX = new();

    private Player _player;
    private Stats _stats;
    private Movement _movement;
    
    private readonly Vector3 _baseScale = new(1f, 1f, 1f);

    protected override void OnSpawned()
    {
        base.OnSpawned();
        
        PlayerObj.onChanged += OnPlayerObjChanged;

        if (isOwner)
        {
            PlayerObj.value = gameObject;
            _player = GetComponent<Player>();
            _stats = _player.Core.GetCoreComponent<Stats>();
            _movement = _player.Core.GetCoreComponent<Movement>();
        }
    }

    private void OnPlayerObjChanged(GameObject newVal)
    {
        if (!newVal) return;

        _player = newVal.GetComponent<Player>();
        if (_player)
        {
            _stats = _player.Core.GetCoreComponent<Stats>();
            _movement = _player.Core.GetCoreComponent<Movement>();
        }
    }

    private void Update()
    {
        if (!debugText) return;

        // Only owner updates the SyncVar
        if (isOwner && _movement != null)
        {
            facingX.value = _movement.FacingX;
            health.value = _stats.CurrentHealth;
            speed.value = _movement.CurrentVelocity.magnitude;
            stateName.value = _player.StateMachine.CurrentState.StateName;
        }
        
        if (PlayerObj != null)
        {
            var s = _baseScale;
            s.x = Mathf.Abs(s.x) * facingX.value;
            debugText.transform.localScale = s;

            debugText.text = $"HP: {health.value}" +
                             $"\nState: {stateName.value}" +
                             $"\nSpeed: {speed.value}";
        }
    }

}