using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    public bool groundDashSheld = false;    //바닥대쉬 무적
    public GameObject Sheldobj;
    public Animator SheldAnimator;
    Coroutine shieldCoroutine;

    private bool isInvincible;

    public Transform spawnpoint;
    public Transform checkpoint;
    public Animator animator;
    public bool canMove = true; //조작 여부

    private PlayerController pc;
    private Rigidbody2D rb;
    private Dash ds;
    private bool isGrounded;
    private float moveInput;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    float score;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // 스프라이트의 flipX 상태를 기준으로 초기 방향 설정
        facingRight = spriteRenderer == null ? true : !spriteRenderer.flipX;
        Sheldobj.SetActive(false);
        score = 0f;
    }
    void Update()
    {
        isInvincible = itemSheld || groundDashSheld; // 무적 상태 판단
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
            Debug.Log(1);
            moveSpeed = 3.55f;
        }
        if (moveSpeed <= 3.54)
        {
            Debug.Log(2);
            moveSpeed = 3.55f;
        }
        if (jumpForce <= 4.49)
        {
            Debug.Log(3);
            jumpForce = 4.55f;
        }
        if(moveSpeed == 0 && jumpForce == 0)
        {
            Debug.Log(4);
            moveSpeed = 3.55f;
            jumpForce = 4.55f;
        }
        if (itemMove == false && ds.isBoost == true && moveSpeed == 3.55)
        {
            moveSpeed = 6.2125f;
        }
        if (itemMove == true && ds.isBoost == false)
        {
            moveSpeed = 4.615f;
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
            SoundManager.instance.PlaySFX(SoundManager.instance.JumpClip, 0.175f,1.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish") && itemMission >= missionCount)
        {
            collision.GetComponent<Goalpoint>().MoveToNextLevel();
            score += 10f; //점수
            StageResultSaver.SaveStage(SceneManager.GetActiveScene().buildIndex, (int)score);
        }
        if (collision.CompareTag("Item_Mission"))
        {
            itemMission++;
            score += collision.GetComponent<ItemObject>().GetPoint();
            Destroy(collision.gameObject);
            SoundManager.instance.PlaySFX(SoundManager.instance.ItemPowerupClip, 0.8f, 1.5f);
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
        if (collision.CompareTag("Item_Speed"))
        {
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
            SoundManager.instance.PlaySFX(SoundManager.instance.ItemClip, 0.4f, 2f);
            SoundManager.instance.PlaySFX(SoundManager.instance.ItemPowerupClip, 0.4f, 1.5f);
        }
        if (collision.CompareTag("Item_Jump"))
        {
            itemJump = true;
            if (itemJump && pc != null && !itemJumpBoosted)
            {
                originalitemJump = pc.jumpForce;
                pc.jumpForce = originalitemJump * itemJumpvalue;
                itemJumpBoosted = true;
                JumpP.Play();
                Invoke(nameof(ResetJump), 10f);
            }
            SoundManager.instance.PlaySFX(SoundManager.instance.ItemClip, 0.4f, 2f);
            SoundManager.instance.PlaySFX(SoundManager.instance.ItemPowerupClip, 0.4f, 0.7f);
        }
        if (collision.CompareTag("Enemy") && !isInvincible)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.ErrorEnemyClip, 0.2f, 1.0f);
        }
        else if (collision.CompareTag("Enemy") && itemSheld && !groundDashSheld)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.ErrorEnemyClip, 0.2f, 1.0f);
            SoundManager.instance.PlaySFX(SoundManager.instance.ItemClip, 0.4f, 4f);
        }
        if (collision.CompareTag("Respawn"))
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.ErrorEnemyClip, 0.2f, 1.0f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Item_Sheld"))
        {
            if(!itemSheld)
                SoundManager.instance.PlaySFX(SoundManager.instance.ItemClip, 0.4f, 2f);
            itemSheld = true;
            Sheldobj.SetActive(true);

            // 기존 코루틴 취소
            if (shieldCoroutine != null)
            {
                StopCoroutine(shieldCoroutine);
            }
        }
        if (collision.CompareTag("Enemy") && !isInvincible)
        {
            Vector3 newPos = spawnpoint.position;
            newPos.z = transform.position.z; // z값은 현재 플레이어의 z값 유지
            transform.position = newPos;
            ResetJump();
            ResetSpeed();
            CancelInvoke(nameof(ResetJump));
            CancelInvoke(nameof(ResetSpeed));
            score -= 20;
            return;
        }
        else if (collision.CompareTag("Enemy") && itemSheld && !groundDashSheld)
        {
            StartCoroutine(HideShield());
            SheldAnimator.SetTrigger("Break");
        }
        // Respawn: 플레이어를 spawnpoint로 이동
        if (collision.CompareTag("Respawn"))
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
    IEnumerator HideShield()
    {
        yield return new WaitForSeconds(0.5f);
        itemSheld = false;
        Sheldobj.SetActive(false);
    }
}