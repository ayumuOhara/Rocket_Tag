using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class GameManager : MonoBehaviourPun
{
    [SerializeField] InstantiatePlayer instantiatePlayer;
    private const int JOIN_CNT_MIN = 4;       // �Q���l���̍ŏ��l
    bool isStart = false;                     // �Q�[�����n�܂��Ă��邩

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            CheckSuvivorCnt();
        }
    }

    // �v���C���[���w��̐l���ȏ�Q�����Ă��邩
    public void CheckJoinedPlayer()
    {
        var currentCnt = instantiatePlayer.GetCurrentPlayerCount();
        Debug.Log($"���݂̎Q���l���F{currentCnt}");

        if (currentCnt >= JOIN_CNT_MIN)       // �S����������������������J�n�ɂ���
        {
            Debug.Log("�v���C���[���������̂ŃQ�[�����J�n���܂�");
            isStart = true;
            ChooseRocketPlayer();
        }
        else
        {
            Debug.Log("�v���C���[�������܂őҋ@���܂�");
        }
    }

    // �Q�����Ă���v���C���[����P�l��I�сA���P�b�g��t�^
    void ChooseRocketPlayer()
    {
        // "Player" �^�O�������ׂẴv���C���[���擾
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("�v���C���[��������܂���B");
            return;
        }

        // �����_���Ƀv���C���[�𒊑I
        int rnd = Random.Range(0, players.Length);
        GameObject selectedPlayer = players[rnd];

        // ���I�����v���C���[�� PhotonView ���擾
        PhotonView targetPhotonView = selectedPlayer.GetComponent<PhotonView>();
        if (targetPhotonView != null)
        {
            // hasRocket �� true �ɐݒ肵�A����
            targetPhotonView.RPC("SetHasRocket", RpcTarget.All, true);
        }
        else
        {
            Debug.LogWarning("PhotonView ��������܂���B");
        }
    }

    // �����c���Ă���l�����l������
    void CheckSuvivorCnt()
    {
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        if (player.Length == 1)
        {
            Debug.Log("�����l�����P�l�ɂȂ����̂ŃQ�[�����I�����܂�");
            isStart = false;
        }
        else
        {
            Debug.Log("�����l�����c�P�l�ɂȂ�܂őҋ@");
        }
    }
}