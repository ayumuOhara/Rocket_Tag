using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    GameObject player;
    Transform playerTransform;                                    // 注視対象プレイヤー
    [SerializeField] private CameraController refCamera; 　       // カメラの水平回転を参照する用
    SetPlayerBool setPlayerBool;

    [SerializeField] private float distance = 5.0f;               // 注視対象プレイヤーからカメラを離す距離
    [SerializeField] private float verticalAngle = 20.0f;         // 垂直回転角度
    [SerializeField] private float minVerticalAngle = 10.0f;      // 垂直回転の最小角度
    [SerializeField] private float maxVerticalAngle = 50.0f;      // 垂直回転の最大角度
    [SerializeField] private Quaternion vRotation;                // カメラの垂直回転(見下ろし回転)
    [SerializeField] public Quaternion hRotation;                 // カメラの水平回転
    [SerializeField] private float turnSpeed = 5.0f;              // 回転速度
    [SerializeField] private Vector3 velocity;                    // 移動方向
    private float moveSpeed = 30.0f;                              // 移動速度
    public bool isShaking = false;                                       // カメラが振動しているか

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.GetComponent<Transform>();
        setPlayerBool = player.GetComponent<SetPlayerBool>();

        // 回転の初期化
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        vRotation = Quaternion.Euler(verticalAngle, 0, 0);  // 初期の垂直回転
        hRotation = Quaternion.identity;                    // 初期の水平回転
        transform.rotation = hRotation * vRotation;         // 合成回転

        // 位置の初期化
        transform.position = playerTransform.position - transform.rotation * Vector3.forward * distance;
    }

    void Update()
    {
        if (isShaking == false)
        {
            RotationCamera();
            if (setPlayerBool.isDead == true)
            {
                GetVelocity();
                CameraMovement();
            }
            else
            {
                TrackingTarget();
            }
        }       
    }

    // カメラの回転の制御
    void RotationCamera()
    {
        // 水平回転の更新
        if (Input.GetMouseButton(0))
        {
            hRotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * turnSpeed, 0);

            if (setPlayerBool.isDead == true)
            {
                var minVerticalAngle = -90.0f;         // カメラ独立後の垂直回転の最小角度
                var maxVerticalAngle = 90.0f;      // カメラ独立後の垂直回転の最大角度
                // 垂直回転の更新
                verticalAngle -= Input.GetAxis("Mouse Y") * turnSpeed;
                verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle); // 制限を適用
            }
            else
            {
                // 垂直回転の更新
                verticalAngle -= Input.GetAxis("Mouse Y") * turnSpeed;
                verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle); // 制限を適用
            }            

            vRotation = Quaternion.Euler(verticalAngle, 0, 0);
        }

        // カメラの回転(transform.rotation)の更新
        transform.rotation = hRotation * vRotation;
    }

    // 押下された移動キーに応じてベクトルを取得
    void GetVelocity()
    {
        // WASD入力から、XZ平面(水平な地面)を移動する方向(velocity)を得ます
        velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            velocity.z += 1;
        if (Input.GetKey(KeyCode.A))
            velocity.x -= 1;
        if (Input.GetKey(KeyCode.S))
            velocity.z -= 1;
        if (Input.GetKey(KeyCode.D))
            velocity.x += 1;
        if (Input.GetKey(KeyCode.Space))
            velocity.y += 1;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            velocity.y -= 1;

        // 速度ベクトルの長さを1秒でmoveSpeedだけ進むように調整します
        velocity = velocity.normalized * moveSpeed * Time.deltaTime;
    }

    // 取得したベクトルの方向に移動させる
    // 取得したベクトルの方向に移動&回転させる+ジャンプ処理
    void CameraMovement()
    {
        // いずれかの方向に移動している場合
        if (velocity.magnitude > 0)
        {
            transform.position += refCamera.hRotation * velocity;
        }
    }

    // 指定したオブジェクトを追跡する処理
    void TrackingTarget()
    {
        // カメラの位置(transform.position)の更新
        transform.position = playerTransform.position + new Vector3(0, 1.5f, 0) - transform.rotation * Vector3.forward * distance;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        isShaking = true;

        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = originalPosition + Random.insideUnitSphere * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;

        isShaking = false;
        yield break;
    }
}
