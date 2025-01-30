using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject tutorialPanel;

    //�`���[�g���A����ʂ�\��
    public void ShowTutorialPanel()
    {
        tutorialPanel.SetActive(true);
    }

    //�`���[�g���A����ʂ��\��
    public void HideTutorialPanel()
    {
        tutorialPanel.SetActive(false);
    }
}