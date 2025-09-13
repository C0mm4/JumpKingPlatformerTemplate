using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Inspector 연결 오브젝트")]
    [SerializeField] private StatusModel statusModel;
    [SerializeField] private PlayerStateMachine stateMachine;

    public Vector2 CurMovementInput { get; private set; }
    public bool IsJumpKeyHeld { get; private set; }
    public bool IsMoveKeyHeld { get; private set; }

    public bool isLocked;

    private void Update()
    {
        if (Time.timeScale == 0) return;

        // 상태 업데이트만 호출
        stateMachine.UpdateState();
        statusModel.SetTargetVelocity(CurMovementInput);
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0) return;
        Vector2 input = context.ReadValue<Vector2>();

        if (context.phase == InputActionPhase.Performed)
        {
            if (isLocked) return;
            CurMovementInput = input;
            IsMoveKeyHeld = true;
            stateMachine.OnInput(InputType.Move);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            CurMovementInput = Vector2.zero;
            IsMoveKeyHeld = false;
            stateMachine.OnInput(InputType.StopMove);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0) return;
        if (context.phase == InputActionPhase.Started)
        {
            IsJumpKeyHeld = true;
            stateMachine.OnInput(InputType.JumpPressed);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            IsJumpKeyHeld = false;
            stateMachine.OnInput(InputType.JumpReleased);
        }
    }

}
