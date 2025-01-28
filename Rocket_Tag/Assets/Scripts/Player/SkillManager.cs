using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillManager : MonoBehaviourPunCallbacks
{
    [SerializeField] SkillDataBase skillDataBase;
    [SerializeField] public SkillData skillData;
    public int skillIdx;
    [SerializeField] int countLimit;

    ChangeObjColor changeObjColor;
    PlayerMovement playerMovement;
    TimeManager timeManager;
    GameManager gameManager;

    [SerializeField] GameObject rocketObj;

    public bool finishSkill = true;

    // スキルを設定
    void SetSkill(SkillData newSkillData)
    {
        skillData = newSkillData;
        countLimit = skillData.countLimit;
    }

    // 所持スキルを削除
    public void RemoveSkill()
    {
        skillData = null;
    }

    private void Start()
    {
        changeObjColor = GetComponent<ChangeObjColor>();
        playerMovement = GetComponent<PlayerMovement>();
        timeManager = rocketObj.GetComponent<TimeManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        skillIdx = 0;
        SetSkill(skillDataBase.skillDatas[skillIdx]);
    }

    // 設定されているスキル使用
    public void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (countLimit > 0 && finishSkill == true)
            {
                Debug.Log($"【{skillData.skillName}】を使用");
                countLimit--;

                switch (skillData.skillCode)
                {
                    case 101 : StartCoroutine(Dash());          break;
                    case 102 : StartCoroutine(TimeStop());      break;
                    case 103 : RocketWarp();                    break;
                    case 105 : StartCoroutine(InvisibleBody()); break;
                }

                if(countLimit <= 0)
                {
                    RemoveSkill();
                }
            }
        }
    }
    
    float boostValue = 1.5f;     // ダッシュの加速度
    float dashLimit = 3.0f;      // ダッシュの効果時間
    // ダッシュスキル
    IEnumerator Dash()
    {
        finishSkill = false;

        float speed = playerMovement.GetMoveSpeed();
        playerMovement.SetMoveSpeed(speed * boostValue);
        changeObjColor.SetColor(1);

        yield return new WaitForSeconds(dashLimit);

        playerMovement.SetMoveSpeed(speed);
        changeObjColor.SetColor(0);

        finishSkill = true;

        yield break;
    }

    float stopLimit = 5.0f;     // タイマー停止の効果時間
    // ロケットのタイマー停止
    IEnumerator TimeStop()
    {
        finishSkill = false;

        timeManager.isTimeStop = true;
        yield return new WaitForSeconds(stopLimit);
        timeManager.isTimeStop = false;

        finishSkill = true;

        yield break;
    }

    // ロケットを転移
    void RocketWarp()
    {
        SetPlayerBool mySpb = GetComponent<SetPlayerBool>();
        mySpb.SetHasRocket(false);

        List<GameObject> players = gameManager.GetPlayerList();
        int rnd = Random.Range(0, players.Count);

        SetPlayerBool targetSpb = players[rnd].GetComponent<SetPlayerBool>();
        targetSpb.SetHasRocket(true);
    }

    float heatUpCnt = 30.0f;    // カウントの進行数
    // 渡したときにロケットのカウントを進行
    public void HeatUpCnt()
    {
        countLimit--;
        timeManager.rocketCount -= heatUpCnt;
        if(timeManager.rocketCount <= 0)
        {
            timeManager.rocketCount = 3.0f;
        }
    }

    float invisibleLimit = 10.0f;
    // プレイヤーを不可視にする
    IEnumerator InvisibleBody()
    {
        ChangeObjColor rocketCoc = rocketObj.GetComponent<ChangeObjColor>();
        finishSkill = false;

        // 透明化
        changeObjColor.SetColor(3);
        rocketCoc.SetColor(1);

        yield return new WaitForSeconds(invisibleLimit);

        // デフォルトに戻す
        changeObjColor.SetColor(0);
        rocketCoc.SetColor(0);

        finishSkill = true;

        yield break;
    }
}
