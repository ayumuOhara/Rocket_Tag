using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade_In : MonoBehaviour
{
    private Image fadeImage; // Imageコンポーネント
    public float fadeInTime = 2.0f; // フェードイン時間

    private void Start()
    {
        // Imageコンポーネントを取得
        fadeImage = GetComponent<Image>();

        if (fadeImage == null)
        {
           // Debug.LogError("Image コンポーネントが見つかりません！");
            return;
        }
    }

    public IEnumerator Color_FadeIn()
    {
        fadeImage.color = new Color(0, 0, 0, 1); // 初期状態は不透明
        float elapsedTime = 0f;

        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeInTime);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0); // 完全に透明

        // フェード処理が完了したら、このスクリプトと Fade_Out スクリプトを無効化
        GetComponent<Fade_Out>().enabled = false;
        this.enabled = false;
    }
}
