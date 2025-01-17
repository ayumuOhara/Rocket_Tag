using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;
using TMPro;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.GraphicsBuffer;

// PUNのコールバックを受け取れるようにする為のMonoBehaviourPunCallbacks
public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject rocketObj;          // ロケット
    [SerializeField]
    List<Material> colorMaterial = new List<Material>();
    // [0] DefaultBodyColor
    // [1] DefaultEyeColor
    // [2] UseSkillBodyColor

    private float skillCT; // スキルのクールタイム(α版のみ。マスター版ではCSVファイルを使用)
    [SerializeField] TextMeshProUGUI skillTimerText;
    [SerializeField] GameObject skillCTUI;

    [SerializeField] private Vector3 velocity;              // 移動方向
    private float defaultMoveSpeed = 10.0f;                 // 移動速度(初期値)
    [SerializeField] private float moveSpeed = 10.0f;       // 移動速度
    [SerializeField] private float applySpeed = 0.2f;       // 回転の適用速度
    [SerializeField] private float jumpForce = 20.0f;       // ジャンプ力
    private bool isGround = false;                          // 接地判定
    private float groundLimit = 0.7f;                       // 接地判定のしきい値
    [SerializeField] private CameraController refCamera; 　 // カメラの水平回転を参照する用
    [SerializeField] Rigidbody rb;
    private string targetTag = "Player";                    // タッチ時の検知対象のtag(実装時にはPlayerに変更する)
    public float maxDistance = 5;                           // 検知する最大距離
    [SerializeField] private bool hasRocket;                // ロケットを所持しているか
    public bool isDead;                                     // 死亡判定

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
        if(photonView.IsMine)
        {
            GetVelocity();
            MovePlayer();
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

    // プレイヤーの初期化
    public void SetPlayerCondition()
    {
        // ロケットの状態を初期化
        photonView.RPC("SetHasRocket", RpcTarget.All, false);

        maxDistance = 2f;

        // α版以外では削除
        skillCT = 0;
    }

    // 死亡処理
    void PlayerDead()
    {
        isDead = true;
    }


    //--- プレイヤーの移動処理 ---//

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


    //--- タッチアクション関係 ---//

    // 自身の hasRocket を変更するときのみ使用
    [PunRPC]
    void ToggleHasRocket(bool newHasRocket)
    {
        hasRocket = newHasRocket;
        rocketObj.SetActive(hasRocket);
        Debug.Log($"hasRocket を {hasRocket} に更新しました");
    }
    // hasRocket を設定し、同期
    // 他プレイヤーから hasRocket を変更するときのみ使用
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
        photonView.RPC("ChangeColor", RpcTarget.All, colorMaterial[2].color.r, colorMaterial[2].color.g, colorMaterial[2].color.b, colorMaterial[2].color.a);

        yield return new WaitForSeconds(2.0f);

        // スキルを使用する前の状態
        moveSpeed = defaultMoveSpeed;
        photonView.RPC("ChangeColor", RpcTarget.All, colorMaterial[0].color.r, colorMaterial[0].color.g, colorMaterial[0].color.b, colorMaterial[0].color.a);

        finishSkill = true;

        yield break;
    }

    // プレイヤーの色変更 (修正版)
    [PunRPC]
    void ChangeColor(float r, float g, float b, float a)
    {
        Color newColor = new Color(r, g, b, a);
        GetComponent<Renderer>().material.color = newColor;
    }

    float time = 0;
    public bool finishSkill = true;

    // スキルクールタイム管理
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
