using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;
using TMPro;

// PUNのコールバックを受け取れるようにする為のMonoBehaviourPunCallbacks
public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject rocketObj;          // ロケット
    [SerializeField]
    List<Material> colorMaterial = new List<Material>();
    // [0] DefaultBodyColor
    // [1] DefaultEyeColor
    // [2] UseSkillBodyColor
    // [3] StunBodyColor

    private float skillCT; // スキルのクールタイム(α版のみ。マスター版ではCSVファイルを使用)
    [SerializeField] TextMeshProUGUI skillTimerText;
    [SerializeField] GameObject skillCTUI;

    [SerializeField] private Vector3 movingVelocity;              // 移動方向
    private float defaultMoveSpeed = 10.0f;                 // 移動速度(初期値)
    [SerializeField] private float moveSpeed = 10.0f;       // 移動速度
    [SerializeField] private float applySpeed = 0.2f;       // 回転の適用速度
    [SerializeField] private float jumpForce = 20.0f;       // ジャンプ力
    private bool isGround = false;                          // 接地判定
    private float groundLimit = 0.7f;                       // 接地判定のしきい値
    [SerializeField] private CameraController refCamera; 　 // カメラの水平回転を参照する用
    [SerializeField] Rigidbody rb;
    private string targetTag = "Player";                    // タッチ時の検知対象のtag(実装時にはPlayerに変更する)
    public float maxDistance = 3.0f;                           // 検知する最大距離
    [SerializeField] private bool hasRocket;                // ロケットを所持しているか
    public bool isDead;                                     // 死亡判定
    bool isStun;                                            // スタン判定

    private void Awake()
    {
        SetPlayerCondition();
        skillCTUI = GameObject.Find("SkillCTUI");
        skillTimerText = GameObject.Find("SkillTimerText").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {   
        refCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CameraController>();
        skillCTUI.SetActive(false);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (isDead == false)
            {
                // ジャンプ処理
                if (Input.GetKeyDown(KeyCode.Space) && isGround)
                {
                    isGround = false;
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (isDead == false)
            {
                GetVelocity();
                PlayerMovement();
                if (skillCT >= 0 && finishSkill)
                {
                    if (skillCT <= 0)
                    {
                        skillCTUI.SetActive(false);
                    }
                    else
                    {
                        // α版のみ使用(マスター版では削除)
                        SkillCool();
                    }
                }

                if (hasRocket)
                {
                    PlayerAction();
                }
            }
        }
    }

    // プレイヤーの状態の初期化
    public void SetPlayerCondition()
    {
        // ロケットの状態を初期化
        photonView.RPC("SetHasRocket", RpcTarget.All, false);

        photonView.RPC("SetPlayerDead", RpcTarget.All, false);
        isStun = false;

        maxDistance = 3f;

        // α版以外では削除
        skillCT = 0;
    }

    // 死亡処理
    [PunRPC]
    public void SetPlayerDead(bool newIsDead)
    {
        isDead = newIsDead;
    }


    //--- プレイヤーの移動処理 ---//

    // 押下された移動キーに応じてベクトルを取得
    void GetVelocity()
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
    void PlayerMovement()
    {
        if (isStun == false)
        {
            // いずれかの方向に移動している場合
            if (movingVelocity.magnitude > 0)
            {
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

    float stunTime = 3.0f;

    // タッチされたときに停止
    IEnumerator StunPlayer()
    {
        photonView.RPC("ChangeColor", RpcTarget.All,
                        colorMaterial[3].color.r,
                        colorMaterial[3].color.g,
                        colorMaterial[3].color.b,
                        colorMaterial[3].color.a);

        yield return new WaitForSeconds(stunTime);
        photonView.RPC("SetIsStun",RpcTarget.All, false);

        photonView.RPC("ChangeColor", RpcTarget.All,
                        colorMaterial[0].color.r,
                        colorMaterial[0].color.g,
                        colorMaterial[0].color.b,
                        colorMaterial[0].color.a);

        yield break;
    }

    [PunRPC]
    public void SetIsStun(bool newIsStun)
    {
        isStun = newIsStun;
        if (isStun)
        {
            StartCoroutine(StunPlayer());
        }
    }


    //--- プレイヤーの特殊アクション処理 ---//

    // 押下されたキーに応じてアクション
    void PlayerAction()
    {
        UseSkill();
        RocketAction();        
    }
    // スキル使用
    void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"スキルCT：{skillCT}");
            if(skillCT <= 0f)
            {
                Debug.Log("スキル１を使用した");
                StartCoroutine(DashSkill());
            }
            else
            {
                Debug.Log("スキル１の使用条件を満たしていません");
            }            
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (skillCT >= 30.0f)
            {
                Debug.Log("スキル２を使用した");
            }
            else
            {
                Debug.Log("スキル２の使用条件を満たしていません");
            }
        }
    }
    // タッチ/投擲アクション
    void RocketAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("ロケットを投擲した");
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject target = GetTargetDistance();
            if (target != null)
            {
                PlayerController player = target.GetComponent<PlayerController>();

                // 自分の hasRocket を切り替え
                photonView.RPC("SetHasRocket", RpcTarget.All, !hasRocket);

                // ターゲットの hasRocket を切り替え
                PhotonView targetPhotonView = target.GetComponent<PhotonView>();
                if (targetPhotonView != null)
                {
                    targetPhotonView.RPC("SetHasRocket", RpcTarget.All, !player.hasRocket);
                    targetPhotonView.RPC("SetIsStun",RpcTarget.All, true);
                }
            }
        }
    }


    //--- タッチアクション関係 ---//

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


    //--- スキル関係 ---//

    // ダッシュスキル効果(修正版)
    IEnumerator DashSkill()
    {
        skillCT = 30.0f;
        finishSkill = false;
        skillCTUI.SetActive(true);
        skillTimerText.text = skillCT.ToString();

        // スキルを使用した状態
        moveSpeed *= 3.0f;
        photonView.RPC( "ChangeColor", RpcTarget.All, 
                        colorMaterial[2].color.r, 
                        colorMaterial[2].color.g, 
                        colorMaterial[2].color.b, 
                        colorMaterial[2].color.a     );

        yield return new WaitForSeconds(2.0f);

        // スキルを使用する前の状態
        moveSpeed = defaultMoveSpeed;
        photonView.RPC( "ChangeColor", RpcTarget.All, 
                        colorMaterial[0].color.r, 
                        colorMaterial[0].color.g, 
                        colorMaterial[0].color.b, 
                        colorMaterial[0].color.a     );

        finishSkill = true;

        yield break;
    }

    // プレイヤーの色変更
    [PunRPC]
    void ChangeColor(float r, float g, float b, float a)
    {
        Color newColor = new Color(r, g, b, a);
        GetComponent<Renderer>().material.color = newColor;
    }

    float time = 0;
    public bool finishSkill = true;

    // スキルクールタイム
    void SkillCool()
    {
        time += Time.deltaTime;
        if(time > 1)
        {
            skillCT = Mathf.Clamp(skillCT - 1, 0, 30.0f);
            skillTimerText.text = skillCT.ToString();
            time = 0;
        }
    }
}
