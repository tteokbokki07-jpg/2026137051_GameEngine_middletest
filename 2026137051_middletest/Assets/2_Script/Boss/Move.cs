using UnityEngine;

public class Move : MonoBehaviour
{
    public bool canMove = true;
    public float moveSpeed = 3f;
    public Transform Target;
    public Animator animator;
    [Tooltip("거리 로그 출력 시 허용 오차")]
    public float distanceEpsilon = 0.05f;
    [Tooltip("거리 계산 기준으로 사용할 콜라이더")]
    public Collider2D distanceReferenceCollider;

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
        {
            if (animator != null)
                animator.SetBool("Move", false);
            return;
        }

        // 목표 위치를 구하되 Y 좌표는 0으로 고정
        Vector3 targetPos = new Vector3(Target.position.x, 0f, Target.position.z);
        // 현재 위치에서 목표 방향으로 이동 (프레임 독립적)
        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        newPos.y = 0f; // Y값 고정

        // 이동 전후의 X 차이로 이동 여부 및 방향 판단
        float deltaX = newPos.x - transform.position.x;
        bool isMoving = Mathf.Abs(deltaX) > 0.001f;

        transform.position = newPos;

        // 애니메이터에 이동 상태 전달
        if (animator != null)
            animator.SetBool("Move", isMoving);

        // 좌우 이동에 따라 이미지 X 플립(있다면 SpriteRenderer 사용, 없으면 로컬 스케일 사용)
        if (isMoving)
        {
            if (deltaX < 0f)
                transform.localScale = new Vector3(1f, 1f, 1f);
            else if (deltaX > 0f)
                transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        // 플레이어(타겟)과의 거리 계산 및 디버그 로그 (거리 약 1일 때)
        Vector3 referencePoint;
        if (distanceReferenceCollider != null)
        {
            // Collider2D.ClosestPoint 반환값은 Vector2
            Vector2 cp = distanceReferenceCollider.ClosestPoint(Target.position);
            referencePoint = new Vector3(cp.x, cp.y, transform.position.z);
        }
        else
        {
            referencePoint = transform.position;
        }

        float distance = Vector3.Distance(referencePoint, Target.position);
        if (Mathf.Abs(distance - 1f) <= distanceEpsilon)
        {
            Debug.Log("거리1");
        }
    }
}
