using UnityEngine;

public class BounceUp : MonoBehaviour
{
    [Header("�o�E���X�ݒ�")]
    [SerializeField] private float bounceForce = 5f; // �͂�����
    [SerializeField] private string playerTag = "Player"; // �v���C���[�̃^�O�iInspector �ŕύX�\�j


    void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����I�u�W�F�N�g���v���C���[���m�F
        if (collision.gameObject.CompareTag(playerTag))
        {
            // �v���C���[�� Rigidbody ���擾
            Rigidbody playerRigidbody = collision.rigidbody;

            if (playerRigidbody != null)
            {
                // ������ɗ͂�������
                Vector3 bounceDirection = Vector3.up;
                playerRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
                AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); // �W�����v���SE
            }
        }
    }
}
