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
            DontDestroyOnLoad(gameObject);  // �V�[�����܂����ł��j�����Ȃ�
        }
        else
        {
            Destroy(gameObject);  // ���ɑ��݂���ꍇ�A�d��������邽�߂Ɏ������g��j��
        }
    }

    // SE�Đ�
    public void PlaySE(SEManager.SEType seType)
    {
        seManager.PlaySE(seType);
    }
}