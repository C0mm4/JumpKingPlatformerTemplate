using UnityEngine;

public class PlayerJumpUpState : IPlayerState
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
        if (player.body.Velocity.y <= 0)
            player.stateMachine.ChangeState<PlayerFallDownState>();

        player.status.CurVelocity =
            Vector2.Lerp(player.status.CurVelocity, player.status.TargetVelocity,
            player.status.AccelRate * Time.deltaTime);
        player.body.InputHandler(player.status.CurVelocity);
    }

    public void InputHandle(InputType type) { }

}
