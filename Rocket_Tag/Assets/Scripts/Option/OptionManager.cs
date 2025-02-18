using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    [SerializeField] GameObject optionPanel;

    //�I�v�V������ʂ�\��
    public void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
        AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //�{�^���N���b�N��
    }

    //�I�v�V������ʂ��\��
    public void HideOptionPanel()
    {
        optionPanel.SetActive(false);
    }
    //�Q�[���I��
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
        Application.Quit();//�Q�[���v���C�I��
#endif

        Debug.Log("�Q�[�����I�����܂���");
    }
}