using Photon.Pun;
using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    ChangeObjColor changeObjColor;

    [SerializeField] private Vector3 movingVelocity;             // 移動方向
    [SerializeField] private float moveSpeed = 10.0f;            // 移動速度
    [SerializeField] private float defaultMoveSpeed = 10.0f;     // 通常の移動速度
    [SerializeField] private float applySpeed = 0.2f;            // 回転の適用速度
    [SerializeField] private float jumpForce = 20.0f;            // ジャンプ力
    private bool isGround = false;                               // 接地判定
    private float groundLimit = 0.7f;                            // 接地判定のしきい値
    [SerializeField] private CameraController refCamera;      　 // カメラの水平回転を参照する用

    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider _collider;
    [SerializeField] PhysicsMaterial defaultFriction;       // 通常状態の摩擦
    [SerializeField] PhysicsMaterial noneFriction;          // 方向キー入力中の摩擦

    float stunTime = 3.0f;                                  // スタン時間

    void Start()
    {
        refCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CameraController>();
        changeObjColor = GetComponent<ChangeObjColor>();
    }

    public void SetMoveSpeed(float _moveSpeed)
    {
        moveSpeed = _moveSpeed;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    // プレイヤーの速度切り替え
    public void ChangeMoveSpeed(bool hasRocket,float newSpeed = 0)
    {
        if (hasRocket == true)
            SetMoveSpeed(newSpeed);
        else
            SetMoveSpeed(defaultMoveSpeed);
    }    

    // 押下された移動キーに応じてベクトルを取得
    public void GetVelocity()
    {
        movingVelocity = Vector3.zero;
        // GetAxisRawを使って移動する方向を取得
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 movingDirection = new Vector3(x, 0, z);
        // 斜め移動が速くならないようにする
        movingDirection.Normalize();              

        movingVelocity = movingDirection * moveSpeed;
    }

    // 取得したベクトルの方向に移動&回転させる+ジャンプ処理
    public void PlayerMove()
    {
        // いずれかの方向に移動している場合
        if (movingVelocity.magnitude > 0)
        {
            _collider.material = noneFriction;

            // カメラの前方向をXZ平面に投影
            Vector3 cameraForward = refCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            // カメラの右方向を取得
            Vector3 cameraRight = refCamera.transform.right;

            // カメラ基準で移動方向を再計算
            Vector3 adjustedVelocity = cameraForward * movingVelocity.z + cameraRight * movingVelocity.x;

            // プレイヤーの回転(transform.rotation)の更新
            if (adjustedVelocity.magnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(adjustedVelocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, applySpeed);
            }

            // プレイヤーの位置の更新
            rb.linearVelocity = new Vector3(adjustedVelocity.x, rb.linearVelocity.y, adjustedVelocity.z);
        }
        else
        {
            _collider.material = defaultFriction; 
        }
    }

    public void JumpAction()
    {
        // ジャンプ処理
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            isGround = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // 衝突判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // 接触点の法線が上向き（地面）に近い場合のみ接地判定を行う
                if (Vector3.Dot(contact.normal, Vector3.up) > groundLimit)
                {
                    isGround = true;
                    break; // 接地を検出したらループを終了
                }
            }
        }
    }

    // タッチされたときに停止
    public IEnumerator StunPlayer()
    {
        _collider.material = defaultFriction;

        changeObjColor.SetColor(2);

        yield return new WaitForSeconds(stunTime);
        photonView.RPC("SetIsStun", RpcTarget.All, false);

        changeObjColor.SetColor(0);

        yield break;
    }
}
