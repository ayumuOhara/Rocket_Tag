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
        AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //ボタンクリック音
    }

    //オプション画面を非表示
    public void HideOptionPanel()
    {
        optionPanel.SetActive(false);
    }
    //ゲーム終了
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
        Application.Quit();//ゲームプレイ終了
#endif

        Debug.Log("ゲームを終了しました");
    }
}