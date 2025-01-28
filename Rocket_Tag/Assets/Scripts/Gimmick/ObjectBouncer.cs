using UnityEngine;

public class ObjectBouncer : MonoBehaviour
{
    // �͂����͂̑傫��
    public float bounceForce = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����I�u�W�F�N�g��Rigidbody�������Ă��邩�m�F
        Rigidbody otherRigidbody = collision.rigidbody;
        if (otherRigidbody != null)
        {
            // �Փˈʒu���玩���̒��S�ւ̃x�N�g�����v�Z
            Vector3 bounceDirection = collision.transform.position - transform.position;

            // ���������݂̂ɐ���
            bounceDirection.y = 1f;
            bounceDirection.Normalize(); // ���K�����ĕ����x�N�g�����쐬

            // ���������ɗ͂�K�p
            otherRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}
