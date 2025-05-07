using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour
{
    [SerializeField] private BGMManager bgmManager;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Œ»İ‚ÌƒV[ƒ“‚ğæ“¾‚µ‚ÄABGM‚ğÄ¶‚·‚é
        Scene currentScene = SceneManager.GetActiveScene();
        OnSceneLoaded(currentScene, LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bgmManager.StopBGM();

        switch (scene.name)
        {
            case "Title":
                bgmManager.PlayBGM(BGMManager.BGMType.BGM_1);
                break;
            case "Lobby":
                 bgmManager.PlayBGM(BGMManager.BGMType.BGM_2);
                break;
            case "Test_Shiromoto":
                bgmManager.PlayBGM(BGMManager.BGMType.BGM_2);
                break;
            case "Locker":
                bgmManager.PlayBGM(BGMManager.BGMType.BGM_2);
                break;
            case "PlayScene":
                bgmManager.PlayBGM(BGMManager.BGMType.BGM_3);
                break;
            case "Result":
                bgmManager.PlayBGM(BGMManager.BGMType.BGM_4);
                break;
        }
    }
}
