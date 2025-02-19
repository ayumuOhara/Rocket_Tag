using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // シーン遷移に必要
public class PhotonMaster : MonoBehaviourPunCallbacks
{
    public Text statusText;
    public Image cover;
    private const int MAX_PLAYER_PER_ROOM = 4;
    bool isMatching = false; // マッチング中かどうか
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called before the first frame update
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        statusText.text = "サーバーに接続中です。\nしばらくお待ちください。";
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    //これをボタンにつける
    public void FindOponent()
    {
        if (PhotonNetwork.IsConnected && !isMatching)
        {
            isMatching = true; // マッチング中フラグを立てる
            PhotonNetwork.JoinRandomRoom();
            statusText.text = "ルームを探しています...";
        }
    }

    // マッチングキャンセルのメソッド
    public void CancelMatching()
    {
        if (isMatching)
        {
            isMatching = false; // マッチング中フラグを解除
            statusText.text = "マッチングをキャンセルしました。";
            PhotonNetwork.LeaveRoom(); // ルームから退出
        }
        else
        {
            statusText.text = "ロビーに戻ります。";
        }
            SceneManager.LoadScene("Lobby"); // 戻りたいシーンに遷移
    }

    //Photonのコールバック
    public override void OnConnectedToMaster()
    {
        Debug.Log("マスターに繋ぎました。");
        statusText.text = "サーバーに接続しました。";
        RemoveTheCover();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"{cause}の理由で繋げませんでした。");
        statusText.text = "エラーが発生しました。";
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("ルームを作成します。");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MAX_PLAYER_PER_ROOM });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに参加しました");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount != MAX_PLAYER_PER_ROOM)
        {
            statusText.text = $"対戦相手を待っています。\n　　　　　　　　({playerCount}/{MAX_PLAYER_PER_ROOM})";
        }
        else
        {
            statusText.text = "対戦相手が揃いました。バトルシーンに移動します。";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == MAX_PLAYER_PER_ROOM)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                statusText.text = "対戦相手が揃いました。バトルシーンに移動します。";
                PhotonNetwork.LoadLevel("Test_Takeshita");
            }
        }
    }

    /*
     public override void OnLeftRoom()
    {
        Debug.Log("ルームを退出しました");
        SceneManager.LoadScene("前のシーン名"); // 退出後に前のシーンに戻る
    }
    */

    public void RemoveTheCover()
    {
        cover.gameObject.SetActive(false);
    }    
}