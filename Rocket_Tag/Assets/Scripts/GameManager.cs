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
    [SerializeField] TextMeshProUGUI playerCntText;     // Ready完了しているプレイヤー数
    [SerializeField] TextMeshProUGUI infoText;          // playerCntTextの説明文
    [SerializeField] GameObject readyButton;            // 準備完了ボタン
    private const int JOIN_CNT_MIN = 2;                 // 参加人数の最小値
    private bool isGameStarted = false;                 // ゲームが開始されたかどうかのフラグ

    void Start()
    {
        StartCoroutine(WaitPlayersReady());
    }

    void Update()
    {

    }

    // プレイヤーの準備完了を待つ
    IEnumerator WaitPlayersReady()
    {
        while (true)
        {
            int readyCount = GetReadyPlayerCount();
            playerCntText.text = $"{readyCount} /   {instantiatePlayer.GetCurrentPlayerCount()}";
            infoText.text      = $"準備完了 /  参加人数";

            // マスタークライアントのみゲーム開始処理を実行
            if (PhotonNetwork.IsMasterClient && CheckJoinedPlayer() && CheckAllPlayersReady() && !isGameStarted)
            {
                photonView.RPC(nameof(StartGame), RpcTarget.All); // 全員にゲーム開始を通知
                yield break;
            }

            yield return null;
        }
    }

    // プレイヤーが指定の人数以上参加しているか
    public bool CheckJoinedPlayer()
    {
        var currentCnt = instantiatePlayer.GetCurrentPlayerCount();
        return currentCnt >= JOIN_CNT_MIN;
    }

    // 参加しているプレイヤー全員がReadyしたか
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

    // Readyが完了したプレイヤーの数を取得
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

    // ゲームスタート（全クライアントで呼び出される）
    [PunRPC]
    void StartGame()
    {
        if (isGameStarted) return; // すでにゲームが開始されていれば何もしない

        Debug.Log("プレイヤーが揃ったのでゲームを開始します");
        isGameStarted = true; // ゲーム開始フラグを立てる
        readyButton.SetActive(false);

        // マスタークライアントのみロケット付与処理を実行
        if (PhotonNetwork.IsMasterClient)
        {
            ChooseRocketPlayer();
        }

        StartCoroutine(CheckSuvivorCnt());
    }

    // 参加しているプレイヤーから１人を選び、ロケットを付与
    public void ChooseRocketPlayer()
    {
        Debug.Log("プレイヤーを抽選します");

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
            Debug.LogWarning("PhotonView が見つかりません。");
        }
    }

    // 残り人数を確認し、残り１人になったら終了
    IEnumerator CheckSuvivorCnt()
    {
        while (true)
        {
            List<GameObject> players = GetPlayerCount();
            int playerCnt = players.Count;
            playerCntText.text = $"{playerCnt} /   {instantiatePlayer.GetCurrentPlayerCount()}";
            infoText.text      = $"生存人数 /  参加人数";

            if (playerCnt <= 1)
            {
                Debug.Log("生存人数が１人になったのでゲームを終了します");
                readyButton.SetActive(true);
                playerReady.SetReady(false);
                setPlayerBool.SetPlayerCondition();
                StartCoroutine(WaitPlayersReady());
                yield break;
            }
            yield return null;
        }
    }

    // 生存者リストを返す
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
