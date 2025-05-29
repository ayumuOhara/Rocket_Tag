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
    PlayerMovement playerMovement;
    TimeManager timeManager;
    GameManager gameManager;

    [SerializeField] GameObject player;
    [SerializeField] GameObject rocketObj;
    [SerializeField] GameObject stickyZone;
    [SerializeField] Image skillCTImage;

    public bool finishSkill = true;
    bool skillReady = true;

    float time = 0;
    float skillCT = 0;
    float skillCTmax = 5.0f;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        skillCTImage = GameObject.Find("SkillCoolTime").GetComponent<Image>();
        Debug.Log($"SkillIcon：{skillCTImage}");
    }

    // スキルのクールタイム
    IEnumerator SkillCoolTime()
    {
        Debug.Log("クールタイムコルーチン開始");

        skillCT = skillCTmax;
        skillReady = false;
        time = 0f;
        float cooltimeAmount = skillCT / skillCTmax;

        if (skillCTImage != null)
        {
            skillCTImage.fillAmount = cooltimeAmount; // ← ここでUI更新
        }

        while (skillCT > 0)
        {
            if (finishSkill)
            {
                Debug.Log("クールタイム処理開始");

                time += Time.deltaTime;
                skillCT -= time;

                cooltimeAmount = skillCT / skillCTmax;
                if (skillCTImage != null)
                {
                    skillCTImage.fillAmount = cooltimeAmount; // ← ここでUI更新
                }

                yield return null;
            }

            yield return null;
        }

        skillReady = true;
        Debug.Log("クールタイム処理停止");
    }


    // 設定されているスキル使用
    public void UseSkill()
    {
        if (finishSkill && skillReady)
        {
            StartCoroutine(SkillCoolTime());
            StartCoroutine(Dash());
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

    // ダッシュスキル
    IEnumerator Dash()
    {
        Debug.Log("足が速～い！！！！！！！！！");

        float boostValue = 1.5f;     // ダッシュの加速度
        float dashLimit = 3.0f;      // ダッシュの効果時間

        finishSkill = false;

        float speed = playerMovement.GetDefaultMoveSpeed();
        playerMovement.SetMoveSpeed(speed * boostValue);

        yield return new WaitForSeconds(dashLimit);

        playerMovement.SetMoveSpeed(speed);

        finishSkill = true;
        yield break;
    }
}