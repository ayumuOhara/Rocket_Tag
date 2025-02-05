using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EventManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameManager gameManager;
    [SerializeField] private EventData eventData;  // EventData�̎Q��
    [SerializeField] GameObject blindEffect;       // �ڂԂ��C�x���g�pUI

    // �����_���ɃC�x���g��I�����郁�\�b�h
    public IEnumerator TriggerRandomEvent()
    {
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
                StartCoroutine(BlindEvent());
                break;

            case EventData.EventType.BOMB_AREA:
                StartCoroutine(BombAreaEvent());
                break;

            case EventData.EventType.CHANGE_POS:
                photonView.RPC("ChangePos", RpcTarget.All);
                break;

            case EventData.EventType.JANNKENN:
                StartCoroutine(JannKennEvent());
                break;

            case EventData.EventType.RANDOM_SPEED:
                StartCoroutine(RandomSpeedEvent());
                break;

            case EventData.EventType.RANDOM_SKILL:
                StartCoroutine(RandomSkillEvent());
                break;

            default:
                Debug.Log("���݂��܂���");
                break;
        }
    }

    int blindTime = 5;
    bool isBlind = false;
    // �ڂԂ��C�x���g
    IEnumerator BlindEvent()
    {
        photonView.RPC("BlindEffect", RpcTarget.All);
        yield return new WaitForSeconds(blindTime);
        photonView.RPC("BlindEffect", RpcTarget.All);

        yield break;
    }

    [PunRPC]
    void BlindEffect()
    {
        blindEffect.SetActive(!isBlind);
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

    // ����񂯂�C�x���g
    IEnumerator JannKennEvent()
    {
        yield break;
    }


    // �ړ����x�ω��C�x���g
    IEnumerator RandomSpeedEvent()
    {
        yield break;
    }

    // �X�L�������_���z�z�C�x���g
    IEnumerator RandomSkillEvent()
    {
        yield break;
    }
}
