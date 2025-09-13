using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : IPlayerState
{
    public Player player { get; set; }

    public void Init(Player player)
    {
        this.player = player;
        footStepRate = .5f;
    }

    private float footStepRate;
    private float lastFootStepPlayTime;

    public void EnterState(){}
    public void ExitState() { }
    public void ReEnterState() { }

    public void UpdateState()
    {
        if (!player.body.isGround)
        {
            player.stateMachine.ChangeState<PlayerFallDownState>();
            return;
        }

        player.status.CurVelocity = 
            Vector2.Lerp(player.status.CurVelocity, player.status.TargetVelocity, 
            player.status.AccelRate * Time.deltaTime);

        if (!player.controller.IsMoveKeyHeld)
        {
            if(Mathf.Abs(player.status.CurVelocity.x) <= 0.05f)
            {
                player.stateMachine.ChangeState<PlayerIdleState>();
            }
        }

        player.body.InputHandler(player.status.CurVelocity);
    }

    public void InputHandle(InputType type)
    {
        if (type == InputType.JumpPressed)
            player.stateMachine.ChangeState<PlayerChargeState>();
    }

}
