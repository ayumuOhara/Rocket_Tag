using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // ゲーム画面への遷移
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Test_Shiromoto"); // ゲーム画面のシーン名
        AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //ボタンクリック音
    }

    // ロッカー画面への遷移
    public void LoadLockerScene()
    {
        SceneManager.LoadScene("Locker"); // ロッカー画面のシーン名
        //AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //ボタンクリック音
    }
}