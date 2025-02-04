using Photon.Realtime;
using UnityEngine;

public class Seesaw:MonoBehaviour
{
    [Header("�X���̐ݒ�")]
    public float tiltSpeed = 5f;      // �X������
    public float maxTiltAngle = 15f;  // �ő�X�Ίp�x

    private Transform player;
    private Quaternion originalRotation;

    void Start()
    {
        // �����̉�]��ۑ�
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (player != null)
        {
            // �v���C���[������Ă���ʒu���擾
            Vector3 direction = (player.position - transform.position).normalized;

            // X����Z���̌X���𐧌�
            float tiltX = Mathf.Clamp(direction.z * maxTiltAngle, -maxTiltAngle, maxTiltAngle);
            float tiltZ = Mathf.Clamp(-direction.x * maxTiltAngle, -maxTiltAngle, maxTiltAngle);

            // �V������]��K�p (���̉�]����ɂ���)
            Quaternion targetRotation = Quaternion.Euler(tiltX, 0, tiltZ) * originalRotation;

            // ���X�ɌX����
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
        }
        else
        {
            // �v���C���[���~�肽�猳�̊p�x�ɖ߂�
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * tiltSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }
}
