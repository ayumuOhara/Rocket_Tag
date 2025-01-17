using UnityEngine;

public class ObjectBouncer : MonoBehaviour
{
    // �͂����͂̑傫��
    public float bounceForce = 5f;

    void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����I�u�W�F�N�g��Rigidbody�������Ă��邩�m�F
        Rigidbody otherRigidbody = collision.rigidbody;
        if (otherRigidbody != null)
        {
            // �����̒��S����Փˈʒu�ւ̃x�N�g�����v�Z
            Vector3 bounceDirection = collision.transform.position - transform.position;

            // Y�������𖳎����Đ��������̗͂����ɂ���
            bounceDirection.y = 0f;
            bounceDirection.Normalize(); // ���K��

            // ���������ɗ͂�K�p
            otherRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}

