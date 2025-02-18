using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    // AudioManagerのインスタンスを保持するプロパティ
    public static AudioManager Instance { get; private set; }

    // 効果音用のAudioSourceコンポーネント
    [SerializeField] private AudioSource seAudioSource;

    // 効果音のAudioClipを管理するList
    [SerializeField] private List<AudioClip> seClips;

    private void Awake()
    {
        // AudioManagerがまだ存在しない場合はインスタンスを設定
        if (Instance == null)
        {
            Instance = this;
            // シーンをまたいでもAudioManagerが破棄されないようにする
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでにインスタンスが存在する場合は、重複を避けるために自分自身を破棄
            Destroy(gameObject);
        }
    }

    // SEの種類をEnumで定義
    public enum SEType
    {
        Bottun_Click,       // ボタンをクリックしたときの音
        Dash,               // 走った時の音
        Rocket_Set,         // ロケットを押し付けたときの音
        Skill_use,          // スキル使用時の音
        Bumper,             // ジャンプ台の音
        Landing,            // 着地したときの音
        Smash_Punch,        // スマッシュパンチの音
        Collision_Dash_1,   // ぶつかりダッシュ使用時の音
        Collision_Dash_2,   // ぶつかりダッシュ衝突時の音
        Sticky_Zone,        // ねばねばゾーン展開時の音
    }

    // SEを再生するメソッド
    public void PlaySE(SEType seType)
    {
        // 引数で受け取ったSETypeに基づき、適切な効果音を再生
        switch (seType)
        {
            case SEType.Bottun_Click:
                PlaySEFromList(0); // ボタンをクリックしたときの音（インデックス0）
                break;
            case SEType.Dash:
                PlaySEFromList(1); // 走った時の音（インデックス1）
                break;
            case SEType.Rocket_Set:
                PlaySEFromList(2); // ロケットを押し付けたときの音（インデックス2）
                break;
            case SEType.Skill_use:
                PlaySEFromList(3); // スキル使用時の音（インデックス3）
                break;
            case SEType.Bumper:
                PlaySEFromList(4); // ジャンプ台の音（インデックス4）
                break;
            case SEType.Landing:
                PlaySEFromList(5); // 着地したときの音（インデックス5）
                break;
            case SEType.Smash_Punch:
                PlaySEFromList(6); // スマッシュパンチの音（インデックス6）
                break;
            case SEType.Collision_Dash_1:
                PlaySEFromList(7); // ぶつかりダッシュ使用時の音（インデックス7）
                break;
            case SEType.Collision_Dash_2:
                PlaySEFromList(8); // ぶつかりダッシュ衝突時の音（インデックス8）
                break;
            case SEType.Sticky_Zone:
                PlaySEFromList(9); // ねばねばゾーン展開時の音（インデックス9）
                break;
            default:
                Debug.LogWarning("指定されたSEが見つかりません");
                break;
        }
    }

    // Listから指定されたインデックスの効果音を再生するメソッド
    private void PlaySEFromList(int index)
    {
        // インデックスがリスト内の範囲かを確認
        if (index >= 0 && index < seClips.Count)
        {
            // AudioSourceで指定した効果音を再生
            seAudioSource.PlayOneShot(seClips[index]);
        }
        else
        {
            // 範囲外のインデックスが指定された場合の警告
            Debug.LogWarning("指定されたインデックスに該当する効果音がありません");
        }
    }
}
