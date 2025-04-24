using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private List<AudioClip> bgmClips;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private BGMSetting bgmSetting;

    private BGMType currentBGM;

    private void Start()
    {
        if(bgmSlider != null)
        {
            bgmSlider.value = bgmSetting.volume;
            bgmSlider.onValueChanged.AddListener(OnVolumeChenged);
        }
    }

    private void Update()
    {
        bgmAudioSource.volume = bgmSetting.volume;
    }

    void OnVolumeChenged(float value)
    {
        bgmSetting.volume = value; 
    }

    // EnumによるBGM管理
    public enum BGMType
    {
        BGM_1,  // タイトル時のBGM
        BGM_2,  // ロビー時のBGM
        BGM_3,  // インゲーム時のBGM
        BGM_4,  // インゲーム時のBGM
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
        }
        else
        {
            Debug.LogWarning("指定されたインデックスに該当するBGMがありません");
        }
    }
}
