using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InstantiatePlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] Debuger debuger;

    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject waitCamera;
    [SerializeField] GameManager gameManager;
    [SerializeField] Transform respawnPoint;

    void Start()
    {
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
        Application.targetFrameRate = 240;

        // カスタムプロパティでプレイヤー人数を初期化または更新
        UpdatePlayerCount(1);

        // プレイヤーをリスポーン地点に生成
        GameObject player = PhotonNetwork.Instantiate("Player", respawnPoint.position, Quaternion.identity);

        debuger.SetComponents(player);

        // 入室したプレイヤーのPlayerControllerコンポーネントをGameManagerに渡す
        gameManager.playerController = player.GetComponent<PlayerController>();
        gameManager.setPlayerBool    = player.GetComponent<SetPlayerBool>();

        playerCamera.SetActive(true);
        waitCamera.SetActive(false);

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
