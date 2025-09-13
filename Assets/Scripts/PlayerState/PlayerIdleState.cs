using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    public Player player { get; set; }
    public void Init(Player player)
    {
        this.player = player;
    }
    public void EnterState(){}
    public void ExitState() { }
    public void ReEnterState() { }
    public void UpdateState()
    {
        if (!player.body.isGround)
            player.stateMachine.ChangeState<PlayerFallDownState>();
    }

    public void InputHandle(InputType type)
    {
        if (type == InputType.Move)
            player.stateMachine.ChangeState<PlayerMoveState>();
        else if (type == InputType.JumpPressed)
            player.stateMachine.ChangeState<PlayerChargeState>();
    }

}
