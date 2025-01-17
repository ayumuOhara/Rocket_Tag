using UnityEngine;

public class ObjectBouncer : MonoBehaviour
{
    // はじく力の大きさ（距離に比例）
    public float bounceForce = 5f;

    void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトがRigidbodyを持っているか確認
        Rigidbody otherRigidbody = collision.rigidbody;
        if (otherRigidbody != null)
        {
            // 衝突の法線（接触面の方向）を取得
            Vector3 bounceDirection = collision.contacts[0].normal;

            // はじく力を適用
            otherRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}
