using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToLobby : MonoBehaviour
{
    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby"); // ロビーのシーン名を指定
    }
}