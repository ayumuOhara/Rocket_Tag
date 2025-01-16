using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InstantiatePlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject waitCamera;
    [SerializeField] GameManager gameManager;

    void Start()
    {
        Application.targetFrameRate = 60;

        // マスターサーバーに接続
        PhotonNetwork.ConnectUsingSettings();
    }

    // マスターサーバーに接続成功した時に呼ばれる
    public override void OnConnectedToMaster()
    {
        // ルームの作成または参加
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // ルーム作成時または参加時に呼ばれる
    public override void OnJoinedRoom()
    {
        // カスタムプロパティでプレイヤー人数を初期化または更新
        UpdatePlayerCount(1);

        // プレイヤーをランダムな位置に生成
        var position = new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));
        GameObject player = PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        playerCamera.SetActive(true);
        waitCamera.SetActive(false);

        gameManager.CheckJoinedPlayer();
        Debug.Log("プレイヤーがルームに参加しました。");
    }

    // プレイヤーがルームから退出した場合に呼ばれる
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // プレイヤー人数を減らす
        UpdatePlayerCount(-1);
        Debug.Log($"プレイヤーが退出しました: {otherPlayer.NickName}");
    }

    // プレイヤー人数を更新するメソッド
    private void UpdatePlayerCount(int delta)
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            // 現在の人数を取得
            int currentCount = 0;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerCount", out object count))
            {
                currentCount = (int)count;
            }

            // 人数を更新
            currentCount += delta;

            // カスタムプロパティに保存
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["PlayerCount"] = currentCount;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            Debug.Log($"現在のプレイヤー数: {currentCount}");
        }
    }

    // カスタムプロパティから現在のプレイヤー人数を取得するメソッド
    public int GetCurrentPlayerCount()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerCount", out object count))
        {
            return (int)count;
        }
        return 0;
    }
}
