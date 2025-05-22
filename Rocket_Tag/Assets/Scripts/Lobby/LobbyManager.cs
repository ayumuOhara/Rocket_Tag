using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // ゲーム画面への遷移
    public void LoadGameScene()
    {
        SceneFadeManager.Instance.LoadScene("Test_Shiromoto", 0.2f,0.2f);
        AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //ボタンクリック音
    }

    // ロッカー画面への遷移
    public void LoadLockerScene()
    {
        SceneFadeManager.Instance.LoadScene("Locker", 0.2f, 0.3f);
        AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //ボタンクリック音
    }
}