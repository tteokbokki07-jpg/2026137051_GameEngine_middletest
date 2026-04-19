using Unity.Cinemachine;
using UnityEngine;

public class CinemachineSmooth : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    private CinemachineFollow follow;

    [Header("Offset")]
    [SerializeField] private Vector3 defaultOffset = new Vector3(0f, 0.4f, -10f);
    [SerializeField] private Vector3 LeftmoveOffset = new Vector3(1f, 0.4f, -10f);
    [SerializeField] private Vector3 RightmoveOffset = new Vector3(-1f, 0.4f, -10f);

    [Header("Smooth")]
    [SerializeField] private float smoothSpeed = 5f;

    private Vector3 velocity;

    void Awake()
    {
        follow = cam.GetComponent<CinemachineFollow>();
    }

    void Update()
    {
        if (follow == null) return;

        Vector3 targetOffset = defaultOffset;

        // 좌/우 화살표 입력에 따라 목표 오프셋 결정
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            targetOffset = LeftmoveOffset;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            targetOffset = RightmoveOffset;
        }
        else
        {
            targetOffset = defaultOffset;
        }

        follow.FollowOffset = Vector3.SmoothDamp(
            follow.FollowOffset,
            targetOffset,
            ref velocity,
            1f / smoothSpeed
        );
    }
}