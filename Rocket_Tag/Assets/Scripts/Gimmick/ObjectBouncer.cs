using UnityEngine;

public class ObjectBouncer : MonoBehaviour
{
    // はじく力の大きさ
    public float bounceForce = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトがRigidbodyを持っているか確認
        Rigidbody otherRigidbody = collision.rigidbody;
        if (otherRigidbody != null)
        {
            // 衝突位置から自分の中心へのベクトルを計算
            Vector3 bounceDirection = collision.transform.position - transform.position;

            // 水平方向のみに制限
            bounceDirection.y = 1f;
            bounceDirection.Normalize(); // 正規化して方向ベクトルを作成

            // 水平方向に力を適用
            otherRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}
