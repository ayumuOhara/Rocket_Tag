using UnityEngine;
using Photon.Pun;

public class RandomMapSpawner : MonoBehaviourPunCallbacks
{
    [Header("マップのプレハブを登録")]
    public GameObject[] mapPrefabs;

    [Header("スポーン位置")]
    public Transform spawnPoint;

    private GameObject currentMap;

    void Start()
    {
        // Master Client のみマップを生成
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnRandomMap();
        }
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

        // ネットワーク上にマップを生成（全員に反映）
        currentMap = PhotonNetwork.InstantiateRoomObject(
            selectedMap.name,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Debug.Log($"[MasterClient] マップ「{selectedMap.name}」を生成しました。");
    }
}
