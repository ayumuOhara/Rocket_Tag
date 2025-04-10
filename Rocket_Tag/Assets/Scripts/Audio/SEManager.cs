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
        Dash,          // 走った時の音
        Rocket_Set,    // ロケットを押し付けたときの音
        Skill_Use,     // スキル使用時の音
        Bumper,        // ジャンプ台の音
        Landing,       // 着地したときの音
        Smash_Punch,   // スマッシュパンチの音
        Collision_Dash_1,  // ぶつかりダッシュ使用時の音
        Collision_Dash_2,  // ぶつかりダッシュ衝突時の音
        Sticky_Zone,   // ねばねばゾーン展開時の音
        Pull_Hook_1,   // 引き寄せフック投擲時の音
        Pull_Hook_2    // 引き寄せフック引き寄せる時の音
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
