using System.Collections;
using UnityEngine;

public class Skill_AI : MonoBehaviour
{
    public Collider2D ATK_C;
    public Collider2D SkillBox;
    public bool SkillDamage = false;

    public float LandTime = 0;
    public float Land_powerup = 0;
    public float MissileTime = 0;
    public float DashTime = 0;

    public Animator animator;
    [Header("Thresholds")]
    public float LandThreshold = 4f;
    //public float MissileThreshold = 999f;
    public float DashThreshold = 999f;

    [Header("Animator Trigger Names")]
    public string LandTriggerName = "Land";
    public string LandPowerupTriggerName = "Land_P";
    //public string MissileTriggerName = "Missile";
    public string DashTriggerName = "Dash";

    // 내부 상태: 한 번 트리거된 후 재발동을 막기 위한 플래그
    private bool landTriggered = false;
    //private bool missileTriggered = false;
    private bool dashTriggered = false;
    private Move BM;
    private HPBar HPBar;
    private PlayerController pc;
    public float DashSpeed = 7.5f;
    [Tooltip("플레이어에게 가할 넉백 힘(임펄스)")]
    public float knockbackForce = 5f;
    private bool isDashing = false;
    private bool stopDash = false;
    private Transform player;
    [Header("Runtime")]
    public float playerDistance = 0f;
    void Start()
    {
        LandTime = 0;
        MissileTime = 0;
        DashTime = 0;
        BM = GetComponent<Move>();
        pc = GetComponent<PlayerController>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
            playerDistance = Vector2.Distance(player.position, transform.position);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어가 트리거에 머물러 있는 동안 각 영역에 대해 실제 시간(Time.deltaTime)을 더합니다.
        if (!collision.CompareTag("Player"))
            return;

        Vector2 playerPos = collision.transform.position;

        if (ATK_C != null && ATK_C.enabled && BM.canMove)
        {
            LandTime += Time.deltaTime;
            //MissileTime += Time.deltaTime;
            DashTime += Time.deltaTime;
            if (0 < playerDistance || playerDistance < 1f)
            {
                LandTime = LandThreshold;
            }
            if (!landTriggered && LandTime >= LandThreshold && playerDistance <= 1.5f)
            {
                landTriggered = true;
                BM.canMove = false;
                if (Land_powerup != 3)
                {
                    if (animator != null && !string.IsNullOrEmpty(LandTriggerName))
                        animator.SetTrigger(LandTriggerName);
                    StartCoroutine(LandSkillActive());
                    StartCoroutine(OneFrameDamage());
                }
                else if (Land_powerup == 3)
                {
                    Land_powerup = 0;
                    if (animator != null && !string.IsNullOrEmpty(LandPowerupTriggerName))
                        animator.SetTrigger(LandPowerupTriggerName);
                    StartCoroutine(LandSkillActive());
                    StartCoroutine(OneFrameDamage());
                }
            }   
            //if (!missileTriggered && MissileTime >= MissileThreshold)
            //{
            //    if (animator != null && !string.IsNullOrEmpty(MissileTriggerName))
            //        animator.SetTrigger(MissileTriggerName);
            //    missileTriggered = true;
            //    BM.canMove = false;
            //    StartCoroutine(MissileSkillActive());
            //}
            if (!dashTriggered && DashTime >= DashThreshold && playerDistance >= 1.5f)
            {
                if (animator != null && !string.IsNullOrEmpty(DashTriggerName))
                    animator.SetTrigger(DashTriggerName);
                dashTriggered = true;
                BM.canMove = false;
                DashMove();
            }
        }
        if (collision.CompareTag("Player") && SkillDamage)
        {
            // Damage only if the boss's SkillBox collider is actually overlapping the player's position.
            playerPos = collision.transform.position;
            bool skillOverlap = false;
            if (SkillBox != null && SkillBox.enabled)
            {
                var cols = Physics2D.OverlapPointAll(playerPos);
                foreach (var c in cols)
                {
                    if (c == SkillBox)
                    {
                        skillOverlap = true;
                        break;
                    }
                }
            }

            if (skillOverlap)
            {
                Debug.Log("Player hit by skill (SkillBox overlap)!");
                // 플레이어의 child로 Health가 붙어있음. 해당 오브젝트에서 HPBar 컴포넌트를 찾음
                Transform healthT = collision.transform.Find("Health");
                HPBar hpBar = null;
                if (healthT != null)
                {
                    hpBar = healthT.GetComponent<HPBar>();
                }

                // fallback: 자식 중에서 HPBar가 있으면 사용
                if (hpBar == null)
                {
                    hpBar = collision.transform.GetComponentInChildren<HPBar>();
                }

                if (hpBar != null)
                {
                    // 이전 HP를 저장하고 실제로 데미지가 적용되었는지 확인
                    float prevHp = hpBar.currentHP;
                    hpBar.TakeDamage(4f);
                    if (hpBar.currentHP < prevHp)
                    {
                        // 넉백 적용: 플레이어를 보스에서 반대 방향으로 밀어냄
                        Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
                        if (playerRb == null && collision.transform.parent != null)
                            playerRb = collision.transform.parent.GetComponent<Rigidbody2D>();

                        if (playerRb != null)
                        {
                        // 보스의 반대 방향(좌우)으로 밀어내고 약간 위로 띄움
                        float horiz = Mathf.Sign(collision.transform.position.x - transform.position.x);
                        Vector2 knockDir = new Vector2(horiz, 0.5f).normalized;
                        playerRb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
                        }
                    }
                }
                else
                {
                    Debug.Log("No HPBar found on player's Health child or children.");
                }
            }
        }
    }
    IEnumerator OneFrameDamage()
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("데미지시작");
        SkillDamage = true;
        yield return new WaitForSeconds(1.05f);
        Debug.Log("데미지끝");
        SkillDamage = false;
    }
    IEnumerator LandSkillActive()
    {
        yield return new WaitForSeconds(2.0f);
        landTriggered = false;
        LandTime = 0;
        BM.canMove = true;
        Land_powerup ++;
        if (DashTime <= 7)
        {
            DashTime += 1.5f;
            if (DashTime >= 8)
            {
                DashTime = 7.9f;
            }
        }
            
    }
    //IEnumerator MissileSkillActive()
    //{
    //    yield return new WaitForSeconds(2.0f);
    //    missileTriggered = false;
    //    MissileTime = 0;
    //    BM.canMove = true;
    //}
    IEnumerator DashSkillDeactive()
    {
        yield return new WaitForSeconds(1.5f);
        // 종료 시 대시 상태 초기화
        dashTriggered = false;
        SkillDamage = false;
        DashTime = 0f;
        animator.SetBool("Dash_end", false);
        if (BM != null)
            BM.canMove = true;
    }
    public void DashMove()
    {
        if (isDashing) return;
        StartCoroutine(DashSequence());
    }
    private System.Collections.IEnumerator DashSequence()
    {
        // DashStart: preparation
        if (BM != null)
            BM.canMove = false;

        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            yield break;

        Vector2 dir = (player.position - transform.position).normalized;
        // 1회성 회전
        if (dir.x < 0f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (dir.x > 0f)
            transform.localScale = new Vector3(-1f, 1f, 1f);

        // 1) DashStart: 방향 조절만 수행하고 1초 대기
        stopDash = false;
        isDashing = true;
        yield return new WaitForSeconds(1.5f);

        // 2) DashMoving: 1초 대기 후 실제 이동 시작
        var moving = StartCoroutine(DashMoving(dir));
        SkillDamage = true;

        // 제한시간 2초 후 중지 플래그 설정
        yield return new WaitForSeconds(2.0f);
        stopDash = true;

        // wait until moving stops
        yield return moving;

        isDashing = false;

        // Dash 종료
        StartCoroutine(DashSkillDeactive());
        animator.SetBool("Dash_end", true);
    }

    private System.Collections.IEnumerator DashMoving(Vector2 dir)
    {
        bool hitBoundary = false;
        while (!stopDash && !hitBoundary)
        {
            float dt = Time.deltaTime;
            Vector2 move = dir * DashSpeed * dt;
            Vector3 newPos = (Vector2)transform.position + move;
            newPos.y = 0f; // Y 고정
            transform.position = newPos;

            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            foreach (var c in cols)
            {
                if (c != null && c.gameObject != gameObject && c.CompareTag("Boundary"))
                {
                    hitBoundary = true;
                    animator.SetBool("Dash_end", true);
                    StartCoroutine(DashSkillDeactive());
                    break;
                }
            }

            yield return null;
        }
    }
}
