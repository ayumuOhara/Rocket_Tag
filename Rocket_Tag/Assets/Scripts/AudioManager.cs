using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーン移行しても破棄しない
        }
        else
        {
            Destroy(gameObject); // 既にある場合は削除（重複防止）
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    // 🔊 効果音を再生
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
