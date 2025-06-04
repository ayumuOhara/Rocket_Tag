using UnityEngine;

public class RandomMapSpawner : MonoBehaviour
{
    [Header("マップのプレハブを登録")]
    public GameObject[] mapPrefabs;

    [Header("スポーン位置")]
    public Transform spawnPoint;

    [Header("MapA用の追加オブジェクト")]
    public GameObject specialObjectPrefab;  // MapA用に表示したいPrefab
    public Transform specialObjectSpawnPoint; // 表示位置（オプション）

    private GameObject currentMap;

    void Start()
    {
        SpawnRandomMap();
    }

    void SpawnRandomMap()
    {
        if (mapPrefabs.Length == 0 || spawnPoint == null)
        {
            //Debug.LogWarning("マッププレハブまたはスポーンポイントが未設定です。");
            return;
        }

        int randomIndex = Random.Range(0, mapPrefabs.Length);
        GameObject selectedMap = mapPrefabs[randomIndex];

        currentMap = Instantiate(
            selectedMap,
            spawnPoint.position,
            spawnPoint.rotation
        );

        //Debug.Log($"マップ「{selectedMap.name}」を生成しました。");

        // もしMapAなら、特定のPrefabを表示
        if (selectedMap.name.Contains("MapA") && specialObjectPrefab != null)
        {
            Vector3 spawnPos = specialObjectSpawnPoint != null
                ? specialObjectSpawnPoint.position
                : Vector3.zero;

            // Y軸に90度回転
            Quaternion spawnRot = Quaternion.Euler(0, 90, 0);

            Instantiate(specialObjectPrefab, spawnPos, spawnRot);
            //Debug.Log("MapA用のオブジェクトをY軸90度回転で生成しました。");
        }

    }
}
