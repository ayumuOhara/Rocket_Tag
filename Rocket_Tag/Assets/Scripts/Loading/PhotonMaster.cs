using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class PhotonMaster : MonoBehaviourPunCallbacks
{
    public Text statusText;
    private const int MAX_PLAYER_PER_ROOM = 4;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called before the first frame update
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        statusText.text = "�T�[�o�[�ɐڑ����ł��B\n���΂炭���҂����������B";
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    //������{�^���ɂ���
    public void FindOponent()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    //Photon�̃R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        Debug.Log("�}�X�^�[�Ɍq���܂����B");
        statusText.text = "�T�[�o�[�ɐڑ����܂����B";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"{cause}�̗��R�Ōq���܂���ł����B");
        statusText.text = "�G���[���������܂����B";
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("���[�����쐬���܂��B");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MAX_PLAYER_PER_ROOM });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("���[���ɎQ�����܂���");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount != MAX_PLAYER_PER_ROOM)
        {
            statusText.text = "�ΐ푊���҂��Ă��܂��B";
        }
        else
        {
            statusText.text = "�ΐ푊�肪�����܂����B�o�g���V�[���Ɉړ����܂��B";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == MAX_PLAYER_PER_ROOM)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                statusText.text = "�ΐ푊�肪�����܂����B�o�g���V�[���Ɉړ����܂��B";
                PhotonNetwork.LoadLevel("Test_Takeshita");
            }
        }
    }
}