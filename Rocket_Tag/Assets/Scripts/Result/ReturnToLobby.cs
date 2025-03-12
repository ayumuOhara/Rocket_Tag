using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToLobby : MonoBehaviourPunCallbacks
{
    public void GoToLobby()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Lobby"); // ロビーのシーン名を指定
    }
}