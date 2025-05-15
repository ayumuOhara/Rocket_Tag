using UnityEngine;

public class RandomMapSpawner : MonoBehaviour
{
    [Header("マップのプレハブを登録")]
    public GameObject[] mapPrefabs;

    [Header("スポーン位置")]
    public Transform spawnPoint;

    private GameObject currentMap;

    void Start()
    {
        SpawnRandomMap();
    }

    void SpawnRandomMap()
    {
        if (mapPrefabs.Length == 0 || spawnPoint == null)
        {
            Debug.LogWarning("マッププレハブまたはスポーンポイントが未設定です。");
            return;
        }

        int randomIndex = Random.Range(0, mapPrefabs.Length);
        GameObject selectedMap = mapPrefabs[randomIndex];

        currentMap = Instantiate(
            selectedMap,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Debug.Log($"マップ「{selectedMap.name}」を生成しました。");
    }
}
