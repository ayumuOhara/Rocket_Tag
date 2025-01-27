using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RotateObject : MonoBehaviourPun, IPunObservable
{

    private string targetTag = "Player";
    public float maxDistance = 5.0f;
    // ��]���x (1�b������̉�]�p�x)
    public float rotationSpeed = 100f;

    // �����p�̌��݂̉�]
    private Quaternion networkRotation;

    void Start()
    {
        // �����̉�]��ݒ�
        networkRotation = transform.rotation;
        maxDistance = 5.0f;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // ���������L�҂̏ꍇ�A��]�𐧌�
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        else
        {
            // ���v���C���[�̉�]���Ԃ��ē���
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }

    // ��]�f�[�^�𓯊����邽�߂̃V���A���C�Y����
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
