using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public PlayerController playerController;
    [SerializeField] InstantiatePlayer instantiatePlayer;
    [SerializeField] PlayerReady playerReady;
    [SerializeField] TextMeshProUGUI playerCntText;     // Ready完了しているプレイヤー数
    [SerializeField] GameObject[] ReadyUI;              // マッチ開始前のUI
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
            playerCntText.text = $"{readyCount}/{instantiatePlayer.GetCurrentPlayerCount()}";

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
        ReadyUI[0].SetActive(false);
        ReadyUI[1].SetActive(false);

        // マスタークライアントのみロケット付与処理を実行
        if (PhotonNetwork.IsMasterClient)
        {
            ChooseRocketPlayer();
        }

        StartCoroutine(CheckSuvivorCnt());
    }

    // 参加しているプレイヤーから１人を選び、ロケットを付与
    void ChooseRocketPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("プレイヤーが見つかりません。");
            return;
        }

        int rnd = Random.Range(0, players.Length);
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
            GameObject[] suvivor = GameObject.FindGameObjectsWithTag("Player");
            var suvivorCnt = suvivor.Length;

            foreach (var player in suvivor)
            {                
                PlayerController pc = player.GetComponent<PlayerController>();
                if(pc.isDead)
                {
                    suvivorCnt--;
                }
            }

            if (suvivorCnt <= 1)
            {
                Debug.Log("生存人数が１人になったのでゲームを終了します");
                ReadyUI[0].SetActive(true);
                ReadyUI[1].SetActive(true);
                playerReady.SetReady(false);
                playerController.SetPlayerCondition();
                yield break;
            }
            yield return null;
        }
    }
}
