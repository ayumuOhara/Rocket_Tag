using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    [SerializeField] GameObject optionPanel;

     void Update()
    {
        //オプション画面の表示切り替え
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(optionPanel.activeSelf)
            {
                HideOptionPanel();
            }
            else
            {
                ShowOptionPanel();
            }
        }
    }

    //オプション画面を表示
    public void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
        Cursor.visible = true;        //マウスカーソルを表示
        AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //ボタンクリック音
    }

    //オプション画面を非表示
    public void HideOptionPanel()
    {
        optionPanel.SetActive(false);
        Cursor.visible = false;      //マウスカーソルを非表示
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