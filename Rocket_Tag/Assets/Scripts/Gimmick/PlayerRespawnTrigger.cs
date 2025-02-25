using UnityEngine;

public class PlayerRespawnTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player"; // プレイヤーのタグ
    [SerializeField] private string stageTag = "Stage"; // ステージのタグ
    [SerializeField] private float searchRadius = 10f; // ステージ検索範囲

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Transform nearestStage = FindNearestStage(other.transform.position);
            if (nearestStage != null)
            {
                Vector3 respawnPosition = nearestStage.position + Vector3.up * 1.5f; // ステージの上にリスポーン
                other.transform.position = respawnPosition;
                Debug.Log("プレイヤーが近くのステージにリスポーンしました");
            }
            else
            {
                Debug.LogWarning("近くにステージが見つかりません！");
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
