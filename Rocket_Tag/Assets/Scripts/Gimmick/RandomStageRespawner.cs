using UnityEngine;
using System.Collections.Generic;

public class RandomStageRespawner : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player"; // プレイヤーのタグ
    [SerializeField] private string stageTag = "Stage";   // ステージのタグ
    [SerializeField] private float searchRadius = 10f;    // ステージ検索範囲

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        Transform randomStage = FindRandomStageInRange(other.transform.position);
        if (randomStage != null)
        {
            Vector3 respawnPosition = randomStage.position + Vector3.up * 1.5f;
            other.transform.position = respawnPosition;
           // Debug.Log("プレイヤーがランダムなステージにリスポーンしました");

            // 慣性をリセット
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
               // Debug.Log("プレイヤーの慣性をリセットしました。");
            }
        }
        else
        {
           // Debug.LogWarning("範囲内にステージが見つかりません！");
        }
    }

    private Transform FindRandomStageInRange(Vector3 playerPosition)
    {
        GameObject[] stages = GameObject.FindGameObjectsWithTag(stageTag);
        List<Transform> validStages = new List<Transform>();

        foreach (GameObject stage in stages)
        {
            if (Vector3.Distance(playerPosition, stage.transform.position) <= searchRadius)
            {
                validStages.Add(stage.transform);
            }
        }

        if (validStages.Count == 0) return null;

        int randomIndex = Random.Range(0, validStages.Count);
        return validStages[randomIndex];
    }
}
