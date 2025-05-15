using UnityEngine;

public class HammerBound : MonoBehaviour
{
    public float bounceForce = 20f;  // 吹っ飛ばす力

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーのノックバック処理を取得
        var knockback = other.GetComponent<PlayerKnockback>();
        if (knockback != null)
        {
            // ハンマー → プレイヤー方向のベクトル
            Vector3 direction = (other.transform.position - transform.position).normalized;
            direction.y = 0.5f; // 少し上向きに調整

            knockback.KnockBack(direction, bounceForce);
        }
    }
}
