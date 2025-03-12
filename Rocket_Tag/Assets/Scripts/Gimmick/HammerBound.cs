using UnityEngine;

public class HammerBound : MonoBehaviour
{
    // はじく力の大きさ
    public float bounceForce = 5f;

    private void OnTriggerEnter(Collider other)
    {
        // 衝突したオブジェクトがRigidbodyを持っているか確認
        Rigidbody otherRigidbody = other.attachedRigidbody;
        if (otherRigidbody != null)
        {
            // 衝突位置から自分の中心へのベクトルを計算
            Vector3 bounceDirection = other.transform.position - transform.position;

            // 水平方向のみに制限
            bounceDirection.y = 1f;
            bounceDirection.Normalize(); // 正規化して方向ベクトルを作成

            // 水平方向に力を適用
            otherRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}