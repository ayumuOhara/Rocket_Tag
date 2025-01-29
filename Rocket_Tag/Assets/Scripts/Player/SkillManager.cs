using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    TextMeshProUGUI countLimitText;

    public bool finishSkill = true;

    // �X�L����ݒ�
    void SetSkill(SkillData newSkillData)
    {
        skillData = newSkillData;
        countLimit = skillData.countLimit;
        WriteCountLimit();
    }

    // �����X�L�����폜
    public void RemoveSkill()
    {
        skillData = null;
        countLimit = 0;
        WriteCountLimit();
    }

    private void Start()
    {
        changeObjColor = GetComponent<ChangeObjColor>();
        playerMovement = GetComponent<PlayerMovement>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        countLimitText = GameObject.Find("CountLimitText").GetComponent<TextMeshProUGUI>();

        skillIdx = 4;
        SetSkill(skillDataBase.skillDatas[skillIdx]);
    }

    void WriteCountLimit()
    {
        countLimitText.text = $"�c��{countLimit}��";
    }

    // �ݒ肳��Ă���X�L���g�p
    public void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (countLimit > 0 && finishSkill == true)
            {
                Debug.Log($"�y{skillData.skillName}�z���g�p");
                countLimit--;
                WriteCountLimit();

                switch (skillData.skillCode)
                {
                    case 100 : StartCoroutine(Dash());          break;
                    case 101 : StartCoroutine(TimeStop());      break;
                    case 102 : RocketWarp();                    break;
                    case 104 : StartCoroutine(InvisibleBody()); break;
                }

                if(countLimit <= 0)
                {
                    RemoveSkill();
                }
            }
        }
    }
    
    float boostValue = 1.5f;     // �_�b�V���̉����x
    float dashLimit = 3.0f;      // �_�b�V���̌��ʎ���
    // �_�b�V���X�L��
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

    float stopLimit = 5.0f;     // �^�C�}�[��~�̌��ʎ���
    // ���P�b�g�̃^�C�}�[��~
    IEnumerator TimeStop()
    {
        finishSkill = false;

        timeManager.timerView.RPC("IsTimeStop", RpcTarget.All, true);
        yield return new WaitForSeconds(stopLimit);
        timeManager.timerView.RPC("IsTimeStop", RpcTarget.All, false);

        finishSkill = true;

        yield break;
    }

    // ���P�b�g��]��
    void RocketWarp()
    {
        SetPlayerBool mySpb = GetComponent<SetPlayerBool>();
        mySpb.SetHasRocket(false);

        List<GameObject> players = gameManager.GetPlayerList();
        int rnd = Random.Range(0, players.Count);

        SetPlayerBool targetSpb = players[rnd].GetComponent<SetPlayerBool>();
        targetSpb.SetHasRocket(true);
    }

    float heatUpCnt = 30.0f;    // �J�E���g�̐i�s��
    // �n�����Ƃ��Ƀ��P�b�g�̃J�E���g��i�s
    public void HeatUpCnt()
    {
        countLimit--;
        timeManager.SyncRocketCount(timeManager.rocketCount -= heatUpCnt);
        if(timeManager.rocketCount <= 0)
        {
            timeManager.SyncRocketCount(3.0f);
        }
    }

    float invisibleLimit = 10.0f;
    // �v���C���[��s���ɂ���
    IEnumerator InvisibleBody()
    {
        ChangeObjColor rocketCoc = rocketObj.GetComponent<ChangeObjColor>();
        finishSkill = false;

        // ������
        changeObjColor.SetColor(3);
        rocketCoc.SetColor(1);

        yield return new WaitForSeconds(invisibleLimit);

        // �f�t�H���g�ɖ߂�
        changeObjColor.SetColor(0);
        rocketCoc.SetColor(0);

        finishSkill = true;

        yield break;
    }
}
