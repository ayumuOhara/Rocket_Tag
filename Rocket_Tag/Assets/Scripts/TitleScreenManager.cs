using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // �Q�[����ʂւ̑J��
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Test_Takeshita"); // �Q�[����ʂ̃V�[����
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