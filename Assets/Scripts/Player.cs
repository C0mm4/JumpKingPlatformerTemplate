using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerController controller;
    [SerializeField] public KinematicObj body;
    [SerializeField] public PlayerStateMachine stateMachine;
    [SerializeField] public StatusModel status;
    private void Awake()
    {
        if(stateMachine == null)
        {
            stateMachine = GetComponent<PlayerStateMachine>();
            if(stateMachine == null )
                stateMachine = gameObject.AddComponent<PlayerStateMachine>();
        }
        stateMachine?.Initialize(this);

        if (body == null)
        {
            body = GetComponent<KinematicObj>();
            if (body == null)
                body = gameObject.AddComponent<KinematicObj>();
        }

        if (controller == null)
        {
            controller = GetComponent<PlayerController>();
            if (controller == null)
                controller = gameObject.AddComponent<PlayerController>();
        }

    }
}
