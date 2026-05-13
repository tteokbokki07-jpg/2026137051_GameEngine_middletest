using UnityEngine;

public class Skill_Attack : MonoBehaviour
{
    public Animator animator;
    [Tooltip("쿨타임(초)")]
    public float cooldown = 1.5f;

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
                lastAttackTime = Time.time;
            }
        }
    }
}
