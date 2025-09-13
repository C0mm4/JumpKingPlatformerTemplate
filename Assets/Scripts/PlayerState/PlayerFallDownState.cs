using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallDownState : IPlayerState
{
    public Player player { get; set; }
    public void Init(Player player)
    {
        this.player = player;
    }
    public void EnterState()
    {
    }

    public void ExitState()
    {
    }
    public void ReEnterState() { }

    public void UpdateState()
    {
        if (player.body.isGround)
            player.stateMachine.ChangeState<PlayerLandingState>();

        player.status.CurVelocity =
            Vector2.Lerp(player.status.CurVelocity, player.status.TargetVelocity,
            player.status.AccelRate * Time.deltaTime);
        player.body.InputHandler(player.status.CurVelocity);
    }

    public void InputHandle(InputType type) { }


}
