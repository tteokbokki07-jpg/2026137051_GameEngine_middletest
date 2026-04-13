using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    [Header("parimeter")]
    public Animator animator;
    public string sprintActionName = "Sprint";
    public string dashTriggerName = "Dash";
    public string dashBoolName = "Dash_hold";

    [Header("Afterimage")]
    public GameObject afterimagePrefab;      // 있으면 프리팹 사용, 없으면 런타임으로 SpriteRenderer 생성
    public float afterSpawnInterval = 0.05f; // 잔상 생성 주기
    public float LifeTime = 0.5f;       // 잔상 생존 시간
    public Color afterColor = new Color(0.5f, 0.5f, 1f, 0.7f);
    public int sortingOrderOffset = -1;

    [Header("Move Speed Boost (옵션)")]
    public bool boostMove = false;    //대시중 이동속도 배율 적용
    public float boostMovevalue = 1.5f; //적용할 배율(1.5 : 50%)

    private PlayerInput playerInput;
    private InputAction sprintAction;
    private PlayerController pc;
    private Rigidbody2D rb;
    private SpriteRenderer playerSR;

    private Coroutine afterCoroutine;
    private readonly List<GameObject> spawnedGhosts = new List<GameObject>();
    private bool airJumpUsed = false;

    // 이동속도 복구 관련
    private float originalMoveSpeed = 0f;
    private bool moveSpeedBoosted = false;

    private void OnEnable() //코드 이해 필요
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null) return;
        sprintAction = playerInput.actions.FindAction(sprintActionName);
        if (sprintAction == null) return;
        sprintAction.started += OnSprintStarted;
        sprintAction.canceled += OnSprintCanceled;
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        playerSR = GetComponentInChildren<SpriteRenderer>();
    }// OnEnable에서 InputAction을 찾아 이벤트 구독, PlayerController와 Rigidbody2D, SpriteRenderer 참조 가져오기

    private void OnDisable() //코드 이해 필요
    {
        if (sprintAction != null)
        {
            sprintAction.started -= OnSprintStarted;
            sprintAction.canceled -= OnSprintCanceled;
        }
        StopAfterimageCoroutine();
        RestoreMoveSpeedIfNeeded();
    } // OnDisable에서 이벤트 구독 해제, 잔상 코루틴 중지, 이동속도 복구

    private void Update()
    {
        // 착지 시 공중 점프 사용 플래그 리셋 및 속도 복구
        if (pc != null && pc.groundCheck != null)
        {
            bool grounded = Physics2D.OverlapCircle(pc.groundCheck.position, 0.2f, pc.groundLayer);
            if (grounded)
            {
                airJumpUsed = false;
                RestoreMoveSpeedIfNeeded();
            }
        }
    }

    private void OnSprintStarted(InputAction.CallbackContext ctx)
    {
        // 애니메이터 호출은 유지
        if (animator != null)
        {
            animator.SetTrigger(dashTriggerName);
            animator.SetBool(dashBoolName, true);
        }
        if (pc == null || rb == null) return;  //안전 검사 코드
        // 착지 상태 판단
        bool grounded = false;
        if (pc.groundCheck != null)
            grounded = Physics2D.OverlapCircle(pc.groundCheck.position, 0.2f, pc.groundLayer);
        // 바닥에서는 대쉬 불가
        if (grounded) return;
        // 공중에서 한 번만 동작
        if (!airJumpUsed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * pc.jumpForce, ForceMode2D.Impulse);
            airJumpUsed = true;
            // 이동속도 상승 옵션
            if (boostMove && pc != null && !moveSpeedBoosted)
            {
                originalMoveSpeed = pc.moveSpeed;
                pc.moveSpeed = originalMoveSpeed * boostMovevalue;
                moveSpeedBoosted = true;
            }

            // 대시 사용 후 착지할 때까지 잔상 생성
            if (afterCoroutine != null) StopCoroutine(afterCoroutine);
            afterCoroutine = StartCoroutine(AfterimageUntilLandCoroutine());
        }
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        if (animator != null)
            animator.SetBool(dashBoolName, false);

    }

    // 대시 사용 직후 착지할 때까지 잔상 생성
    private IEnumerator AfterimageUntilLandCoroutine()
    {
        float timer = 0f;

        while (true)
        {
            // 매 프레임 착지 여부 확인
            bool grounded = false;
            if (pc != null && pc.groundCheck != null)
                grounded = Physics2D.OverlapCircle(pc.groundCheck.position, 0.2f, pc.groundLayer);
            if (grounded)
            {
                // 땅에 닿으면 모든 잔상 제거하고 종료
                RestoreMoveSpeedIfNeeded();
                break;
            }
            // 공중일 때만 주기적으로 잔상 생성
            timer += Time.deltaTime;
            if (timer >= afterSpawnInterval)
            {
                timer = 0f;
                SpawnAfterimageOnce();
            }
            yield return null;
        }
        afterCoroutine = null;
    }

    private void StopAfterimageCoroutine()
    {
        if (afterCoroutine != null)
        {
            StopCoroutine(afterCoroutine);
            afterCoroutine = null;
        }
    }

    private void SpawnAfterimageOnce()
    {
        if (playerSR == null || playerSR.sprite == null) return;
        GameObject ghost;
        if (afterimagePrefab != null)
        {
            ghost = Instantiate(afterimagePrefab, playerSR.transform.parent);
            ghost.transform.localPosition = playerSR.transform.localPosition;
            ghost.transform.localRotation = playerSR.transform.localRotation;
            ghost.transform.localScale = playerSR.transform.localScale;

            // 프리팹 SpriteRenderer값 적용
            var srs = ghost.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var gs in srs)
            {
                gs.sprite = playerSR.sprite;
                gs.flipX = playerSR.flipX;
                gs.flipY = playerSR.flipY;
                gs.sortingLayerID = playerSR.sortingLayerID;
                gs.sortingOrder = playerSR.sortingOrder + sortingOrderOffset;
                gs.sharedMaterial = playerSR.sharedMaterial;
                gs.color = afterColor;
            }
        }
        else
        {
            // 런타임으로 SpriteRenderer만 있는 오브젝트 생성
            ghost = new GameObject("Afterimage_" + playerSR.sprite.name);
            ghost.transform.SetParent(playerSR.transform.parent, false);
            ghost.transform.localPosition = playerSR.transform.localPosition;
            ghost.transform.localRotation = playerSR.transform.localRotation;
            ghost.transform.localScale = playerSR.transform.localScale;

            var gs = ghost.AddComponent<SpriteRenderer>();
            gs.sprite = playerSR.sprite;
            gs.flipX = playerSR.flipX;
            gs.flipY = playerSR.flipY;
            gs.sortingLayerID = playerSR.sortingLayerID;
            gs.sortingOrder = playerSR.sortingOrder + sortingOrderOffset;
            gs.sharedMaterial = playerSR.sharedMaterial;
            gs.color = afterColor;
        }
        spawnedGhosts.Add(ghost);
        StartCoroutine(FadeAndDestroy(ghost, LifeTime));
    } // 플레이어의 현재 스프라이트를 기반으로 잔상 생성, 프리팹이 있으면 프리팹 사용, 없으면 런타임으로 SpriteRenderer만 있는 오브젝트 생성.
      // 생성된 잔상은 리스트에 추가되고, FadeAndDestroy 코루틴으로 서서히 사라지면서 제거됨.

    private IEnumerator FadeAndDestroy(GameObject ghost, float duration)
    {
        if (ghost == null) yield break;
        var srs = ghost.GetComponentsInChildren<SpriteRenderer>(true);
        float elapsed = 0f;
        var startColors = new List<Color>();
        foreach (var sr in srs) startColors.Add(sr.color);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            for (int i = 0; i < srs.Length; i++)
            {
                if (srs[i] != null)
                {
                    Color c = startColors[i];
                    c.a = Mathf.Lerp(c.a, 0f, t);
                    srs[i].color = c;
                }
            }
            yield return null;
        }
        if (ghost != null)
        {
            spawnedGhosts.Remove(ghost);
            Destroy(ghost);
        }
    } // 잔상이 생성된 후 일정 시간 동안 서서히 투명해지면서 사라지는 효과를 주는 코루틴. duration이 끝나면 잔상 오브젝트를 제거.

    private void RestoreMoveSpeedIfNeeded()
    {
        if (moveSpeedBoosted && pc != null)
        {
            pc.moveSpeed = originalMoveSpeed;
            moveSpeedBoosted = false;
        }
    }//플레이어가 공중에서 대시를 사용할 때 잔상을 생성하고, 이동속도를 일시적으로 상승시키는 기능을 구현합니다.
} 
