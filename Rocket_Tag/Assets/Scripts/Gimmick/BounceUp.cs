using UnityEngine;

public class BounceUp : MonoBehaviour
{
    [Header("バウンス設定")]
    [SerializeField] private float bounceForce = 5f; // はじく力
    [SerializeField] private string playerTag = "Player"; // プレイヤーのタグ（Inspector で変更可能）

    private void OnTriggerEnter(Collider other)
    {
        // 衝突したオブジェクトがプレイヤーか確認
        if (other.CompareTag(playerTag))
        {
            // プレイヤーの Rigidbody を取得
            Rigidbody playerRigidbody = other.attachedRigidbody;

            if (playerRigidbody != null)
            {
                // 上方向に力を加える
                Vector3 bounceDirection = Vector3.up;
                playerRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);

                // ジャンプ台のSEを再生
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySE(SEManager.SEType.Bumper);
                }
                else
                {
                    Debug.LogWarning("AudioManager.Instance が null です。");
                }
            }
        }
    }
}
