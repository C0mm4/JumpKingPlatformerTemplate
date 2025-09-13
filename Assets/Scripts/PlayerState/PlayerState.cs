using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState : IState
{
    void Init(Player player);
    Player player { get; set; }

    void InputHandle(InputType type);
}
