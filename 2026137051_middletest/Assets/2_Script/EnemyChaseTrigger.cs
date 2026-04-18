using UnityEngine;

public class EnemyChaseTrigger : MonoBehaviour
{
    [Header("Trigger")]
    public Transform chaseTriggerTF;
    public GameObject ChaceEnemy;
    public bool isChace = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("ChaseTrigger")) return;
        if (ChaceEnemy == null || chaseTriggerTF == null) return;

        isChace = true;
        ChaceEnemy.SetActive(true);

        // ChaceEnemy를 chaseTriggerTF 위치로 이동 (z값은 기존 ChaceEnemy의 z 유지)
        Vector3 target = chaseTriggerTF.position;
        Vector3 enemyPos = ChaceEnemy.transform.position;
        enemyPos.x = target.x;
        enemyPos.y = target.y;
        ChaceEnemy.transform.position = enemyPos;
        return;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("ChaseTrigger")) return;
        isChace = false;
        if (ChaceEnemy != null) ChaceEnemy.SetActive(false);
        return;
    }
}
