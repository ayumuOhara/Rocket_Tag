using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SEManager : MonoBehaviour
{
    [SerializeField] private AudioSource seAudioSource;
    [SerializeField] private List<AudioClip> seClips;
    [SerializeField] private Slider seSlider;
    [SerializeField] private SESetting seSetting;

    private void Start()
    {
        if (seSlider != null)
        {
            seSlider.value = seSetting.volume;
            seSlider.onValueChanged.AddListener(OnVolumeChenged);
        }
    }

    private void Update()
    {
        seAudioSource.volume = seSetting.volume;
    }

    void OnVolumeChenged(float value)
    {
        seSetting.volume = value;
    }

    // EnumによるSE管理
    public enum SEType
    {
        Button_Click,  // ボタンをクリックしたときの音
        Rocket_Set,    // ロケットを押し付けたときの音
        Bumper,        // ジャンプ台の音
        Landing,       // 着地したときの音
        Collision_Dash_1,  // ぶつかりダッシュ使用時の音
        Event_warp,    // 位置入れ替えの効果音
        Event_ink,　　 // インクが画面に飛び散る効果音
        Page,　　　　　// ページをめくる効果音
        win            // 勝利画面の音
    }

    // SE再生メソッド
    public void PlaySE(SEType seType)
    {
        int index = (int)seType;  // Enumからインデックスへ変換
        PlaySEFromList(index);
    }

    // リストからSEを再生
    private void PlaySEFromList(int index)
    {
        if (index >= 0 && index < seClips.Count)
        {
            seAudioSource.PlayOneShot(seClips[index]);
        }
        else
        {
            Debug.LogWarning("指定されたインデックスに該当するSEがありません");
        }
    }
}
