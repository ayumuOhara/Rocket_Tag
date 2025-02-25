using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] Image skillIcon;

    public bool finishSkill = true;

    // スキルを設定
    public void SetSkill(SkillData newSkillData)
    {
        skillData = newSkillData;
        skillIcon.sprite = newSkillData.skillIcon;
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
        skillIcon   = GameObject.Find("SKillIcon").GetComponent<Image>();

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

                switch (skillData.skillId)
                {
                    case 101: break;
                    case 102: photonView.RPC("PutStickyZone", RpcTarget.All); break;
                    case 103: /*DangerousGift();*/                            break;
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

        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;
            float knockbackForce = 30f;

            // 直接 velocity に適用して即座に動かす
            targetRb.linearVelocity = knockbackDirection * knockbackForce;
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