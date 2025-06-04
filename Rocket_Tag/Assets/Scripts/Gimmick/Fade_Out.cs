using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade_Out : MonoBehaviour
{
    private Image fadeImage; // Imageコンポーネント
    public float waitTimeBeforeFadeIn = 2.0f; // フェードアウト後の待機時間
    public float fadeOutTime = 2.0f; // フェードアウト時間

    private void Start()
    {
        // Imageコンポーネントを取得
        fadeImage = GetComponent<Image>();

        if (fadeImage == null)
        {
           // Debug.LogError("Image コンポーネントが見つかりません！");
            return;
        }

        // フェードアウト開始
        StartCoroutine(Color_FadeOut());
    }

    public IEnumerator Color_FadeOut()
    {
        fadeImage.color = new Color(0, 0, 0, 0); // 初期状態は透明
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeOutTime);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1); // 完全に不透明

        // 指定秒数待機
        yield return new WaitForSeconds(waitTimeBeforeFadeIn);

        // フェードインを開始
        StartCoroutine(GetComponent<Fade_In>().Color_FadeIn());
    }
}
