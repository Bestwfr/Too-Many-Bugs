using FlamingOrange.CoreSystem;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public bool CanDash { get; private set; }
    
    private Invincibility Invincibility { get => _invincibility ?? core.GetCoreComponent(ref _invincibility); }
    private Invincibility _invincibility;
    
    private KnockBackReceiver KnockBackReceiver { get => _knockBackReceiver ?? core.GetCoreComponent(ref _knockBackReceiver); }
    private KnockBackReceiver _knockBackReceiver;
    
    private float _lastDashTime;
    
    private Vector2 _dashDirection;
    
    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        CanDash = false;
        player.Input.UseDashInput();
        
        Invincibility.ActivateIFrame(playerData.dashDuration);
        
        _dashDirection = player.Input.MovementDirection;
        
        Movement?.UpdateFacing(_dashDirection);
        
        Movement.SetVelocity(playerData.dashVelocity * _dashDirection);
        Movement.Rb.linearDamping = playerData.dashDrag;
        Movement.CanSetVelocity = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        if (Time.fixedTime >= startTime + playerData.dashDuration)
        {
            Movement.Rb.linearVelocity = Vector2.zero;
            Movement.Rb.linearDamping = 0f;
            _lastDashTime = Time.time;
            Movement.CanSetVelocity = true;
            stateMachine.ChangeState(player.StopState);
        }
    }

    public bool CheckIfCanDash()
    {
        return Time.time >= _lastDashTime + playerData.dashCooldown && !KnockBackReceiver.IsKnockBackActive && !Invincibility.IsIframeActive;
    }
}