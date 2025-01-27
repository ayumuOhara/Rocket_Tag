using UnityEngine;

public class BounceUp : MonoBehaviour
{
    // はじく力（Inspectorで設定可能）
    public float bounceForce = 5f;

    // 衝突イベント
    void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトのRigidbodyを取得
        Rigidbody otherRigidbody = collision.rigidbody;

        if (otherRigidbody != null)
        {
            // 上方向に力を加える
            Vector3 bounceDirection = Vector3.up;
            otherRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}
