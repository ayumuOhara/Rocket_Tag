using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Managers")]
    public BGMManager bgmManager;
    public SEManager seManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // シーンをまたいでも破棄しない
        }
        else
        {
            Destroy(gameObject);  // 既に存在する場合、重複を避けるために自分自身を破棄
        }
    }

    // SE再生
    public void PlaySE(SEManager.SEType seType)
    {
        seManager.PlaySE(seType);
    }
}