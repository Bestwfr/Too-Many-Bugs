using UnityEngine;

public class PlayerIdleState : PlayerState
{
    private Vector2 _inputDirection;
    private bool _dashInput;

    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        _inputDirection = player.Input.MovementDirection;
        _dashInput = player.Input.DashInput;
        
        Movement?.UpdateFacing(_inputDirection);
        
        if (_inputDirection != Vector2.zero)
        {
            stateMachine.ChangeState(player.MoveState);
        }
        else if (_dashInput && player.DashState.CheckIfCanDash() && _inputDirection != Vector2.zero)
        {
            stateMachine.ChangeState(player.DashState);
        }
        else if (player.TryConsumeAttack(out Vector2 dir))
        {
            player.AttackState.RequestDirection(dir);
            stateMachine.ChangeState(player.AttackState);
        }
    }
}