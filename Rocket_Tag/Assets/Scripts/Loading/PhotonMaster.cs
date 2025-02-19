using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // �V�[���J�ڂɕK�v
public class PhotonMaster : MonoBehaviourPunCallbacks
{
    public Text statusText;
    public Image cover;
    private const int MAX_PLAYER_PER_ROOM = 4;
    bool isMatching = false; // �}�b�`���O�����ǂ���
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
        if (PhotonNetwork.IsConnected && !isMatching)
        {
            isMatching = true; // �}�b�`���O���t���O�𗧂Ă�
            PhotonNetwork.JoinRandomRoom();
            statusText.text = "���[����T���Ă��܂�...";
        }
    }

    // �}�b�`���O�L�����Z���̃��\�b�h
    public void CancelMatching()
    {
        if (isMatching)
        {
            isMatching = false; // �}�b�`���O���t���O������
            statusText.text = "�}�b�`���O���L�����Z�����܂����B";
            PhotonNetwork.LeaveRoom(); // ���[������ޏo
        }
        else
        {
            statusText.text = "���r�[�ɖ߂�܂��B";
        }
            SceneManager.LoadScene("Lobby"); // �߂肽���V�[���ɑJ��
    }

    //Photon�̃R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        Debug.Log("�}�X�^�[�Ɍq���܂����B");
        statusText.text = "�T�[�o�[�ɐڑ����܂����B";
        RemoveTheCover();
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
            statusText.text = $"�ΐ푊���҂��Ă��܂��B\n�@�@�@�@�@�@�@�@({playerCount}/{MAX_PLAYER_PER_ROOM})";
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

    /*
     public override void OnLeftRoom()
    {
        Debug.Log("���[����ޏo���܂���");
        SceneManager.LoadScene("�O�̃V�[����"); // �ޏo��ɑO�̃V�[���ɖ߂�
    }
    */

    public void RemoveTheCover()
    {
        cover.gameObject.SetActive(false);
    }    
}