using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;          // 注視対象プレイヤー

    [SerializeField] private float distance = 5.0f;    // 注視対象プレイヤーからカメラを離す距離
    [SerializeField] private float verticalAngle = 20.0f; // 垂直回転角度
    [SerializeField] private float minVerticalAngle = 10.0f; // 垂直回転の最小角度
    [SerializeField] private float maxVerticalAngle = 50.0f; // 垂直回転の最大角度
    [SerializeField] private Quaternion vRotation;      // カメラの垂直回転(見下ろし回転)
    [SerializeField] public Quaternion hRotation;      // カメラの水平回転
    [SerializeField] private float turnSpeed = 5.0f;   // 回転速度

    private void Awake()
    {
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        // 回転の初期化
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        vRotation = Quaternion.Euler(verticalAngle, 0, 0);  // 初期の垂直回転
        hRotation = Quaternion.identity;                   // 初期の水平回転
        transform.rotation = hRotation * vRotation;        // 合成回転

        // 位置の初期化
        transform.position = player.position - transform.rotation * Vector3.forward * distance;
    }

    void Update()
    {
        RotationCamera();
        TrackingTarget();
    }

    // カメラの回転の制御
    void RotationCamera()
    {
        // 水平回転の更新
        if (Input.GetMouseButton(0))
        {
            hRotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * turnSpeed, 0);

            // 垂直回転の更新
            verticalAngle -= Input.GetAxis("Mouse Y") * turnSpeed;
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle); // 制限を適用

            vRotation = Quaternion.Euler(verticalAngle, 0, 0);
        }

        // カメラの回転(transform.rotation)の更新
        transform.rotation = hRotation * vRotation;
    }

    // 指定したオブジェクトを追跡する処理
    void TrackingTarget()
    {
        // カメラの位置(transform.position)の更新
        transform.position = player.position + new Vector3(0, 1.5f, 0) - transform.rotation * Vector3.forward * distance;
    }
}
