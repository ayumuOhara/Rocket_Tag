using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // ゲーム画面への遷移
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Test_Takeshita"); // ゲーム画面のシーン名
    }

    // オプション画面への遷移
    public void LoadOptionScene()
    {
        SceneManager.LoadScene(""); // オプション画面のシーン名
    }

    //ゲーム終了
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("ゲームを終了しました");
    }
}