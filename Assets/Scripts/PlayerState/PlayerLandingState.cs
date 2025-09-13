using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingState : IPlayerState
{
    private float landingStart;
    public Player player { get; set; }
    public void Init(Player player)
    {
        this.player = player;
    }

    public void EnterState()
    {
        landingStart = Time.time;
        player.status.SetTargetVelocity(Vector2.zero);

    }

    public void ExitState() { }
    public void ReEnterState() { }

    public void UpdateState()
    {
        if (Time.time - landingStart > player.status.LandingTime)
        {
            if (player.controller.CurMovementInput != Vector2.zero)
                player.stateMachine.ChangeState<PlayerMoveState>();
            else
                player.stateMachine.ChangeState<PlayerIdleState>();
        }
    }

    public void InputHandle(InputType type) { }
}
