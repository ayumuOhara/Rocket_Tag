using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public PlayerController playerController;
    public SetPlayerBool setPlayerBool;
    [SerializeField] InstantiatePlayer instantiatePlayer;
    [SerializeField] PlayerReady playerReady;
    [SerializeField] TextMeshProUGUI playerCntText;     // Ready�������Ă���v���C���[��
    [SerializeField] TextMeshProUGUI infoText;          // playerCntText�̐�����
    [SerializeField] GameObject readyButton;            // ���������{�^��
    private const int JOIN_CNT_MIN = 2;                 // �Q���l���̍ŏ��l
    private bool isGameStarted = false;                 // �Q�[�����J�n���ꂽ���ǂ����̃t���O

    void Start()
    {
        StartCoroutine(WaitPlayersReady());
    }

    void Update()
    {

    }

    // �v���C���[�̏���������҂�
    IEnumerator WaitPlayersReady()
    {
        while (true)
        {
            int readyCount = GetReadyPlayerCount();
            playerCntText.text = $"{readyCount} /   {instantiatePlayer.GetCurrentPlayerCount()}";
            infoText.text      = $"�������� /  �Q���l��";

            // �}�X�^�[�N���C�A���g�̂݃Q�[���J�n���������s
            if (PhotonNetwork.IsMasterClient && CheckJoinedPlayer() && CheckAllPlayersReady() && !isGameStarted)
            {
                photonView.RPC(nameof(StartGame), RpcTarget.All); // �S���ɃQ�[���J�n��ʒm
                yield break;
            }

            yield return null;
        }
    }

    // �v���C���[���w��̐l���ȏ�Q�����Ă��邩
    public bool CheckJoinedPlayer()
    {
        var currentCnt = instantiatePlayer.GetCurrentPlayerCount();
        return currentCnt >= JOIN_CNT_MIN;
    }

    // �Q�����Ă���v���C���[�S����Ready������
    bool CheckAllPlayersReady()
    {
        Player[] players = PhotonNetwork.PlayerList;

        foreach (var player in players)
        {
            if (!player.CustomProperties.ContainsKey("IsReady") || !(bool)player.CustomProperties["IsReady"])
            {
                Debug.Log($"�v���C���[ {player.NickName} ���܂������������Ă��܂���");
                return false;
            }
        }

        return true;
    }

    // Ready�����������v���C���[�̐����擾
    int GetReadyPlayerCount()
    {
        Player[] players = PhotonNetwork.PlayerList;
        int readyCount = 0;

        foreach (var player in players)
        {
            if (player.CustomProperties.ContainsKey("IsReady") && (bool)player.CustomProperties["IsReady"])
            {
                readyCount++;
            }
        }

        return readyCount;
    }

    // �Q�[���X�^�[�g�i�S�N���C�A���g�ŌĂяo�����j
    [PunRPC]
    void StartGame()
    {
        if (isGameStarted) return; // ���łɃQ�[�����J�n����Ă���Ή������Ȃ�

        Debug.Log("�v���C���[���������̂ŃQ�[�����J�n���܂�");
        isGameStarted = true; // �Q�[���J�n�t���O�𗧂Ă�
        readyButton.SetActive(false);

        // �}�X�^�[�N���C�A���g�̂݃��P�b�g�t�^���������s
        if (PhotonNetwork.IsMasterClient)
        {
            ChooseRocketPlayer();
        }

        StartCoroutine(CheckSuvivorCnt());
    }

    // �Q�����Ă���v���C���[����P�l��I�сA���P�b�g��t�^
    public void ChooseRocketPlayer()
    {
        Debug.Log("�v���C���[�𒊑I���܂�");

        List<GameObject> players = new List<GameObject>();
        players = GetPlayerCount();
        int rnd = Random.Range(0, players.Count);
        GameObject selectedPlayer = players[rnd];

        PhotonView targetPhotonView = selectedPlayer.GetComponent<PhotonView>();
        if (targetPhotonView != null)
        {
            targetPhotonView.RPC("SetHasRocket", RpcTarget.All, true);
        }
        else
        {
            Debug.LogWarning("PhotonView ��������܂���B");
        }
    }

    // �c��l�����m�F���A�c��P�l�ɂȂ�����I��
    IEnumerator CheckSuvivorCnt()
    {
        while (true)
        {
            List<GameObject> players = GetPlayerCount();
            int playerCnt = players.Count;
            playerCntText.text = $"{playerCnt} /   {instantiatePlayer.GetCurrentPlayerCount()}";
            infoText.text      = $"�����l�� /  �Q���l��";

            if (playerCnt <= 1)
            {
                Debug.Log("�����l�����P�l�ɂȂ����̂ŃQ�[�����I�����܂�");
                readyButton.SetActive(true);
                playerReady.SetReady(false);
                setPlayerBool.SetPlayerCondition();
                StartCoroutine(WaitPlayersReady());
                yield break;
            }
            yield return null;
        }
    }

    // �����҃��X�g��Ԃ�
    List<GameObject> GetPlayerCount()
    {
        List<GameObject> players = new List<GameObject>();
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        for (int i = 0; i < players.Count; i++)
        {
            SetPlayerBool spb = players[i].GetComponent<SetPlayerBool>();
            if (spb.isDead)
            {
                players.RemoveAt(i);
            }
        }

        return players;
    }
}
