using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    protected Player Player;

    public void Initialize(Player player) 
    {
        this.Player = player;

        PlayerChargeState playerChargeState = new PlayerChargeState();
        playerChargeState.Init(player);
        AddState(playerChargeState);

        PlayerJumpUpState jumpState = new PlayerJumpUpState();
        jumpState.Init(player);
        AddState(jumpState);

        PlayerFallDownState fallState = new PlayerFallDownState();
        fallState.Init(player);
        AddState(fallState);

        PlayerIdleState idleState = new PlayerIdleState();
        idleState.Init(player);
        AddState(idleState);

        PlayerMoveState moveState = new PlayerMoveState();
        moveState.Init(player);
        AddState(moveState);

        PlayerLandingState landState = new PlayerLandingState();
        landState.Init(player);
        AddState(landState);

        ChangeState<PlayerIdleState>();
    }


    public void OnInput(InputType type)
    {
        if (currentState == null) return;
        ((IPlayerState)currentState)?.InputHandle(type);
    }
}

public enum InputType
{
    Move, StopMove, JumpPressed, JumpReleased
}