using UnityEngine;

public class BounceUp : MonoBehaviour
{
    [Header("バウンス設定")]
    [SerializeField] private float bounceForce = 5f; // はじく力
    [SerializeField] private string playerTag = "Player"; // プレイヤーのタグ（Inspector で変更可能）


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
                AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); // ジャンプ台のSE
            }
        }
    }
}
