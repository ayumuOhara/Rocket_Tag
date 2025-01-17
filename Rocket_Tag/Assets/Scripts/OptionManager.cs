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
    }

    //�I�v�V������ʂ��\��
    public void HideOptionPanel()
    {
        optionPanel.SetActive(false);
    }
}