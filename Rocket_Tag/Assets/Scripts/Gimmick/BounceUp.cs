using UnityEngine;

public class BounceUp : MonoBehaviour
{
    // �͂����́iInspector�Őݒ�\�j
    public float bounceForce = 5f;

    // �Փ˃C�x���g
    void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����I�u�W�F�N�g��Rigidbody���擾
        Rigidbody otherRigidbody = collision.rigidbody;

        if (otherRigidbody != null)
        {
            // ������ɗ͂�������
            Vector3 bounceDirection = Vector3.up;
            otherRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}
