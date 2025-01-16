using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;

// PUNのコールバックを受け取れるようにする為のMonoBehaviourPunCallbacks
public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject rocketObj;          // ロケット

    [SerializeField] private Vector3 velocity;              // 移動方向
    [SerializeField] private float moveSpeed = 10.0f;       // 移動速度
    [SerializeField] private float applySpeed = 0.2f;       // 回転の適用速度
    [SerializeField] private float jumpForce = 20.0f;       // ジャンプ力
    private bool isGround = false;                          // 接地判定
    [SerializeField] private CameraController refCamera; 　 // カメラの水平回転を参照する用
    [SerializeField] Rigidbody rb;
    private string targetTag = "Player";                    // タッチ時の検知対象のtag(実装時にはPlayerに変更する)
    public float maxDistance = 1f;                         // 検知する最大距離
    [SerializeField] private bool hasRocket;                // ロケットを所持しているか

    private void Awake()
    {
        // ロケットの状態を初期化
        photonView.RPC("SetHasRocket", RpcTarget.All, false);
    }

    void Start()
    {   
        refCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CameraController>();
    }

    void Update()
    {
        if(photonView.IsMine)
        {
            GetVelocity();
            MovePlayer();

            if (hasRocket)
            {
                PlayerAction();
            }
        }
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

        // 速度ベクトルの長さを1秒でmoveSpeedだけ進むように調整します
        velocity = velocity.normalized * moveSpeed * Time.deltaTime;
    }

    // 取得したベクトルの方向に移動&回転させる+ジャンプ処理
    void MovePlayer()
    {
        // ジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            isGround = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // いずれかの方向に移動している場合
        if (velocity.magnitude > 0)
        {
            // プレイヤーの回転(transform.rotation)の更新
            // 無回転状態のプレイヤーのZ+方向(後頭部)を、
            // カメラの水平回転(refCamera.hRotation)で回した移動の反対方向(-velocity)に回す回転に段々近づけます
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(refCamera.hRotation * -velocity),
                                                  applySpeed);

            // プレイヤーの位置(transform.position)の更新
            // カメラの水平回転(refCamera.hRotation)で回した移動方向(velocity)を足し込みます
            transform.position += refCamera.hRotation * velocity;
        }
    }

    // 押下されたキーに応じてアクション
    void PlayerAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("スキル１を使用した");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("スキル２を使用した");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("ロケットを投擲した");
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject target = GetTargetDistance();
            if (target != null)
            {
                // 自分の hasRocket を切り替え
                photonView.RPC("ToggleHasRocket", RpcTarget.All, !hasRocket);

                // ターゲットの hasRocket を切り替え
                PhotonView targetPhotonView = target.GetComponent<PhotonView>();
                if (targetPhotonView != null)
                {
                    targetPhotonView.RPC("ToggleHasRocket", RpcTarget.All, !target.GetComponent<PlayerController>().hasRocket);
                }
            }
        }
    }

    [PunRPC]
    void ToggleHasRocket(bool newHasRocket)
    {
        hasRocket = newHasRocket;
        rocketObj.SetActive(hasRocket);
        Debug.Log($"hasRocket を {hasRocket} に更新しました");
    }

    // hasRocket を設定し、同期
    [PunRPC]
    public void SetHasRocket(bool newHasRocket)
    {
        hasRocket = newHasRocket;
        rocketObj.SetActive(hasRocket);
        Debug.Log($"{photonView.Owner.NickName} の hasRocket を {hasRocket} に設定しました");
    }

    // 他のプレイヤーとの距離を測る
    GameObject GetTargetDistance()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        GameObject nearestObject = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            Vector3 direction = target.transform.position - transform.position;
            float distance = direction.magnitude;

            // 距離が範囲内かチェック
            if (distance <= maxDistance)
            {
                // Raycastを発射して障害物がないか確認
                Ray ray = new Ray(transform.position, direction.normalized);
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                {
                    if (hit.collider.gameObject == target)
                    {
                        // 最短距離を更新
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestObject = target;
                        }
                    }
                }
            }
        }

        if (nearestObject != null)
        {
            Debug.Log($"最も近いオブジェクト: {nearestObject.name}, 距離: {nearestDistance}");
        }
        else
        {
            Debug.Log("検知対象が見つかりませんでした");
        }

        return nearestObject;
    }

    // 接地判定
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }
}
