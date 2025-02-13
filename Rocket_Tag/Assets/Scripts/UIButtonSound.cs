using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip buttonSound; // Inspector で設定する音

    private void Start()
    {
        Button button = GetComponent<Button>(); // UI ボタンを取得
        if (button != null)
        {
            button.onClick.AddListener(PlayButtonSound); // ボタンが押されたら音を鳴らす
        }
    }

    private void PlayButtonSound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(buttonSound); // AudioManager で音を再生
        }
    }
}
