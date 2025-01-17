using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // ���r�[��ʂւ̑J��
    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby"); // ���r�[��ʂ̃V�[����
    }

    // �I�v�V������ʂւ̑J��
    public void LoadOptionScene()
    {
        SceneManager.LoadScene(""); // �I�v�V������ʂ̃V�[����
    }

    //�Q�[���I��
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("�Q�[�����I�����܂���");
    }
}