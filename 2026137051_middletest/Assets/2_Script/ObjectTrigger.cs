using System;
using UnityEngine;

public class ObjectTrigger : MonoBehaviour
{
    public Transform spawnpoint;
    public Transform checkpoint;
    [Header("Enemy Push")]
    public float enemyPushForce = 5f; // 적에게 밀려날 때 적용할 임펄스 크기
    [Header("Item")]
    public bool itemMove = false;    //아이템 이동속도 배율 적용
    public float itemMovevalue = 1.5f; //적용할 배율(1.5 : 50%)
    private float originalitemMoveSpeed = 0f;
    private bool itemMoveBoosted = false;

    public bool itemJump = false;    //아이템 점프력 배율 적용
    public float itemJumpvalue = 1.25f; //적용할 배율
    public bool itemSheld = false;    //아이템 무적


    private Rigidbody2D rb;
    private PlayerController pc;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
    }

    private bool HasTag(Collider2D other, string tag) // 다른 콜라이더가 특정 태그를 가지고 있는지 확인하는 헬퍼 메서드
    {
        if (other == null) return false;
        if (other.CompareTag(tag)) return true;
        if (other.transform.root != null && other.transform.root.CompareTag(tag)) return true;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // Enemy: 적이 닿으면 플레이어(this 오브젝트)를 적으로부터 반대 방향으로 밀어냄
        if (HasTag(other, "Enemy"))
        {
            Debug.Log("Enemy감지");
        }

        // Barrier: 플레이어(이 스크립트가 붙은 오브젝트)를 spawnpoint로 이동
        if (HasTag(other, "Barrier"))
        {
            if (spawnpoint == null) return;
            Vector3 newPos = spawnpoint.position;
            newPos.z = transform.position.z; // z값은 현재 플레이어의 z값 유지
            transform.position = newPos;
            transform.rotation = spawnpoint.rotation;
            return;
        }

        // Checkpoint: spawnpoint를 체크포인트 위치로 이동
        if (HasTag(other, "Checkpoint"))
        {
            checkpoint = other.transform;
            if (spawnpoint != null)
            {
                Vector3 newPos = checkpoint.position;
                newPos.z = spawnpoint.position.z; // spawnpoint의 기존 z값 유지
                spawnpoint.position = newPos;
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
        }

        if (HasTag(other, "Item_Sheld"))
        {
            Debug.Log("Item_Sheld");
        }

        if (HasTag(other, "Item_Speed"))
        {
            Debug.Log("Item_Speed");
            itemMove = true;
            // 이동속도 상승 옵션
            if (itemMove && pc != null && !itemMoveBoosted)
            {
                originalitemMoveSpeed = pc.moveSpeed;
                pc.moveSpeed = originalitemMoveSpeed * itemMovevalue;
                itemMoveBoosted = true;
                return;
                //Invoke(nameof(RestoreMoveSpeed), 5f); // 5초 후에 이동속도 복구
            }
        }


        if (HasTag(other, "Item_Jump"))
        {
            Debug.Log("Item_Jump");
        }

        if (HasTag(other, "Item_Mission"))
        {
            Debug.Log("Item_Mission");
        }

    }

    //private object RestoreMoveSpeed()
    //{
    //    if (itemMoveBoosted && pc != null)
    //    {
    //        pc.moveSpeed = originalitemMoveSpeed;
    //        itemMoveBoosted = false;
    //    }
    //}
}
