using UnityEngine;

public class Skill_Attack : MonoBehaviour
{
    public Animator animator;
    [Tooltip("쿨타임(초)")]
    public float cooldown = 1.5f;

    [Header("Attack")]
    public float attackDamage = 4f;
    public float attackRange = 0.5f;
    public float attackOffset = 0.6f;
    public LayerMask targetLayer = ~0; // all by default
    [Tooltip("플레이어가 보스에게 데미지를 줄 때 회복할 HP 양")]
    public float healOnHit = 0.5f;
    [Header("Overrides")]
    [Tooltip("Assign the boss Collider2D in the inspector to target a specific boss hitbox. If left null, tag-based detection is used.")]
    public Collider2D bossCollider;

    // 마지막 공격 시각
    private float lastAttackTime = -Mathf.Infinity;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // C 키를 누르면 Attack 트리거 실행 (쿨타임 적용)
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Time.time < lastAttackTime + cooldown)
            {
                // 쿨타임 중이라면 무시
                return;
            }
            else
            {
                animator.SetTrigger("Attack");
                // 즉시 공격 판정(간단한 히트박스 방식)
                PerformAttack();
                lastAttackTime = Time.time;
                SoundManager.instance.PlaySFX(SoundManager.instance.DashClip, 0.5f, 1.75f);
                SoundManager.instance.PlaySFX(SoundManager.instance.EnemyStartClip, 0.2f, 1f);

            }
        }
    }

    private void PerformAttack()
    {
        // 공격 방향(플레이어의 로컬 스케일 X값 기준)
        float facing = transform.localScale.x >= 0f ? 1f : -1f;
        Vector2 origin = (Vector2)transform.position + Vector2.right * attackOffset * facing;
        Collider2D[] hits;
        // LayerMask가 기본값이면 모든 레이어 검색, 아니면 지정된 레이어만 검색
        if (targetLayer == (LayerMask)~0)
            hits = Physics2D.OverlapCircleAll(origin, attackRange);
        else
            hits = Physics2D.OverlapCircleAll(origin, attackRange, targetLayer.value);

        Debug.Log($"PerformAttack origin={origin} range={attackRange} hits={hits.Length}");
        foreach (var col in hits)
        {
            if (col == null) continue;
            Debug.Log($" - hit: {col.name} tag={col.tag}");

            // HPBar 컴포넌트가 있는지 확인
            var hp = col.GetComponent<HPBar>();
            if (hp == null)
                hp = col.GetComponentInParent<HPBar>();
            if (hp == null)
                hp = col.GetComponentInChildren<HPBar>();

            if (hp == null)
            {
                Debug.Log("   - no HPBar found on hit collider");
                continue;
            }

            // If a bossCollider is assigned in inspector, require the hit to be related to that collider
            if (bossCollider != null)
            {
                bool related = false;
                if (col == bossCollider) related = true;
                else if (col.transform.IsChildOf(bossCollider.transform)) related = true;
                else if (bossCollider.transform.IsChildOf(col.transform)) related = true;

                if (!related)
                {
                    Debug.Log($"   - skipped (not assigned boss collider). hit={col.name}");
                    continue;
                }
            }
            else
            {
                // fallback: tag-based detection
                bool isBoss = IsAncestorTaggedBoss(col.transform) || IsAncestorTaggedBoss(hp.transform);
                if (!isBoss)
                {
                    Debug.Log($"   - skipped (not Boss). colliderTag={col.tag}, hpObjectTag={hp.gameObject.tag}");
                    continue;
                }
            }

            hp.TakeDamage(attackDamage);
            Debug.Log($"   - applied {attackDamage} damage to {hp.gameObject.name}");

            // 플레이어 회복: 플레이어의 Health 자식 오브젝트의 HPBar를 찾아 회복
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && healOnHit > 0f)
            {
                var playerHealthT = player.transform.Find("Health");
                HPBar playerHp = null;
                if (playerHealthT != null)
                    playerHp = playerHealthT.GetComponent<HPBar>();
                if (playerHp == null)
                    playerHp = player.GetComponentInChildren<HPBar>();
                if (playerHp != null)
                {
                    playerHp.Heal(healOnHit);
                }
            }
        }
    }

    private bool IsAncestorTaggedBoss(Transform t)
    {
        Transform cur = t;
        while (cur != null)
        {
            if (cur.CompareTag("Boss")) return true;
            cur = cur.parent;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        // 공격 범위 시각화
        float facing = transform != null ? (transform.localScale.x >= 0f ? 1f : -1f) : 1f;
        Vector2 origin = (Application.isPlaying && transform != null) ? (Vector2)transform.position + Vector2.right * attackOffset * facing : Vector2.zero;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, attackRange);
    }
}
