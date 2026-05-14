using UnityEngine;

public class EnemyTraceController : MonoBehaviour
{
    public float moveSpeed = .5f;
    public float raycastDistance = .2f;
    public float traceDistance = 2f;

    private Transform player;
    private bool isFleeing = false;
    [Tooltip("플레이어와 가까울 때 도망칠 거리")]
    public float fleeDistance = 5f;
    [Tooltip("도망칠 때 걸리는 시간(초)")]
    public float fleeDuration = 1.0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Vector2 direction = (Vector2)(player.position - transform.position);
        float distance = direction.magnitude;
        // 플레이어와의 거리가 1 이하이면 추격을 멈추고 반대 방향으로 부드럽게 도망감
        if (distance <= 0.675f)
        {
            if (!isFleeing)
            {
                Vector2 away = (-direction).normalized;
                StartCoroutine(FleeCoroutine(away));
            }
            return;
        }


        if (distance > traceDistance)
            return;

        // 플레이어 방향으로 Z축 회전 (스프라이트가 오른쪽을 바라본다고 가정)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Vector2 directionNormalized = direction.normalized;
        RaycastHit2D[] his = Physics2D.RaycastAll(transform.position, directionNormalized, raycastDistance);
        Debug.DrawRay(transform.position, directionNormalized * raycastDistance, Color.red);

        foreach (RaycastHit2D rHit in his)
        {
            if (rHit.collider != null && rHit.collider.CompareTag("Obstacle"))
            {
                Vector3 alternativeDirection = Quaternion.Euler(0f, 0f, -90f) * direction;
                transform.Translate(alternativeDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }
    }

    private System.Collections.IEnumerator FleeCoroutine(Vector2 awayDirection)
    {
        isFleeing = true;
        Vector3 start = transform.position;
        Vector3 target = start + (Vector3)(awayDirection.normalized * fleeDistance);
        float elapsed = 0f;

        while (elapsed < fleeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fleeDuration);
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        // 보간을 마친 후 잠시 플래그 풀기
        isFleeing = false;
    }
}
