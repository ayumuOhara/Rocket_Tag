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
    [SerializeField] GameObject inputPlayerName;

#if true
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
        // カスタムプロパティでプレイヤー人数を初期化または更新
        UpdatePlayerCount();
        Application.targetFrameRate = 60;

        // プレイヤーをリスポーン地点に生成
        GameObject player = PhotonNetwork.Instantiate("Player", respawnPoint.position, Quaternion.identity);

        debuger.SetComponents(player);

        // 入室したプレイヤーのPlayerControllerコンポーネントをGameManagerに渡す
        gameManager.playerController = player.GetComponent<PlayerController>();
        gameManager.setPlayerBool = player.GetComponent<SetPlayerBool>();

        inputPlayerName.SetActive(true);
        playerCamera.SetActive(true);
        waitCamera.SetActive(false);
    }
#endif

#if false
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UpdatePlayerCount();
        }

        Application.targetFrameRate = 60;
        // プレイヤーをリスポーン地点に生成
        GameObject player = PhotonNetwork.Instantiate("Player", respawnPoint.position, Quaternion.identity);

        debuger.SetComponents(player);

        // 入室したプレイヤーのPlayerControllerコンポーネントをGameManagerに渡す
        gameManager.playerController = player.GetComponent<PlayerController>();
        gameManager.setPlayerBool = player.GetComponent<SetPlayerBool>();

        inputPlayerName.SetActive(true);
        playerCamera.SetActive(true);
        waitCamera.SetActive(false);
    }
#endif

    // プレイヤーがルームから退出したとき
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount();
        Debug.Log($"プレイヤーが退出しました: {otherPlayer.NickName}");
    }

    // プレイヤー人数を更新するメソッド
    private void UpdatePlayerCount()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            // 現在の人数を取得
            int currentCount = PhotonNetwork.CurrentRoom.PlayerCount;

            // カスタムプロパティに保存
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["PlayerCount"] = currentCount; // PhotonNetwork.CurrentRoom.PlayerCount で直接更新
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            Debug.Log($"現在のプレイヤー数: {currentCount}");
        }
    }

    // カスタムプロパティが更新されたときに呼ばれる
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("PlayerCount"))
        {
            Debug.Log($"プレイヤー数が更新されました: {changedProps["PlayerCount"]}");
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
