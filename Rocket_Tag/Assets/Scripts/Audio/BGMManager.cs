using UnityEngine;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private List<AudioClip> bgmClips;

    private BGMType currentBGM;

    // EnumによるBGM管理
    public enum BGMType
    {
        BGM_1,  // タイトル時のBGM
        BGM_2,  // ロビー時のBGM
        BGM_3   // インゲーム時のBGM
    }

    // BGM再生メソッド
    public void PlayBGM(BGMType bgmType)
    {
        if(currentBGM == bgmType && bgmAudioSource.isPlaying)
        {
            return; // すでに再生中なら変更しない
        }
       
        currentBGM = bgmType;

        // 現在のBGMを止めてから、新しいBGMを再生
        StopBGM();
        PlayBGMFromList((int)bgmType);

        //int index = (int)bgmType;  // Enumからインデックスへ変換
        //PlayBGMFromList(index);
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = null;
    }

    // リストからBGMを再生
    private void PlayBGMFromList(int index)
    {
        if (index >= 0 && index < bgmClips.Count)
        {
            bgmAudioSource.clip = bgmClips[index];
            bgmAudioSource.Play();
            //bgmAudioSource.PlayOneShot(bgmClips[index]);
            Debug.Log("BGMを再生");
        }
        else
        {
            Debug.LogWarning("指定されたインデックスに該当するBGMがありません");
        }
    }
}
