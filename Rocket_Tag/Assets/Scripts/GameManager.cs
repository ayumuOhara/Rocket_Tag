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
    [SerializeField] EventManager eventManager;
    [SerializeField] TimeManager timeManager;
    [SerializeField] InstantiatePlayer instantiatePlayer;
    [SerializeField] PlayerReady playerReady;
    [SerializeField] TextMeshProUGUI playerCntText;     // Ready�������Ă���v���C���[��
    [SerializeField] TextMeshProUGUI infoText;          // playerCntText�̐�����
    [SerializeField] GameObject readyButton;            // ���������{�^��
    private const int JOIN_CNT_MIN = 2;                 // �Q���l���̍ŏ��l
    private bool isGameStarted = false;                 // �Q�[�����J�n���ꂽ���ǂ����̃t���O
    private Player currentRocketHolder;                 // ���݂̃��P�b�g�ێ���

    void Start()
    {
        StartCoroutine(WaitPlayersReady());
    }

    IEnumerator WaitPlayersReady()
    {
        while (true)
        {
            int readyCount = GetReadyPlayerCount();
            photonView.RPC("PlayerCntText", RpcTarget.All, readyCount, "��������");

            if (PhotonNetwork.IsMasterClient && CheckJoinedPlayer() && CheckAllPlayersReady() && !isGameStarted)
            {
                photonView.RPC(nameof(StartGame), RpcTarget.All);
                yield break;
            }

            yield return null;
        }
    }

    public bool CheckJoinedPlayer()
    {
        var currentCnt = instantiatePlayer.GetCurrentPlayerCount();
        return currentCnt >= JOIN_CNT_MIN;
    }

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

    [PunRPC]
    void StartGame()
    {
        timeManager.ResetRocketCount();
        if (isGameStarted) return;

        Debug.Log("�v���C���[���������̂ŃQ�[�����J�n���܂�");
        isGameStarted = true;
        timeManager.isTimeStart = true;
        readyButton.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            ChooseRocketPlayer();
        }

        StartCoroutine(eventManager.TriggerRandomEvent());
        StartCoroutine(CheckSurvivorCount());
    }

    public void ChooseRocketPlayer()
    {
        Debug.Log("���P�b�g�ێ��҂𒊑I���܂�");

        List<GameObject> players = GetPlayerList();
        players.RemoveAll(player =>
            player.GetComponent<PhotonView>().Owner == currentRocketHolder); // �����ێ��҂����O

        if (players.Count == 0)
        {
            Debug.LogWarning("���҂����܂���");
        }

        int rnd = Random.Range(0, players.Count);
        GameObject selectedPlayer = players[rnd];
        PhotonView targetPhotonView = selectedPlayer.GetComponent<PhotonView>();

        if (targetPhotonView != null)
        {
            currentRocketHolder = targetPhotonView.Owner;
            targetPhotonView.RPC("SetHasRocket", RpcTarget.All, true);
        }
        else
        {
            Debug.LogWarning("PhotonView ��������܂���");
        }
    }

    IEnumerator CheckSurvivorCount()
    {
        while (true)
        {
            List<GameObject> players = GetPlayerList();
            int playerCount = players.Count;
            photonView.RPC("PlayerCntText", RpcTarget.All, playerCount, "�����l��");

            if (playerCount <= 1)
            {
                Debug.Log("�����l�����P�l�ɂȂ����̂ŃQ�[�����I�����܂�");
                readyButton.SetActive(true);
                playerReady.SetReady(false);
                setPlayerBool.SetPlayerCondition();
                timeManager.isTimeStart = false;
                StartCoroutine(WaitPlayersReady());
                yield break;
            }
            yield return null;
        }
    }

    [PunRPC]
    void PlayerCntText(int playerCnt, string text)
    {
        playerCntText.text = $"{playerCnt} / {instantiatePlayer.GetCurrentPlayerCount()}";
        infoText.text = $"{text} / �Q���l��";
    }

    public List<GameObject> GetPlayerList()
    {
        List<GameObject> players = new List<GameObject>();
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        players.RemoveAll(player =>
        {
            SetPlayerBool spb = player.GetComponent<SetPlayerBool>();
            return spb != null && spb.isDead;
        });

        return players;
    }
}
