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
    [Header("Item")]
    public float itemMovevalue = 1.5f; //적용할 배율(1.5 : 50%)
    public bool itemMove = false;    //아이템 이동속도 배율 적용
    public ParticleSystem MoveP;
    public ParticleSystem JumpP;
    private float originalitemMoveSpeed = 0f;
    private bool itemMoveBoosted = false;

    public float itemJumpvalue = 1.25f; //적용할 배율
    public bool itemJump = false;    //아이템 점프력 배율 적용
    private float originalitemJump = 0f;
    private bool itemJumpBoosted = false;
    public bool itemSheld = false;    //아이템 무적
    public GameObject Sheldobj;
    
    public Transform spawnpoint;
    public Transform checkpoint;
    public Animator animator;
    private PlayerController pc;
    private Rigidbody2D rb;
    private Dash ds;
    private bool isGrounded;
    private float moveInput;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // 스프라이트의 flipX 상태를 기준으로 초기 방향 설정
        facingRight = spriteRenderer == null ? true : !spriteRenderer.flipX;
        Sheldobj.SetActive(false);
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

        ds = GetComponent<Dash>();
        if (itemMove == false && ds.isBoost == false && moveSpeed >= 3.56)
        {
            moveSpeed = 3.55f;
        }
        if (moveSpeed <= 3.54)
        {
            moveSpeed = 3.55f;
        }
        if (jumpForce <= 4.49)
        {
            jumpForce = 4.55f;
        }
        if(moveSpeed == 0 && jumpForce == 0)
        {
            moveSpeed = 3.55f;
            jumpForce = 4.55f;
        }
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
        if (collision.CompareTag("Finish") && itemMission >= missionCount)
        {
            collision.GetComponent<Goalpoint>().MoveToNextLevel();
        }

        if (collision.CompareTag("Item_Mission"))
        {
            Debug.Log("Item_Mission");
            itemMission++;
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Enemy") && !itemSheld)
        {
            Vector3 newPos = spawnpoint.position;
            newPos.z = transform.position.z; // z값은 현재 플레이어의 z값 유지
            transform.position = newPos;
            ResetJump();
            ResetSpeed();
            CancelInvoke(nameof(ResetJump));
            CancelInvoke(nameof(ResetSpeed));
            return;
        }
        else if (collision.CompareTag("Enemy") && itemSheld)
        {
            itemSheld = false;
            Sheldobj.SetActive(false);
        }
        // Respawn: 플레이어를 spawnpoint로 이동
        if (collision.CompareTag("Respawn") && !itemSheld)
        {
            Vector3 newPos = spawnpoint.position;
            newPos.z = transform.position.z; // z값은 현재 플레이어의 z값 유지
            transform.position = newPos;
            ResetJump();
            ResetSpeed();
            CancelInvoke(nameof(ResetJump));
            CancelInvoke(nameof(ResetSpeed));
            return;
        }
        else if (collision.CompareTag("Respawn") && itemSheld)
        {
            itemSheld = false;
            Sheldobj.SetActive(false);
        }

        // Checkpoint: spawnpoint를 체크포인트 위치로 이동
        if (collision.CompareTag("Checkpoint"))
        {
            checkpoint = collision.transform;
            Vector3 newPos = checkpoint.position;
            newPos.z = spawnpoint.position.z; // spawnpoint의 기존 z값 유지
            spawnpoint.position = newPos;
            return;
        }

        if (collision.CompareTag("Item_Sheld"))
        {
            Debug.Log("Item_Sheld");
            itemSheld = true;
            Sheldobj.SetActive(true);
        }

        if (collision.CompareTag("Item_Speed"))
        {
            Debug.Log("Item_Speed");
            itemMove = true;
            // 이동속도 상승 옵션
            if (itemMove && pc != null && !itemMoveBoosted)
            {
                originalitemMoveSpeed = pc.moveSpeed;
                pc.moveSpeed = originalitemMoveSpeed * itemMovevalue;
                itemMoveBoosted = true;
                MoveP.Play();
                Invoke(nameof(ResetSpeed), 10f);
            }
        }

        if (collision.CompareTag("Item_Jump"))
        {
            Debug.Log("Item_Jump");
            itemJump = true;
            if (itemJump && pc != null && !itemJumpBoosted)
            {
                originalitemJump = pc.jumpForce;
                pc.jumpForce = originalitemJump * itemJumpvalue;
                itemJumpBoosted = true;
                JumpP.Play();
                Invoke(nameof(ResetJump), 10f);
            }
        }

    }
    void ResetSpeed()
    {
        itemMove = false;
        itemMoveBoosted = false;
        pc.moveSpeed = originalitemMoveSpeed;
        MoveP.Stop();
    }
    void ResetJump()
    {
        itemJump = false;
        itemJumpBoosted = false;
        pc.jumpForce = originalitemJump;
        JumpP.Stop();
    }
}

