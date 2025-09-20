using FlamingOrange.CoreSystem;
using UnityEngine;

public class PlayerState
{
    protected Core core;
    
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    
    protected readonly Player player;
    protected readonly PlayerStateMachine stateMachine;
    protected readonly PlayerData playerData;
    
    protected bool isAnimationFinished;
    
    protected float startTime;

    protected readonly string animBoolName;
    
    public string StateName => animBoolName;

    protected PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData,string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
        core = player.Core;
    }

    public virtual void Enter()
    {
        DoCheck();
        player.Anim.SetBool(animBoolName, true);
        startTime =  Time.time;
        isAnimationFinished = false;
    }

    public virtual void Exit()
    {
        player.Anim.SetBool(animBoolName, false);
    }
    public virtual void LogicUpdate(){}

    public virtual void PhysicsUpdate()
    {
        DoCheck();
    }
    public virtual void DoCheck(){}
    
    public virtual void AnimationTrigger(){}
    public virtual void AnimationFinishedTrigger()
    {
        isAnimationFinished = true;
    }
}