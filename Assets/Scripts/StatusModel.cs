using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusModel : MonoBehaviour
{
    [SerializeField]
    private float baseSpeed = 3.0f;

    public float Speed => baseSpeed;

    [SerializeField]
    private float baseJump = 6.3f;
    [SerializeField]
    private float maxJumpForce = 10.8f;

    [SerializeField]
    private float maxChargeTime = 1.2f;
    public float MaxChargeTime => maxChargeTime;

    [SerializeField]
    private float landingTime = 0.2f;
    public float LandingTime => landingTime;

    [SerializeField]
    private float accelTime = 0.2f;
    [SerializeField]
    private float deAccelTime = 0.1f;

    public float AccelRate => 1f / accelTime;
    public float DeAccelRate => 1f / deAccelTime;

    public Vector2 TargetVelocity {  get; private set; }
    public Vector2 CurVelocity { get; set; }

    public bool isLocked;

    public float GetJumpForce(float chargeRange)
    {
        if(chargeRange < 0.33f)
        {
            return baseJump;
        }

        return Mathf.Lerp(baseJump, maxJumpForce, chargeRange);
    }

    public void SetTargetVelocity(Vector2 dir)
    {
        TargetVelocity = dir * Speed;
    }
}
