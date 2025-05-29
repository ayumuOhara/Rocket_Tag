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
    [SerializeField] Image cooldownMask;  // クール用
    public bool SkillCool = true;

    PlayerMovement playerMovement;
    TimeManager timeManager;
    GameManager gameManager;

    [SerializeField] GameObject player;
    [SerializeField] GameObject rocketObj;
    [SerializeField] GameObject stickyZone;
    [SerializeField] Image skillIcon;

    public bool finishSkill = true;

    float SkillCT = 10.0f;//スキルのクールタイム

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        skillIcon   = GameObject.Find("SkillIcon").GetComponent<Image>();
        Debug.Log($"SkillIcon：{skillIcon}");
        cooldownMask = GameObject.Find("SkillCoolTime").GetComponent<Image>();
    }

    // 設定されているスキル使用
    public void UseSkill()
    {
        if (finishSkill && SkillCool)
        {
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

        StartCoroutine(CoolTime(5));

        yield break;
    }

    public IEnumerator CoolTime(float SkillCT)//クールタイム
    {
        Debug.Log("呼ばれた");
        float elapsed = 0f;
        SkillCool = false;
        cooldownMask.fillAmount = 1f;

        while (elapsed < SkillCT)
        {
            elapsed += Time.deltaTime;
            cooldownMask.fillAmount = 1f - (elapsed / SkillCT);
            yield return null;
        }

        cooldownMask.fillAmount = 0f;
        SkillCool = true;
    }
}