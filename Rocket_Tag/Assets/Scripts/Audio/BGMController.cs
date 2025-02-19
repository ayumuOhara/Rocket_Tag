using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour
{
    [SerializeField] private BGMManager bgmManager;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //bgmManager.PlayBGM(BGMManager.BGMType.BGM_1);

        // 現在のシーンを取得して、BGMを再生する
        Scene currentScene = SceneManager.GetActiveScene();
        OnSceneLoaded(currentScene, LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("OnDestroy");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("シーン遷移: " + scene.name);

        bgmManager.StopBGM();

        switch (scene.name)
        {
            case "Title":
                bgmManager.PlayBGM(BGMManager.BGMType.BGM_1);
                break;
            case "Lobby":
                 bgmManager.PlayBGM(BGMManager.BGMType.BGM_2);
                break;
            case "Test_Takeshita":
                 bgmManager.PlayBGM(BGMManager.BGMType.BGM_3);
                break;
        }
    }
}
