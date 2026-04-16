using System.Security;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    [Header("clearmission")]
    public int itemMission = 0;
    public int missionCount = 0;

    public Transform spawnpoint;
    public Transform checkpoint;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;

    public Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;

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
        if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);

        // 이동상태 판단 : 실제 속도 기준 판정
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.01f && isGrounded;
        animator.SetBool("Move", isMoving);

        // 점프상태 판단 (상승 / 정점 / 하강 분리)
        const float vertThreshold = 0.01f;
        float vertical = rb.linearVelocity.y;
        bool isJumpUp = !isGrounded && vertical > vertThreshold;                 // 상승 중
        bool isJumpDown = !isGrounded && vertical < -vertThreshold;              // 하강 중
        animator.SetBool("Jump_up", isJumpUp);
        animator.SetBool("Jump_down", isJumpDown);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Finish") && itemMission >= missionCount)
        {
            collision.GetComponent<Goalpoint>().MoveToNextLevel();
        }

        if (collision.CompareTag("Item_Mission"))
        {
            Debug.Log("Item_Mission");
            itemMission++;
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Enemy"))
        {
            Vector3 newPos = spawnpoint.position;
            newPos.z = transform.position.z; // z값은 현재 플레이어의 z값 유지
            transform.position = newPos;
            transform.rotation = spawnpoint.rotation;
            return;
        }
        // Barrier: 플레이어를 spawnpoint로 이동
        if (collision.CompareTag("Respawn"))
        {
            //if (spawnpoint == null) return;
            Vector3 newPos = spawnpoint.position;
            newPos.z = transform.position.z; // z값은 현재 플레이어의 z값 유지
            transform.position = newPos;
            transform.rotation = spawnpoint.rotation;
            return;
        }
        // Checkpoint: spawnpoint를 체크포인트 위치로 이동
        if (collision.CompareTag("Checkpoint"))
        {
            checkpoint = collision.transform;
            if (spawnpoint != null)
            {
                Vector3 newPos = checkpoint.position;
                newPos.z = spawnpoint.position.z; // spawnpoint의 기존 z값 유지
                spawnpoint.position = newPos;
                spawnpoint.rotation = checkpoint.rotation;
                return;
            }
        }
    }
}

