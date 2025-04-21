using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    public static OptionManager Instance;

    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject optionButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // シーンをまたいでも破棄しない
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);  // 既に存在する場合、重複を避けるために自分自身を破棄
        }
    }

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
        if(SceneManager.GetActiveScene().name == "PlayScene")
        {
            Cursor.visible = false;       //マウスカーソルを非表示
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //オプションボタンの表示
        if (scene.name == "Lobby" || scene.name == "Locker")
        {
            optionButton.SetActive(true);
        }
        else
        {
            optionButton.SetActive(false);
        }
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