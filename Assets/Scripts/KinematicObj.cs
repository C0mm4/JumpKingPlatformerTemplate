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

    [SerializeField] protected const float minMoveDistance = 0.001f;    // 최소 이동 거리
    [SerializeField] protected const float shellRadius = 0.05f;         // 살짝 떠있는 거리


    public bool isGround { get; private set; } = false;     // 땅을 밟고있는지

    [SerializeField] protected Vector2 velocity;            // 속력
    public Vector2 Velocity { get { return velocity; } }
    private Vector2 additionalVelocty;                      // 외부 힘
    private Vector2 groundNormal = Vector2.up;

    [SerializeField] private float minGroundY = 0.995f;     // 바닥으로 인식하는 Normal 최솟값
    [SerializeField] private float minMagnitudeSlideDir = 0.0001f;  // 슬라이드 처리 최소값
    private bool isJump = false;                            // 점프 상태인지
    private bool isStartJump = false;                       // 점프로 시작했는지 (실족사 트리거)

    public event Action<Vector2> OnWallHitAction;       // 벽충돌 시 이벤트
    public event Action<bool, float> OnFall;            // 떨어질 때 이벤트

    [SerializeField] private bool isSlide = false;      // 슬라이드 상태인지
    [SerializeField] private int slideCheckCount = 10;  // 슬라이드 인식 보정 프레임
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
            // 중력 처리
            if (gravityModifier > 0f)
            {
                // 하강 시에는 gravityModifier 적용
                if (velocity.y < 0)
                {
                    velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
                }
                // 상승 시 점프 속력 감소 (gravityModifier와 무관하게)
                else
                {
                    velocity += Physics2D.gravity * Time.deltaTime;
                }
            }
            // 역중력도 가능은 함 (게임에선 안쓰는중)
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

                    // 경사면 방향 계산
                    Vector2 g = Physics2D.gravity.normalized;
                    Vector2 slideDir = g - Vector2.Dot(g, currentNormal) * currentNormal;
                    if (slideDir.sqrMagnitude < minMagnitudeSlideDir) slideDir = g;
                    slideDir.Normalize();

                    // 기존 속도 투영 + 중력 가속 추가
                    float speedAlongSlide = Vector2.Dot(velocity, slideDir);
                    Vector2 velocityAlongSlide = slideDir * speedAlongSlide;
                    Vector2 gravityAlongSlide = Vector2.Dot(Physics2D.gravity * gravityModifier, slideDir) * slideDir;

                    velocity = velocityAlongSlide + gravityAlongSlide * Time.fixedDeltaTime;

                    // 경사 탈 때 좌우반전 적용
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


            // y 축 이동일 때만 추락 데이터 액션 실행
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
