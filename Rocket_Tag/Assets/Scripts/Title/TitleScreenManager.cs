using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // ロビー画面への遷移
    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby"); // ロビー画面のシーン名
        AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //ボタンクリック音
    }
}