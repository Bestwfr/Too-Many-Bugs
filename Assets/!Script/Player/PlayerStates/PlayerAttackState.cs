using FlamingOrange.Tools;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private Vector2 _lookInput;
    private bool _dashInput;

    private Tool _tool;

    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName, Tool tool) : base(player, stateMachine, playerData, animBoolName)
    {
        _tool = tool;

        _tool.OnExit += ExitHandler;
    }

    public override void Enter()
    {
        base.Enter();
        
        _tool.Enter();
        _tool.SetFacingDirection(_lookInput);
        Movement.UpdateFacing(_lookInput);
        
        Movement.Rb.linearDamping = playerData.attackDrag;
        Movement?.SetVelocity(Vector2.zero);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate(); 
        
        _dashInput = player.Input.DashInput;
        
        Movement?.UpdateFacing(_lookInput);

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(player.StopState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        Movement.Rb.linearDamping = 0f;
    }


    private void ExitHandler()
    {
        AnimationFinishedTrigger();
    }
    
    public void RequestDirection(Vector2 dir) => _lookInput = dir;
}