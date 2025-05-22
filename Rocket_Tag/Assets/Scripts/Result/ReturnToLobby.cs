using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToLobby : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        AudioManager.Instance.PlaySE(SEManager.SEType.Win); //ƒŠƒUƒ‹ƒgSE
    }
    public void GoToLobby()
    {
        Cursor.visible = true;
        PhotonNetwork.Disconnect();
        SceneFadeManager.Instance.LoadScene("Lobby", 0.2f, 0.2f);
    }
}