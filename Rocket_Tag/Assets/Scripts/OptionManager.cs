using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    [SerializeField] GameObject optionPanel;

    //オプション画面を表示
    public void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
    }

    //オプション画面を非表示
    public void HideOptionPanel()
    {
        optionPanel.SetActive(false);
    }
}