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

    // BGM�Đ�
    public void PlayBGM(BGMManager.BGMType bgmType)
    {
        bgmManager.PlayBGM(bgmType);
    }

    // SE�Đ�
    public void PlaySE(SEManager.SEType seType)
    {
        seManager.PlaySE(seType);
    }
}

// BGM�Đ�
// AudioManager.Instance.PlayBGM(BGMManager.BGMType.BGM_1);

// ���ʉ��Đ�
// AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click);