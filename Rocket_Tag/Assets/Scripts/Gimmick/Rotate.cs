using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RotateObject : MonoBehaviourPun, IPunObservable
{
    private string targetTag = "Player";
    [SerializeField] private float maxDistance = 5.0f;  // ���m����ő勗���iInspector �Őݒ�j

    [Header("��]�ݒ�")]  // Inspector �ŕ�����₷�����邽�߂̃w�b�_�[
    [SerializeField] private float rotationSpeed = 100f;   // ������]���x
    [SerializeField] private float switchInterval = 3f;    // ������؂�ւ���Ԋu�i�b�j

    private float timer = 0f;           // �o�ߎ��Ԃ�ǐՂ��邽�߂̃^�C�}�[
    private Quaternion networkRotation; // �l�b�g���[�N�����p

    void Start()
    {
        networkRotation = transform.rotation;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // ��莞�Ԃ��Ƃɉ�]�����𔽓]
            timer += Time.deltaTime;
            if (timer >= switchInterval)
            {
                rotationSpeed = -rotationSpeed; // ��]�����𔽓]
                timer = 0f;                     // �^�C�}�[�����Z�b�g
            }

            // �I�u�W�F�N�g����]������
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        else
        {
            // ���v���C���[�̉�]���Ԃ��ē���
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }

    // ��]�f�[�^�𓯊�
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // �����̃I�u�W�F�N�g�̉�]�𑗐M
            stream.SendNext(transform.rotation);
        }
        else
        {
            // ���v���C���[�̃I�u�W�F�N�g�̉�]����M
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    // �߂��̃^�[�Q�b�g���擾
    public GameObject GetTargetDistance()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        GameObject nearestObject = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            Vector3 direction = target.transform.position - transform.position;
            float distance = direction.magnitude;

            // �������͈͓����`�F�b�N
            if (distance <= maxDistance)
            {
                // Raycast�𔭎˂��ď�Q�����Ȃ����m�F
                Ray ray = new Ray(transform.position, direction.normalized);
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                {
                    if (hit.collider.gameObject == target)
                    {
                        // �ŒZ�������X�V
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestObject = target;
                        }
                    }
                }
            }
        }

        return nearestObject;
    }
}
