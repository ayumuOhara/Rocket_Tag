using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // �Q�[����ʂւ̑J��
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Test_Takeshita"); // �Q�[����ʂ̃V�[����
    }

    // �V���b�v��ʂւ̑J��
    public void LoadShopScene()
    {
        SceneManager.LoadScene(""); // �V���b�v��ʂ̃V�[����
    }

    // �`���[�g���A����ʂւ̑J��
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene(""); // �`���[�g���A����ʂ̃V�[����
    }

    // �^�C�g����ʂւ̑J��
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("Title"); // �^�C�g����ʂ̃V�[����
    }

    // ���b�J�[��ʂւ̑J��
    public void LoadLockerScene()
    {
        SceneManager.LoadScene("Locker"); // ���b�J�[��ʂ̃V�[����
    }
}