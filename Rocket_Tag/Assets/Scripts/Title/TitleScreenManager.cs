using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // ���r�[��ʂւ̑J��
    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby"); // ���r�[��ʂ̃V�[����
    }
}