using UnityEngine;

public class EnemyTraceController : MonoBehaviour
{
    public float moveSpeed = .5f;
    public float raycastDistance = .2f;
    public float traceDistance = 2f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Vector2 direction = player.position - transform.position;
        if (direction.magnitude > traceDistance)
            return;

        Vector2 directionNormalized = direction.normalized;
        RaycastHit2D[] his = Physics2D.RaycastAll(transform.position, directionNormalized, raycastDistance);
        Debug.DrawRay(transform.position, directionNormalized * raycastDistance, Color.red);

        foreach(RaycastHit2D rHit in his)
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
}
