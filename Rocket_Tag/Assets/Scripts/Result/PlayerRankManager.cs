using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerRankManager : MonoBehaviour
{
    public int playerRank;

    void Start()
    {
        //�v���C���[�̏��ʂ�ݒ�
        playerRank = Random.Range(0, 10);//��

        //���ʂ��J�X�^���v���p�e�B�ɕۑ�
        Hashtable playerProperties = new Hashtable();
        playerProperties.Add("PlayerRank", playerRank);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }
}
