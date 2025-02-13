using UnityEngine;

public class BounceUp : MonoBehaviour
{
    [Header("バウンス設定")]
    [SerializeField] private float bounceForce = 5f; // はじく力

    [Header("サウンド設定")]
    [SerializeField] private AudioSource audioSource; // 音を再生する AudioSource
    [SerializeField] private string playerTag = "Player"; // プレイヤーのタグ（Inspector で変更可能）

    void Start()
    {
        // AudioSource が設定されていない場合、自動で追加
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // デフォルトでは再生しない
            audioSource.enabled = false; // 最初は無効化しておく
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトがプレイヤーか確認
        if (collision.gameObject.CompareTag(playerTag))
        {
            // プレイヤーの Rigidbody を取得
            Rigidbody playerRigidbody = collision.rigidbody;

            if (playerRigidbody != null)
            {
                // 上方向に力を加える
                Vector3 bounceDirection = Vector3.up;
                playerRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            }

            // AudioSource を有効化して音を鳴らす
            if (audioSource != null)
            {
                audioSource.enabled = true;  // AudioSource を ON にする
                if (!audioSource.isPlaying)  // すでに鳴っていなければ再生
                {
                    audioSource.Play();
                }
            }
        }
    }
}
