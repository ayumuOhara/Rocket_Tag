using UnityEngine;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private List<AudioClip> bgmClips;

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
        int index = (int)bgmType;  // Enumからインデックスへ変換
        PlayBGMFromList(index);
    }

    // リストからBGMを再生
    private void PlayBGMFromList(int index)
    {
        if (index >= 0 && index < bgmClips.Count)
        {
            bgmAudioSource.PlayOneShot(bgmClips[index]);
        }
        else
        {
            Debug.LogWarning("指定されたインデックスに該当するBGMがありません");
        }
    }
}
