using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToLobby : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        AudioManager.Instance.PlaySE(SEManager.SEType.Win); //リザルトSE
    }
    public void GoToLobby()
    {
        Cursor.visible = true;
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Lobby"); // ロビーのシーン名を指定
    }
}