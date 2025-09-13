using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChargeState : IPlayerState
{
    private float jumpChargeT;

    public Player player { get; set; }
    public void Init(Player player)
    {
        this.player = player;
    }

    public void EnterState()
    {
        jumpChargeT = Time.time;
        player.status.SetTargetVelocity(Vector2.zero);
    }

    public void ExitState()
    {
        float heldTime = Time.time - jumpChargeT;
        float chargeRange = Mathf.Clamp01(heldTime / player.status.MaxChargeTime);
        player.body.Jump(player.status.GetJumpForce(chargeRange));
        
    }


    public void ReEnterState()
    {
    }

    public void UpdateState()
    {
    }

    public void InputHandle(InputType type)
    {
        if (type == InputType.JumpReleased)
        {
            player.stateMachine.ChangeState<PlayerJumpUpState>();
        }
    }
}
