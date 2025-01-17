using UnityEngine;

public class ObjectBouncer : MonoBehaviour
{
    // �͂����͂̑傫���i�����ɔ��j
    public float bounceForce = 5f;

    void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����I�u�W�F�N�g��Rigidbody�������Ă��邩�m�F
        Rigidbody otherRigidbody = collision.rigidbody;
        if (otherRigidbody != null)
        {
            // �Փ˂̖@���i�ڐG�ʂ̕����j���擾
            Vector3 bounceDirection = collision.contacts[0].normal;

            // �͂����͂�K�p
            otherRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}
