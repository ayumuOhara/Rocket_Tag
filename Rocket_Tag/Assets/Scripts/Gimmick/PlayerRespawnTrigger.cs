using UnityEngine;

public class PlayerRespawnTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player"; // �v���C���[�̃^�O
    [SerializeField] private string stageTag = "Stage"; // �X�e�[�W�̃^�O
    [SerializeField] private float searchRadius = 10f; // �X�e�[�W�����͈�

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Transform nearestStage = FindNearestStage(other.transform.position);
            if (nearestStage != null)
            {
                Vector3 respawnPosition = nearestStage.position + Vector3.up * 1.5f; // �X�e�[�W�̏�Ƀ��X�|�[��
                other.transform.position = respawnPosition;
                Debug.Log("�v���C���[���߂��̃X�e�[�W�Ƀ��X�|�[�����܂���");
            }
            else
            {
                Debug.LogWarning("�߂��ɃX�e�[�W��������܂���I");
            }
        }
    }

    private Transform FindNearestStage(Vector3 playerPosition)
    {
        GameObject[] stages = GameObject.FindGameObjectsWithTag(stageTag);
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject stage in stages)
        {
            float distance = Vector3.Distance(playerPosition, stage.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = stage.transform;
            }
        }

        return nearest;
    }
}
