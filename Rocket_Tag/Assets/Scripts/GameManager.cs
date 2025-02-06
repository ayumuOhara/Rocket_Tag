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
    [SerializeField] TextMeshProUGUI playerCntText;     // Ready完了しているプレイヤー数
    [SerializeField] TextMeshProUGUI infoText;          // playerCntTextの説明文
    [SerializeField] GameObject readyButton;            // 準備完了ボタン
    private const int JOIN_CNT_MIN = 2;                 // 参加人数の最小値
    private bool isGameStarted = false;                 // ゲームが開始されたかどうかのフラグ
    private Player currentRocketHolder;                 // 現在のロケット保持者

    void Start()
    {
        StartCoroutine(WaitPlayersReady());
    }

    IEnumerator WaitPlayersReady()
    {
        while (true)
        {
            int readyCount = GetReadyPlayerCount();
            photonView.RPC("PlayerCntText", RpcTarget.All, readyCount, "準備完了");

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
                Debug.Log($"プレイヤー {player.NickName} がまだ準備完了していません");
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

        Debug.Log("プレイヤーが揃ったのでゲームを開始します");
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
        Debug.Log("ロケット保持者を抽選します");

        List<GameObject> players = GetPlayerList();
        players.RemoveAll(player =>
            player.GetComponent<PhotonView>().Owner == currentRocketHolder); // 既存保持者を除外

        if (players.Count == 0)
        {
            Debug.LogWarning("候補者がいません");
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
            Debug.LogWarning("PhotonView が見つかりません");
        }
    }

    IEnumerator CheckSurvivorCount()
    {
        while (true)
        {
            List<GameObject> players = GetPlayerList();
            int playerCount = players.Count;
            photonView.RPC("PlayerCntText", RpcTarget.All, playerCount, "生存人数");

            if (playerCount <= 1)
            {
                Debug.Log("生存人数が１人になったのでゲームを終了します");
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
        infoText.text = $"{text} / 参加人数";
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
