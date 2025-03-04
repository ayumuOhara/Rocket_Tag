using UnityEngine;

public class FadeManager : MonoBehaviour
{
    //private static FadeManager instance; // シングルトンインスタンス
    private Fade_In fadeIn;
    private Fade_Out fadeOut;

    private void Awake()
    {
        //// すでに存在するインスタンスがあれば破棄
        //if (instance != null && instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //// インスタンスが存在しなければ、このオブジェクトを保持
        //instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 同じオブジェクトにアタッチされている Fade_In / Fade_Out を取得
        fadeIn = GetComponent<Fade_In>();
        fadeOut = GetComponent<Fade_Out>();

        if (fadeIn == null || fadeOut == null)
        {
            Debug.LogError("Fade_In または Fade_Out のスクリプトが見つかりません！");
        }
    }

    // フェードアウト→フェードインを開始する関数（外部から呼び出し可能）
    public void StartFadeSequence()
    {
        if (fadeIn != null && fadeOut != null)
        {
            // フェードスクリプトを有効化
            fadeIn.enabled = true;
            fadeOut.enabled = true;

            // フェードアウトを開始
            StartCoroutine(fadeOut.Color_FadeOut());
        }
    }
}
// FindFirstObjectByType<FadeManager>().StartFadeSequence(); //フェード呼び出し