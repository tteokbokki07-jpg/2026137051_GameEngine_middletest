using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;

    public Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;

    // 정점 진입 감지용 이전 상태
    private bool prevIsJumpTop = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // 스프라이트의 flipX 상태를 기준으로 초기 방향 설정
        facingRight = spriteRenderer == null ? true : !spriteRenderer.flipX;
    }
    private void SetFacingRight(bool right)
    {
        facingRight = right;
        spriteRenderer.flipX = !right;
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // 좌우 이동에 따라 스프라이트 플립
        float horizontal = rb.linearVelocity.x;
        if (Mathf.Abs(horizontal) > 0.01f)
        {
            if (horizontal < 0 && !facingRight)
            {
                SetFacingRight(true);
            }
            else if (horizontal > 0 && facingRight)
            {
                SetFacingRight(false);
            }
        }

        // 이동상태 판단 : 실제 속도 기준 판정
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.01f && isGrounded;
        animator.SetBool("Move", isMoving);

        // 점프상태 판단 (상승 / 정점 / 하강 분리)
        const float vertThreshold = 0.01f;
        float vertical = rb.linearVelocity.y;
        bool isJumpUp = !isGrounded && vertical > vertThreshold;                 // 상승 중
        bool isJumptop = !isGrounded && Mathf.Abs(vertical) <= vertThreshold;   // 정점(최고점)
        bool isJumpDown = !isGrounded && vertical < -vertThreshold;              // 하강 중
        animator.SetBool("Jump_up", isJumpUp);
        animator.SetBool("Jump_top", isJumptop);
        animator.SetBool("Jump_down", isJumpDown);
        // 정점에 진입했을 때 한 번만 로그 출력

        if (isJumptop && !prevIsJumpTop)
        {
            Debug.Log("점프 정점 진입");
        }
        prevIsJumpTop = isJumptop; ;
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

}

