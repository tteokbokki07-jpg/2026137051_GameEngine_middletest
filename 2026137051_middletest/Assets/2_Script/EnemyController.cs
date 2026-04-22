using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    public bool isMovingRight = true;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (isMovingRight)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(-0.7f, 0.7f, 0.7f);
        }
        else if (!isMovingRight)
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary"))
        {
            isMovingRight = !isMovingRight;
        }
    }
}
