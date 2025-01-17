using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // ゲーム画面への遷移
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Test_Takeshita"); // ゲーム画面のシーン名
    }

    // ショップ画面への遷移
    public void LoadShopScene()
    {
        SceneManager.LoadScene(""); // ショップ画面のシーン名
    }

    // チュートリアル画面への遷移
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene(""); // チュートリアル画面のシーン名
    }

    // タイトル画面への遷移
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("Title"); // タイトル画面のシーン名
    }

    // ロッカー画面への遷移
    public void LoadLockerScene()
    {
        SceneManager.LoadScene("Locker"); // ロッカー画面のシーン名
    }
}