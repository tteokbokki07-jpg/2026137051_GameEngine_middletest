using UnityEngine;

public class Sheld : MonoBehaviour
{
    public float speed = 180f; // 초당 회전 각도

    void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}