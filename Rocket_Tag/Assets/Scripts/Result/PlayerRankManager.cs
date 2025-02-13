using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerRankManager : MonoBehaviour
{
    [SerializeField] InstantiatePlayer instantiatePlayer;
    public int playerRank;

    public void SetPlayerRank()
    {
        //�v���C���[�̏��ʂ�ݒ�
        playerRank = instantiatePlayer.GetCurrentPlayerCount();

        //���ʂ��J�X�^���v���p�e�B�ɕۑ�
        Hashtable playerProperties = new Hashtable();
        playerProperties.Add("PlayerRank", playerRank);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }
}
