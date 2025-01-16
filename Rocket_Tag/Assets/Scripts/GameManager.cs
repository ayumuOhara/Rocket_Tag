using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> playerList = new List<GameObject>(); // �Q������v���C���[�̃��X�g
    private const int JOIN_CNT_MIN = 4;  // �Q���l���̍ŏ��l
    private bool isStart = false;        // �Q�[�����n�܂��Ă��邩

    void Start()
    {

    }

    void Update()
    {
        if (isStart)
        {
            CheckSuvivorCnt();
        }
    }

    // �v���C���[���w��̐l���ȏ�Q�����Ă��邩���m�F
    public void CheckJoinedPlayer()
    {
        int playerCount = GetCurrentPlayerCount(); // �J�X�^���v���p�e�B����l�����擾

        if (playerCount >= JOIN_CNT_MIN) // �K�v�l���𖞂����Ă��邩
        {
            Debug.Log("�v���C���[���������̂ŃQ�[�����J�n���܂�");
            isStart = true;
            ChooseRocketPlayer();
        }
        else
        {
            Debug.Log($"�v���C���[�������܂őҋ@���܂� ({playerCount}/{JOIN_CNT_MIN})");
        }
    }

    // �Q�����Ă���v���C���[����1�l��I�сA���P�b�g��t�^
    void ChooseRocketPlayer()
    {
        if (playerList.Count == 0)
        {
            Debug.LogWarning("�v���C���[��������܂���B");
            return;
        }

        // �����_���Ƀv���C���[�𒊑I
        int rnd = Random.Range(0, playerList.Count);
        GameObject selectedPlayer = playerList[rnd];

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

    // �����l�����m�F
    void CheckSuvivorCnt()
    {
        if (playerList.Count == 1)
        {
            Debug.Log("�����l����1�l�ɂȂ����̂ŃQ�[�����I�����܂�");
            isStart = false;
        }
        else
        {
            Debug.Log($"�����l����{playerList.Count}�l�ł��B�܂��I�������𖞂����Ă��܂���B");
        }
    }

    // �J�X�^���v���p�e�B�𗘗p���Č��݂̃v���C���[�l�����擾
    private int GetCurrentPlayerCount()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerCount", out object count))
        {
            return (int)count;
        }
        return 0;
    }

    // �v���C���[�����[���ɎQ�������ۂɌĂ΂��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCount(1);
        CheckJoinedPlayer(); // �V�K�v���C���[�Q�����ɐl�����`�F�b�N
    }

    // �v���C���[�����[������ޏo�����ۂɌĂ΂��
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount(-1);
        CheckJoinedPlayer(); // �v���C���[�ޏo���ɐl�����ă`�F�b�N
    }

    // �v���C���[�l�����X�V���郁�\�b�h
    private void UpdatePlayerCount(int delta)
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            int currentCount = GetCurrentPlayerCount();
            currentCount += delta;

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "PlayerCount", currentCount }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            Debug.Log($"���݂̃v���C���[��: {currentCount}");
        }
    }
}
