using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;


public class StateMachine : MonoBehaviour
{
    protected IState currentState;
    private readonly Dictionary<Type, IState> states = new();
    [SerializeField] private string CurrentState;   // State ������

    public void AddState(IState state) 
        => states[state.GetType()] = state;

    public void ChangeState<T>() where T : IState
    {
        // ���� ���°� null�� ���
        if (currentState == null)
        {
            currentState = states[typeof(T)];
            CurrentState = currentState.GetType().Name;
            currentState?.EnterState();
            return;
        }

        // ���ο� ���°� ���� ���¿� �����ϸ� ������ �� ����
        if (currentState?.GetType() == typeof(T))
        {
            currentState.ReEnterState();
            return;
        }

        // ���� ������ ����
        currentState?.ExitState();

        // ���ο� ������ ����
        currentState = states[typeof(T)];
        currentState.EnterState();

        CurrentState = currentState.GetType().Name;
    }

    public void UpdateState()
    {
        currentState?.UpdateState();
    }

}
