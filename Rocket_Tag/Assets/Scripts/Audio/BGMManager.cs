using UnityEngine;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private List<AudioClip> bgmClips;

    // Enum�ɂ��BGM�Ǘ�
    public enum BGMType
    {
        BGM_1,  // �^�C�g������BGM
        BGM_2,  // ���r�[����BGM
        BGM_3   // �C���Q�[������BGM
    }

    // BGM�Đ����\�b�h
    public void PlayBGM(BGMType bgmType)
    {
        int index = (int)bgmType;  // Enum����C���f�b�N�X�֕ϊ�
        PlayBGMFromList(index);
    }

    // ���X�g����BGM���Đ�
    private void PlayBGMFromList(int index)
    {
        if (index >= 0 && index < bgmClips.Count)
        {
            bgmAudioSource.PlayOneShot(bgmClips[index]);
        }
        else
        {
            Debug.LogWarning("�w�肳�ꂽ�C���f�b�N�X�ɊY������BGM������܂���");
        }
    }
}
