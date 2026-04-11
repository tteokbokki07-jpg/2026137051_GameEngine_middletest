using UnityEngine;

public class ObjectTrigger : MonoBehaviour
{
    public Transform spawnpoint;
    public Transform checkpoint;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private bool HasTag(Collider2D other, string tag)
    {
        if (other == null) return false;
        if (other.CompareTag(tag)) return true;
        if (other.transform.root != null && other.transform.root.CompareTag(tag)) return true;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // Barrier: 플레이어(이 스크립트가 붙은 오브젝트)를 spawnpoint로 이동
        if (HasTag(other, "Barrier"))
        {
            if (spawnpoint == null) return;
            transform.position = spawnpoint.position;
            transform.rotation = spawnpoint.rotation;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            return;
        }

        // Checkpoint: spawnpoint를 체크포인트 위치로 이동
        if (HasTag(other, "Checkpoint"))
        {
            // 우선 other의 transform을 checkpoint로 사용하고 spawnpoint를 갱신
            checkpoint = other.transform;
            if (spawnpoint != null)
            {
                spawnpoint.position = checkpoint.position;
                spawnpoint.rotation = checkpoint.rotation;
            }
            else
            {
                // spawnpoint가 비어있으면 checkpoint를 spawnpoint로 지정
                spawnpoint = checkpoint;
            }
            return;
        }

        // Finish: 디버그 로그 출력
        if (HasTag(other, "Finish"))
        {
            Debug.Log("Finish reached");
            return;
        }
    }
}
