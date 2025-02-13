using UnityEngine;

public class BounceUp : MonoBehaviour
{
    [Header("�o�E���X�ݒ�")]
    [SerializeField] private float bounceForce = 5f; // �͂�����

    [Header("�T�E���h�ݒ�")]
    [SerializeField] private AudioSource audioSource; // �����Đ����� AudioSource
    [SerializeField] private string playerTag = "Player"; // �v���C���[�̃^�O�iInspector �ŕύX�\�j

    void Start()
    {
        // AudioSource ���ݒ肳��Ă��Ȃ��ꍇ�A�����Œǉ�
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // �f�t�H���g�ł͍Đ����Ȃ�
            audioSource.enabled = false; // �ŏ��͖��������Ă���
        }
    }

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
            }

            // AudioSource ��L�������ĉ���炷
            if (audioSource != null)
            {
                audioSource.enabled = true;  // AudioSource �� ON �ɂ���
                if (!audioSource.isPlaying)  // ���łɖ��Ă��Ȃ���΍Đ�
                {
                    audioSource.Play();
                }
            }
        }
    }
}
