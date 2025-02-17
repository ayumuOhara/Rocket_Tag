using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SkillManager : MonoBehaviourPunCallbacks
{
    [SerializeField] SkillDataBase skillDataBase;
    [SerializeField] public SkillData skillData;
    public int skillIdx;

    [SerializeField] ObserveDistance observeDistance;

    PlayerMovement playerMovement;
    TimeManager timeManager;
    GameManager gameManager;

    [SerializeField] GameObject player;
    [SerializeField] GameObject rocketObj;
    [SerializeField] GameObject stickyZone;

    public bool finishSkill = true;

    // スキルを設定
    public void SetSkill(SkillData newSkillData)
    {
        skillData = newSkillData;
    }

    // 所持スキルを削除
    public void RemoveSkill()
    {
        skillData = null;
    }

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        skillIdx = 0;
        SetSkill(skillDataBase.SkillData[skillIdx]);
    }

    // 設定されているスキル使用
    public void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (finishSkill == true)
            {
                Debug.Log($"【{skillData.skillName}】を使用");

                switch (skillData.SkillId)
                {
                    case 101: break;
                    case 102: photonView.RPC("PutStickyZone", RpcTarget.All); break;
                    case 103: DangerousGift();                                break;
                    case 104: SmashPunch();                                   break;
                    case 105: StartCoroutine(Dash());                         break;
                }

                SendSkillData();
            }
        }
    }

    // ねばねばゾーン設置
    [PunRPC]
    void PutStickyZone()
    {
        GameObject zone = Instantiate(stickyZone);
        Vector3 playerPos = player.transform.position;
        playerPos += Vector3.down;
        zone.transform.position = playerPos;
    }

    // プレイヤーにロケットを配布
    void DangerousGift()
    {
        int playerCnt = gameManager.GetPlayerList().Count;

        int minCnt = 1;
        int maxCnt = playerCnt > 3 ? 3 : playerCnt;
        int rocketCnt = Random.Range(minCnt, maxCnt);

        for(int i = 0; i < rocketCnt; i++)
        {
            gameManager.ChooseRocketPlayer();
        }
    }

    // スマッシュパンチ
    void SmashPunch()
    {
        GameObject target = observeDistance.GetTargetDistance();

        if (target == null) return; // ターゲットがいない場合は処理しない

        // プレイヤーをターゲットの方向へ向ける
        transform.LookAt(target.transform.position);
        KnockBackTarget(target);
    }

    // 対象を吹っ飛ばす
    public void KnockBackTarget(GameObject target)
    {
        PhotonView targetView = target.GetComponent<PhotonView>();
        targetView.RPC("SetIsStun", RpcTarget.All, true);

        // ターゲットの Rigidbody を取得
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            // 吹っ飛ばす方向を計算（プレイヤー → ターゲット の方向）
            Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;

            // 吹っ飛ばす力（適宜調整）
            float knockbackForce = 30f;

            // ターゲットに力を加える
            targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }        
    }
    
    // ダッシュスキル
    IEnumerator Dash()
    {
        float boostValue = 1.5f;     // ダッシュの加速度
        float dashLimit = 3.0f;      // ダッシュの効果時間

        finishSkill = false;

        float speed = playerMovement.GetMoveSpeed();
        playerMovement.SetMoveSpeed(speed * boostValue);

        yield return new WaitForSeconds(dashLimit);

        playerMovement.SetMoveSpeed(speed);

        finishSkill = true;

        yield break;
    }

    // スキルをプレイヤーに与える
    void SendSkillData()
    {
        int rnd = Random.Range(0, skillDataBase.SkillData.Count);
        SkillData giveSkill = skillDataBase.SkillData[rnd];
        SetSkill(giveSkill);
    }
}