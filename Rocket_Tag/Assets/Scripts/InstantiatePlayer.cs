using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//PUNのコールバックを受け取れるようにする為のMonoBehaviourPunCallbacks
public class InstantiatePlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject _camera;

    void Start()
    {
        //マスターサーバーに接続
        PhotonNetwork.ConnectUsingSettings();
    }

    //マスターサーバーに接続成功した時に呼ばれる
    public override void OnConnectedToMaster()
    {
        //Roomという名前のルームを作成する、既存の場合は参加する
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    //ルームへの接続が成功したら呼ばれる
    public override void OnJoinedRoom()
    {
        //Playerを生成する座量をランダムに決める
        var position = new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));

        //Resourcesフォルダから"Player/PlayerCamera"を探してきてそれを生成
        GameObject player = PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        _camera.SetActive(true);
    }
}