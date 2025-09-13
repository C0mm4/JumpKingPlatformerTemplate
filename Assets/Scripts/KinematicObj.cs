using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class KinematicObj : MonoBehaviour
{
    [SerializeField]
    protected float gravityModifier = 1f;

    [SerializeField] private Rigidbody2D body;
    [SerializeField] protected ContactFilter2D contactFilter;
    [SerializeField] protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

    [SerializeField] protected const float minMoveDistance = 0.001f;    // �ּ� �̵� �Ÿ�
    [SerializeField] protected const float shellRadius = 0.05f;         // ��¦ ���ִ� �Ÿ�


    public bool isGround { get; private set; } = false;     // ���� ����ִ���

    [SerializeField] protected Vector2 velocity;            // �ӷ�
    public Vector2 Velocity { get { return velocity; } }
    private Vector2 additionalVelocty;                      // �ܺ� ��
    private Vector2 groundNormal = Vector2.up;

    [SerializeField] private float minGroundY = 0.995f;     // �ٴ����� �ν��ϴ� Normal �ּڰ�
    [SerializeField] private float minMagnitudeSlideDir = 0.0001f;  // �����̵� ó�� �ּҰ�
    private bool isJump = false;                            // ���� ��������
    private bool isStartJump = false;                       // ������ �����ߴ��� (������ Ʈ����)

    public event Action<Vector2> OnWallHitAction;       // ���浹 �� �̺�Ʈ
    public event Action<bool, float> OnFall;            // ������ �� �̺�Ʈ

    [SerializeField] private bool isSlide = false;      // �����̵� ��������
    [SerializeField] private int slideCheckCount = 10;  // �����̵� �ν� ���� ������
    [SerializeField] private int checkFrame = 0;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if (body == null)
        {
            body = gameObject.AddComponent<Rigidbody2D>();
        }

        body.isKinematic = true;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.layerMask = contactFilter.layerMask & ~LayerMask.GetMask("Structure");
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = false;
    }

    protected virtual void FixedUpdate()
    {
        if (Time.timeScale == 0) return;
        if (body != null)
        {
            // �߷� ó��
            if (gravityModifier > 0f)
            {
                // �ϰ� �ÿ��� gravityModifier ����
                if (velocity.y < 0)
                {
                    velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
                }
                // ��� �� ���� �ӷ� ���� (gravityModifier�� �����ϰ�)
                else
                {
                    velocity += Physics2D.gravity * Time.deltaTime;
                }
            }
            // ���߷µ� ������ �� (���ӿ��� �Ⱦ�����)
            else
            {
                velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
            }

            // Add External Force
            velocity += additionalVelocty;

            // Calculate Movement
            var deltaPos = velocity * Time.deltaTime;
            var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
            var move = moveAlongGround * deltaPos.x;

            // X Axis Movement
            PerformMovement(move, false);

            move = Vector2.up * deltaPos.y;

            // Y Axis Movement
            PerformMovement(move, true);


            // Reset External Force
            velocity -= additionalVelocty;

            // Decay External Force
            DecayAdditionalVelocity();

        }
    }

    public virtual void PerformMovement(Vector2 dir, bool yMovement)
    {
        var distance = dir.magnitude;
        checkFrame++;
        if (distance > minMoveDistance)
        {
            // Check hit buffer
            var cnt = body.Cast(dir, contactFilter, hitBuffer, distance + shellRadius);
            if (cnt == 0)
            {
                isGround = false;
                if (slideCheckCount * 2 < checkFrame)
                {
                    checkFrame = 0;
                    isSlide = false;
                }
            }
            for (int i = 0; i < cnt; i++)
            {
                checkFrame = 0;
                if (hitBuffer[i].collider.isTrigger)
                {
                    continue;
                }

                var currentNormal = hitBuffer[i].normal;
                // Check Bottom hit
                if (currentNormal.y > minGroundY)
                {
                    if (!isGround)
                    {
                        velocity.x = 0;
                    }
                    isGround = true;
                    isJump = false;
                    velocity.y = 0f;
                    groundNormal = currentNormal;
                }
                else
                {
                    if (!isSlide)
                    {
                        OnWallHitAction?.Invoke(currentNormal);
                    }
                    isSlide = true;
                    isGround = false;
                    isJump = false;

                    // ���� ���� ���
                    Vector2 g = Physics2D.gravity.normalized;
                    Vector2 slideDir = g - Vector2.Dot(g, currentNormal) * currentNormal;
                    if (slideDir.sqrMagnitude < minMagnitudeSlideDir) slideDir = g;
                    slideDir.Normalize();

                    // ���� �ӵ� ���� + �߷� ���� �߰�
                    float speedAlongSlide = Vector2.Dot(velocity, slideDir);
                    Vector2 velocityAlongSlide = slideDir * speedAlongSlide;
                    Vector2 gravityAlongSlide = Vector2.Dot(Physics2D.gravity * gravityModifier, slideDir) * slideDir;

                    velocity = velocityAlongSlide + gravityAlongSlide * Time.fixedDeltaTime;

                    // ��� Ż �� �¿���� ����
                    bool isFlipX = velocity.x < 0;

                    dir = velocity * Time.fixedDeltaTime;
                }
                // Check Head hit
                if (currentNormal.y < -0.5f)
                {
                    velocity.y = Mathf.Min(velocity.y, 0);
                }



                var modifiedDistance = hitBuffer[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
            var moveDistance = dir.normalized * distance;
            body.position += moveDistance;


            // y �� �̵��� ���� �߶� ������ �׼� ����
            if (yMovement && !isGround && !isJump)
            {
                OnFall?.Invoke(isStartJump, moveDistance.y);
            }
        }
    }

    public void AddForce(Vector2 force)
    {
        additionalVelocty += force;
    }

    private void DecayAdditionalVelocity()
    {
        additionalVelocty.x *= (1 - 0.1f);
        if (additionalVelocty.y > 0)
        {
            additionalVelocty += gravityModifier * Physics2D.gravity * Time.fixedDeltaTime;
        }
        else
        {
            additionalVelocty.y = 0f;
        }
        if (additionalVelocty.magnitude <= 0.01f)
        {
            additionalVelocty = Vector2.zero;
        }
    }

    public void InputHandler(Vector2 inputVec)
    {
        if (isGround || isJump)
        {
            velocity.x = inputVec.x;
            bool isFlipX = velocity.x < 0;
        }
    }

    public void Jump(float jumpForce, bool isForce = false)
    {
        if (isGround || isForce)
        {
            isJump = true;
            isStartJump = true;
            velocity.y = jumpForce;
        }
    }
}
