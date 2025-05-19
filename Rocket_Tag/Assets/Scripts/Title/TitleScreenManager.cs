using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // ロビー画面への遷移
    public void LoadLobbyScene()
    {
        SceneFadeManager.Instance.LoadScene("Lobby", 0.1f, 0.2f);
        AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //ボタンクリック音
        //FadeManager fadeManager = FindFirstObjectByType<FadeManager>();
    }
}