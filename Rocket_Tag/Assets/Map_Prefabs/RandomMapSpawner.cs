using UnityEngine;

public class RandomMapSpawner : MonoBehaviour
{
    [Header("マップのプレハブを登録")]
    public GameObject[] mapPrefabs; // 複数のマッププレハブ

    [Header("スポーン位置")]
    public Transform spawnPoint;    // 生成する位置

    private GameObject currentMap;

    void Start()
    {
        SpawnRandomMap();
    }

    public void SpawnRandomMap()
    {
        if (mapPrefabs.Length == 0 || spawnPoint == null)
        {
            Debug.LogWarning("マッププレハブまたはスポーンポイントが未設定です。");
            return;
        }

        // すでに生成されているマップがある場合は削除
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        // ランダムに1つ選んで生成
        int randomIndex = Random.Range(0, mapPrefabs.Length);
        GameObject selectedMap = mapPrefabs[randomIndex];

        currentMap = Instantiate(selectedMap, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"マップ「{selectedMap.name}」が生成されました。");
    }
}
