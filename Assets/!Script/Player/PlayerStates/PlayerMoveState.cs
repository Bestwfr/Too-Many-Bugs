using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private Vector2 _inputDirection;
    private bool _dashInput;
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        _inputDirection = player.Input.MovementDirection;
        _dashInput = player.Input.DashInput;
        
        Movement?.UpdateFacing(_inputDirection);
        
        if (_inputDirection == Vector2.zero)
        {
            stateMachine.ChangeState(player.StopState);
        }
        else if (_dashInput && player.DashState.CheckIfCanDash())
        {
            stateMachine.ChangeState(player.DashState);
        }
        else if (player.TryConsumeAttack(out Vector2 dir))
        {
            player.AttackState.RequestDirection(dir);
            stateMachine.ChangeState(player.AttackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        Movement?.SetVelocity(playerData.movementVelocity * _inputDirection.normalized);
    }

    public override void Exit()
    {
        base.Exit();
        
        Movement.Rb.linearVelocity = Vector2.zero;
    }
}