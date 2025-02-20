using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    GameObject player;
    Transform playerTransform;                                    // 注視対象プレイヤー
    Transform playerRightHandTransform;
    [SerializeField] private CameraController refCamera; 　       // カメラの水平回転を参照する用
    SetPlayerBool setPlayerBool;
    PlayerMovement playerMovement;

    [SerializeField] private float distance = 2.0f;               // 注視対象プレイヤーからカメラを離す距離
    [SerializeField] private float verticalAngle = 20.0f;         // 垂直回転角度
    [SerializeField] private float minVerticalAngle = 20.0f;      // 垂直回転の最小角度
    [SerializeField] private float maxVerticalAngle = 50.0f;      // 垂直回転の最大角度
    [SerializeField] private Quaternion vRotation;                // カメラの垂直回転(見下ろし回転)
    [SerializeField] public  Quaternion hRotation;                // カメラの水平回転
    [SerializeField] private float turnSpeed = 5.0f;              // 回転速度
    [SerializeField] private Vector3 velocity;                    // 移動方向
    private float moveSpeed = 30.0f;                              // 移動速度
    private float aimMoveSpeed = 2.0f;                              // 移動速度
    private float tmpPlayerMoveSpeed;                             // デフォルトプレイヤー移動速度
    private float aimDis = 3.2f;                                  //  ADS中のカメラ距離
    private float tmpDis = 5.0f;                                  //  デフォルトのカメラ位置
    private float minAimVerticalAngle = -20f;                     //  ADS中のカメラ距離
    private float tmpMinverticalAngle = 20f;                      //  デフォルトのカメラの最低角度
    public bool isShaking = false;                                //  カメラが振動しているか
    public bool isAiming = false;                                 //  エイム中か

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        setPlayerBool = player.GetComponent<SetPlayerBool>();
        playerTransform = player.GetComponent<Transform>();
        playerRightHandTransform = GameObject.Find("RightHand").GetComponent<Transform>();
        playerMovement = player.GetComponent<PlayerMovement>();

        // 回転の初期化
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        vRotation = Quaternion.Euler(verticalAngle, 0, 0);  // 初期の垂直回転
        hRotation = Quaternion.identity;                    // 初期の水平回転
        transform.rotation = hRotation * vRotation;         // 合成回転

        tmpPlayerMoveSpeed = playerMovement.GetMoveSpeed();
        // 位置の初期化
        transform.position = playerTransform.position - transform.rotation * Vector3.forward * distance;

        // マウスカーソルを画面内の範囲のみ動かせるようにする
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        CursorVisible();

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

    void CursorVisible()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }

    }

    // カメラの回転の制御
    void RotationCamera()
    {
        // 水平回転の更新
        if (!Input.GetKey(KeyCode.LeftAlt))
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
        if(!isAiming)
        {
            distance = tmpDis;
           // playerMovement.SetMoveSpeed(tmpPlayerMoveSpeed);
            // カメラの位置(transform.position)の更新
            transform.position = playerTransform.position + new Vector3(0, 1.0f, 0) - transform.rotation * Vector3.forward * distance;
        }
        else
        {
            distance = aimDis;
            minVerticalAngle = minAimVerticalAngle;
           // playerMovement.SetMoveSpeed(aimMoveSpeed);
            //transform.position = playerTransform.position + new Vector3(-1.02f, 1.74f, 2.75f) - transform.rotation * Vector3.forward * distance;
            //transform.position = playerTransform.position + new Vector3(playerTransform.position.x + 1, playerTransform.position.y, 0f) - transform.rotation * Vector3.forward * distance;
            //transform.position = Vector3.Lerp(transform.position, playerTransform.position + new Vector3(playerRightHandTransform.localPosition.x + 0.2f, playerRightHandTransform.localPosition.y + 0.8f, 0f) - transform.rotation * Vector3.forward * distance, 20 * Time.deltaTime); 
            transform.position = playerTransform.position + new Vector3(playerRightHandTransform.localPosition.x + 0.2f, playerRightHandTransform.localPosition.y + 0.8f, 0f) - transform.rotation * Vector3.forward * distance; 
        }
        
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
        Debug.Log("振動終了");
        isShaking = false;
        transform.position = originalPosition;
        Debug.Log($"isShaking：{isShaking}");
        yield break;
    }
}
