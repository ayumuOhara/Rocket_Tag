using UnityEngine;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private List<AudioClip> bgmClips;

    private BGMType currentBGM;

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
        if(currentBGM == bgmType && bgmAudioSource.isPlaying)
        {
            return; // ���łɍĐ����Ȃ�ύX���Ȃ�
        }
       
        currentBGM = bgmType;

        // ���݂�BGM���~�߂Ă���A�V����BGM���Đ�
        StopBGM();
        PlayBGMFromList((int)bgmType);

        //int index = (int)bgmType;  // Enum����C���f�b�N�X�֕ϊ�
        //PlayBGMFromList(index);
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = null;
    }

    // ���X�g����BGM���Đ�
    private void PlayBGMFromList(int index)
    {
        if (index >= 0 && index < bgmClips.Count)
        {
            bgmAudioSource.clip = bgmClips[index];
            bgmAudioSource.Play();
            //bgmAudioSource.PlayOneShot(bgmClips[index]);
            Debug.Log("BGM���Đ�");
        }
        else
        {
            Debug.LogWarning("�w�肳�ꂽ�C���f�b�N�X�ɊY������BGM������܂���");
        }
    }
}
