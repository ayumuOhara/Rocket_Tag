using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public PlayerController playerController;
    [SerializeField] InstantiatePlayer instantiatePlayer;
    [SerializeField] PlayerReady playerReady;
    [SerializeField] TextMeshProUGUI playerCntText;     // Ready完了しているプレイヤー数
    [SerializeField] GameObject[] ReadyUI;              // マッチ開始前のUI
    private const int JOIN_CNT_MIN = 2;       // 参加人数の最小値

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitPlayersReady());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // プレイヤーの準備完了を待つ
    IEnumerator WaitPlayersReady()
    {
        while (true)
        {
            int readyCount = GetReadyPlayerCount();
            playerCntText.text = $"{readyCount}/{JOIN_CNT_MIN}";

            if (CheckJoinedPlayer() && CheckAllPlayersReady())
            {
                StartGame();
                yield break;
            }

            yield return null;
        }
    }


    // プレイヤーが指定の人数以上参加しているか
    public bool CheckJoinedPlayer()
    {
        var currentCnt = instantiatePlayer.GetCurrentPlayerCount();

        if (currentCnt >= JOIN_CNT_MIN)
        {
            return true;
        }
        
        return false;
    }

    // 参加しているプレイヤー全員がReadyしたか
    bool CheckAllPlayersReady()
    {
        Player[] players = PhotonNetwork.PlayerList;

        // 全プレイヤーの「IsReady」フラグをチェック
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

    // ゲームスタート
    void StartGame()
    {
        Debug.Log("プレイヤーが揃ったのでゲームを開始します");
        ReadyUI[0].SetActive(false);
        ReadyUI[1].SetActive(false);
        ChooseRocketPlayer();
        CheckSuvivorCnt();
    }

    // 参加しているプレイヤーから１人を選び、ロケットを付与
    void ChooseRocketPlayer()
    {
        // "Player" タグを持つすべてのプレイヤーを取得
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("プレイヤーが見つかりません。");
            return;
        }

        // ランダムにプレイヤーを抽選
        int rnd = Random.Range(0, players.Length);
        GameObject selectedPlayer = players[rnd];

        // 抽選したプレイヤーの PhotonView を取得
        PhotonView targetPhotonView = selectedPlayer.GetComponent<PhotonView>();
        if (targetPhotonView != null)
        {
            // hasRocket を true に設定し、同期
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
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            if (player.Length <= 1)
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