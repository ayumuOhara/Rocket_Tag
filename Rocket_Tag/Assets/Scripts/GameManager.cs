using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> playerList = new List<GameObject>(); // 参加するプレイヤーのリスト
    private const int JOIN_CNT_MIN = 4;  // 参加人数の最小値
    private bool isStart = false;        // ゲームが始まっているか

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

    // プレイヤーが指定の人数以上参加しているかを確認
    public void CheckJoinedPlayer()
    {
        int playerCount = GetCurrentPlayerCount(); // カスタムプロパティから人数を取得

        if (playerCount >= JOIN_CNT_MIN) // 必要人数を満たしているか
        {
            Debug.Log("プレイヤーが揃ったのでゲームを開始します");
            isStart = true;
            ChooseRocketPlayer();
        }
        else
        {
            Debug.Log($"プレイヤーが揃うまで待機します ({playerCount}/{JOIN_CNT_MIN})");
        }
    }

    // 参加しているプレイヤーから1人を選び、ロケットを付与
    void ChooseRocketPlayer()
    {
        if (playerList.Count == 0)
        {
            Debug.LogWarning("プレイヤーが見つかりません。");
            return;
        }

        // ランダムにプレイヤーを抽選
        int rnd = Random.Range(0, playerList.Count);
        GameObject selectedPlayer = playerList[rnd];

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

    // 生存人数を確認
    void CheckSuvivorCnt()
    {
        if (playerList.Count == 1)
        {
            Debug.Log("生存人数が1人になったのでゲームを終了します");
            isStart = false;
        }
        else
        {
            Debug.Log($"生存人数が{playerList.Count}人です。まだ終了条件を満たしていません。");
        }
    }

    // カスタムプロパティを利用して現在のプレイヤー人数を取得
    private int GetCurrentPlayerCount()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerCount", out object count))
        {
            return (int)count;
        }
        return 0;
    }

    // プレイヤーがルームに参加した際に呼ばれる
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCount(1);
        CheckJoinedPlayer(); // 新規プレイヤー参加時に人数をチェック
    }

    // プレイヤーがルームから退出した際に呼ばれる
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount(-1);
        CheckJoinedPlayer(); // プレイヤー退出時に人数を再チェック
    }

    // プレイヤー人数を更新するメソッド
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

            Debug.Log($"現在のプレイヤー数: {currentCount}");
        }
    }
}
