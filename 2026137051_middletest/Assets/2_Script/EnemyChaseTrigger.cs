using UnityEngine;

public class EnemyChaseTrigger : MonoBehaviour
{
    [Header("Trigger")]
    public Transform chaseTriggerTF;
    public Transform slowchaseTriggerTF;
    public GameObject ChaceEnemy;
    public GameObject slowChaceEnemy;
    public bool isChace = false;
    public bool isSlowChace = false;

    private bool IsFromTrigger(Collider2D col, Transform trigger)
    {
        if (col == null || trigger == null) return false;
        var t = col.transform;
        if (t == trigger) return true;
        return t.IsChildOf(trigger);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        // 일반 추적 적: chaseTriggerTF에 의한 진입 처리
        if (IsFromTrigger(collision, chaseTriggerTF) && ChaceEnemy != null)
        {
            isChace = true;
            ChaceEnemy.SetActive(true);

            Vector3 target = chaseTriggerTF.position;
            Vector3 enemyPos = ChaceEnemy.transform.position;
            enemyPos.x = target.x;
            enemyPos.y = target.y;
            ChaceEnemy.transform.position = enemyPos;

            var rb = ChaceEnemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        // 느리게 추적하는 적: slowchaseTriggerTF에 의한 진입 처리
        if (IsFromTrigger(collision, slowchaseTriggerTF) && slowChaceEnemy != null)
        {
            isSlowChace = true;
            slowChaceEnemy.SetActive(true);

            Vector3 targetS = slowchaseTriggerTF.position;
            Vector3 slowPos = slowChaceEnemy.transform.position;
            slowPos.x = targetS.x;
            slowPos.y = targetS.y;
            slowChaceEnemy.transform.position = slowPos;

            var rbSlow = slowChaceEnemy.GetComponent<Rigidbody2D>();
            if (rbSlow != null)
            {
                rbSlow.linearVelocity = Vector2.zero;
                rbSlow.angularVelocity = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null) return;

        // chaseTriggerTF에서 나갔을 때 해당 적 비활성화
        if (IsFromTrigger(collision, chaseTriggerTF))
        {
            isChace = false;
            if (ChaceEnemy != null) ChaceEnemy.SetActive(false);
        }

        // slowchaseTriggerTF에서 나갔을 때 해당 적 비활성화
        if (IsFromTrigger(collision, slowchaseTriggerTF))
        {
            isSlowChace = false;
            if (slowChaceEnemy != null) slowChaceEnemy.SetActive(false);
        }
    }
}
