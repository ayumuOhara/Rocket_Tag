using UnityEngine;

public class ObjectBouncer : MonoBehaviour
{
    // はじく力の大きさ
    public float bounceForce = 5f;

    void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトがRigidbodyを持っているか確認
        Rigidbody otherRigidbody = collision.rigidbody;
        if (otherRigidbody != null)
        {
            // 自分の中心から衝突位置へのベクトルを計算
            Vector3 bounceDirection = collision.transform.position - transform.position;

            // Y軸成分を無視して水平方向の力だけにする
            bounceDirection.y = 0f;
            bounceDirection.Normalize(); // 正規化

            // 水平方向に力を適用
            otherRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}

