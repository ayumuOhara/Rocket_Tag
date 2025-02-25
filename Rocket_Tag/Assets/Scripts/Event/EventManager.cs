using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EventManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameManager gameManager;
    [SerializeField] UILogManager uiLogManager;
    [SerializeField] private EventData eventData;          // EventData�̎Q��
    [SerializeField] private SkillDataBase skillDataBase;  // SkillData�̎Q��
    [SerializeField] GameObject blindEffect;               // �ڂԂ��C�x���g�pUI

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(TriggerRandomEvent());
        }
    }

    // �����_���ɃC�x���g��I�����郁�\�b�h
    public IEnumerator TriggerRandomEvent()
    {
        Debug.Log("�C�x���g���I�J�n");

        while(true)
        {
            yield return new WaitForSeconds(30.0f);

            int totalPercent = 0;

            // �C�x���g�̊m���̍��v���v�Z
            foreach (var eventSetting in eventData.EventSettings)
            {
                totalPercent += eventSetting.eventPercent;
            }

            // �����_���Ȑ��𐶐��i0����totalPercent�̊ԁj
            int randomValue = Random.Range(0, totalPercent);

            int eventPer = 0;

            // �m���Ɋ�Â��ă����_���ɃC�x���g��I��
            foreach (var eventSetting in eventData.EventSettings)
            {
                eventPer += eventSetting.eventPercent;

                // �����_���Ȓl�����݂̃C�x���g�͈͓̔��Ɏ��܂����ꍇ�A���̃C�x���g��I��
                if (randomValue < eventPer)
                {
                    HandleEvent(eventSetting.EVENT_TYPE);
                    break;
                }
            }
        }        
    }

    // �C�x���g���������郁�\�b�h
    void HandleEvent(EventData.EventType EVENT_TYPE)
    {
        // �C�x���g���Ƃ̏������L�q
        switch (EVENT_TYPE)
        {
            case EventData.EventType.BLIND:
                Debug.Log("�ډB���C�x���g�J�n");
                StartCoroutine(BlindEvent());
                uiLogManager.AddLog("���J�N�V", UILogManager.LogType.Event);
                break;

            case EventData.EventType.BOMB_AREA:
                Debug.Log("�G���A�C�x���g�J�n");
                StartCoroutine(BombAreaEvent());
                uiLogManager.AddLog("�G���A", UILogManager.LogType.Event);
                break;

            case EventData.EventType.CHANGE_POS:
                Debug.Log("�ʒu����ւ��C�x���g�J�n");
                photonView.RPC("ChangePos", RpcTarget.All);
                uiLogManager.AddLog("�ʒu����ւ�", UILogManager.LogType.Event);
                break;

            case EventData.EventType.RANDOM_SPEED:
                Debug.Log("���x�ω��C�x���g�J�n");
                StartCoroutine(RandomSpeedEvent());
                uiLogManager.AddLog("���x�ω�", UILogManager.LogType.Event);
                break;

            case EventData.EventType.RANDOM_SKILL:
                Debug.Log("�X�L���ω��C�x���g�J�n");
                StartCoroutine(RandomSkillEvent());
                uiLogManager.AddLog("�X�L���`�F���W", UILogManager.LogType.Event);
                break;

            default:
                Debug.Log("���݂��܂���");
                break;
        }
    }

    // �ڂԂ��C�x���g
    IEnumerator BlindEvent()
    {
        float eventTime = 10.0f;
        photonView.RPC("BlindEffect", RpcTarget.All, true);
        yield return new WaitForSeconds(eventTime);
        photonView.RPC("BlindEffect", RpcTarget.All, false);

        yield break;
    }

    [PunRPC]
    void BlindEffect(bool isBlind)
    {
        blindEffect.SetActive(isBlind);
    }

    // �G���A�C�x���g
    IEnumerator BombAreaEvent()
    {
        yield break;
    }

    // �v���C���[�̈ʒu����ւ��C�x���g    
    [PunRPC]
    void ChangePos()
    {
        List<GameObject> playerList = gameManager.GetPlayerList();
        List<Vector3> playerPos = new List<Vector3>();

        // ���݂̃v���C���[�̍��W��ۑ�
        foreach (GameObject player in playerList)
        {
            playerPos.Add(player.transform.position);
        }

        // �v���C���[�̍��W���V���b�t��
        for (int i = 0; i < playerPos.Count; i++)
        {
            int rnd = Random.Range(0, playerPos.Count);
            (playerPos[i], playerPos[rnd]) = (playerPos[rnd], playerPos[i]); // C# �̃^�v���X���b�v
        }

        // �V�������W���v���C���[�ɓK�p
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].transform.position = playerPos[i];
        }
    }
        
    // �ړ����x�ω��C�x���g
    IEnumerator RandomSpeedEvent()
    {
        float eventTime = 15.0f;
        List<GameObject> playerList = gameManager.GetPlayerList();
        ChangeSpeed(playerList);
        yield return new WaitForSeconds(eventTime);
        ResetSpeed(playerList);

        yield break;
    }

    // �����_���Ɉړ����x��ω�
    void ChangeSpeed(List<GameObject> playerList)
    {
        int minSpeed = 10;
        int maxSpeed = 30;

        foreach (GameObject player in playerList)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            int rndSpeed = Random.Range(minSpeed, maxSpeed);
            Debug.Log($"���x�ω��F{rndSpeed}");
            playerMovement.SetMoveSpeed(rndSpeed);
        }
    }

    // �ړ����x�����ɖ߂�
    void ResetSpeed(List<GameObject> playerList)
    {
        foreach (GameObject player in playerList)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.SetMoveSpeed(playerMovement.GetDefaultMoveSpeed());
        }
    }

    // �X�L�������_���z�z�C�x���g
    IEnumerator RandomSkillEvent()
    {
        List<GameObject> playerList = gameManager.GetPlayerList();

        foreach (GameObject player in playerList)
        {
            SkillManager skillManager = player.gameObject.GetComponent<SkillManager>();
            int rnd = Random.Range(0, skillDataBase.SkillData.Count);

            SkillData giveSkill = skillDataBase.SkillData[rnd];
            skillManager.SetSkill(giveSkill);
        }

        playerList.Clear();
        yield break;
    }
}
