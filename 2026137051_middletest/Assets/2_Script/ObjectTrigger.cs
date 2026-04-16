using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectTrigger : MonoBehaviour
{

    [Header("Item")]
    public float itemMovevalue = 1.5f; //적용할 배율(1.5 : 50%)
    public bool itemMove = false;    //아이템 이동속도 배율 적용
    private float originalitemMoveSpeed = 0f;
    private bool itemMoveBoosted = false;

    public float itemJumpvalue = 1.25f; //적용할 배율
    public bool itemJump = false;    //아이템 점프력 배율 적용
    private float originalitemJump = 0f;
    private bool itemJumpBoosted = false;
    public bool itemSheld = false;    //아이템 무적

    private Rigidbody2D rb;
    private PlayerController pc;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item_Sheld"))
        {
            Debug.Log("Item_Sheld");
            itemSheld = true;
        }

        if (collision.CompareTag("Item_Speed"))
        {
            Debug.Log("Item_Speed");
            itemMove = true;
            // 이동속도 상승 옵션
            if (itemMove && pc != null && !itemMoveBoosted)
            {
                originalitemMoveSpeed = pc.moveSpeed;
                pc.moveSpeed = originalitemMoveSpeed * itemMovevalue;
                itemMoveBoosted = true;
                Invoke(nameof(ResetSpeed), 10f);
            }
        }

        if (collision.CompareTag("Item_Jump"))
        {
            Debug.Log("Item_Jump");
            itemJump = true;
            if (itemJump &&  pc != null && !itemJumpBoosted)
            {
                originalitemJump = pc.jumpForce;
                pc.jumpForce = originalitemJump * itemJumpvalue;
                itemJumpBoosted = true;
                Invoke(nameof(ResetJump), 10f);
            }
        }

    }

    void ResetSpeed()
    {
        itemMove = false;
        itemMoveBoosted = false;
        pc.moveSpeed = originalitemMoveSpeed;
    }
    void ResetJump()
    {
        itemJump = false;
        itemJumpBoosted = false;
        pc.jumpForce = originalitemJump;
    }
}
