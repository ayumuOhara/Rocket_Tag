using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject tutorialPanel;

    //チュートリアル画面を表示
    public void ShowTutorialPanel()
    {
        tutorialPanel.SetActive(true);
        AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //ボタンクリック音
    }

    //チュートリアル画面を非表示
    public void HideTutorialPanel()
    {
        tutorialPanel.SetActive(false);
    }
}