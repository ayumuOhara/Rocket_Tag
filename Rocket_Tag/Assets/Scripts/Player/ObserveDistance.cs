using Photon.Pun;
using UnityEngine;

public class ObserveDistance : MonoBehaviour
{
    private string targetTag = "Player";                    // �^�b�`���̌��m�Ώۂ�tag(�������ɂ�Player�ɕύX����)
    public float maxDistance = 3.0f;                        // ���m����ő勗��

    private void Start()
    {
        maxDistance = 3f;
    }

    // ���̃v���C���[�Ƃ̋����𑪂�
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
