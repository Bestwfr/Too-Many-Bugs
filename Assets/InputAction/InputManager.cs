using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Transform aimOrigin;
    
    public static InputManager Instance;
    
    public Vector2 MovementDirection { get; private set; }
    public bool DashInput { get; private set; }
    public bool DashInputStop { get; private set; }
    public bool AttackInput { get; private set; }
    public event Action<Vector2> OnAttack;
    
    private Vector2 _aimDir;
    private bool _isAiming;
    private float _dashInputStartTime;

    #region Unity Callbacks Functions
    
    private void Awake()
    {if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (worldCamera == null) worldCamera = Camera.main;
    }

    private void Update()
    {
        if (!_isAiming)
        {
            if (MovementDirection.sqrMagnitude > 0.0001f)
                _aimDir = MovementDirection.normalized;
        }
    }
    
    #endregion

    #region Input Callbacks Functions

    public void OnMoveInput(InputAction.CallbackContext ctx)
    {
        var v = ctx.ReadValue<Vector2>();
        MovementDirection = v.sqrMagnitude > 1f ? v.normalized : v;
    }

    public void OnLookInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            var control = ctx.control;

            switch (control.device)
            {
                case Gamepad or Joystick:
                    GamePadLookLogic(ctx); 
                    return;
                case Mouse:
                    MouseLookLogic(ctx);
                    return;
            }
        }
        else if (ctx.canceled)
        {
            _isAiming = false;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector2 dir = _aimDir.sqrMagnitude > 0.0001f
                ? _aimDir
                : (MovementDirection.sqrMagnitude > 0.0001f ? MovementDirection.normalized : Vector2.up);

            AttackInput = true;
            OnAttack?.Invoke(dir);
        }
    }
    public void UseAttackInput() => AttackInput = false;
    
    public void OnDashInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            DashInput = true;
            DashInputStop = false;
            _dashInputStartTime = Time.time;
        }
        else if (ctx.canceled)
        {
            DashInputStop = true;
        }
    }
    public void UseDashInput() => DashInput = false;

    #endregion
    
    #region Platform Logic Functions

    private void GamePadLookLogic(InputAction.CallbackContext ctx)
    {
        var stick = ctx.ReadValue<Vector2>();
        _isAiming = stick.sqrMagnitude > 0.0001f;
        if (_isAiming) _aimDir = stick.normalized;
    }
    private void MouseLookLogic(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = ctx.ReadValue<Vector2>();
        if (worldCamera != null && aimOrigin != null)
        {
            Vector3 world = worldCamera.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, worldCamera.nearClipPlane)); // ortho ignores z
            world.z = 0f; // keep 2D

            Vector2 dir = (Vector2)(world - aimOrigin.position);
            _isAiming = dir.sqrMagnitude > 0.0001f;
            if (_isAiming) _aimDir = dir.normalized;
        }
    }

    #endregion

    #region Gizmos Functions

    private void OnDrawGizmos()
    {
        if (aimOrigin == null) return;

        // origin point
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(aimOrigin.position, 0.1f);

        // draw direction line
        Vector3 dir3 = new Vector3(_aimDir.x, _aimDir.y, 0f);
        if (dir3.sqrMagnitude > 0.001f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(aimOrigin.position, aimOrigin.position + dir3 * 2f);
        }
    }

    #endregion

}
