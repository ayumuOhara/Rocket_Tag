using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SeedBasedMapSpawner : MonoBehaviourPunCallbacks
{
    StartCamera startCamera;

    public GameObject[] mapPrefabs;
    public Transform spawnPoint;

    const string ROOM_SEED_KEY = "RoomSeed";
    private bool mapSpawned = false;

    void Start()
    {
        TrySetRoomSeed();

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(ROOM_SEED_KEY))
        {
            SpawnMapFromSeed();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        if (!mapSpawned && changedProps.ContainsKey(ROOM_SEED_KEY))
        {
            SpawnMapFromSeed();
        }
    }

    void TrySetRoomSeed()
    {
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(ROOM_SEED_KEY))
        {
            int seed = Random.Range(int.MinValue, int.MaxValue);
            Hashtable props = new Hashtable { { ROOM_SEED_KEY, seed } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            Debug.Log($"Room にランダムシード {seed} を設定しました");
        }
    }

    void SpawnMapFromSeed()
    {
        if (mapPrefabs.Length == 0 || spawnPoint == null) return;

        int seed = (int)PhotonNetwork.CurrentRoom.CustomProperties[ROOM_SEED_KEY];
        Random.InitState(seed);

        int index = Random.Range(0, mapPrefabs.Length);
        GameObject selected = mapPrefabs[index];

        Instantiate(selected, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"[同期済] マップ「{selected.name}」を生成しました。");
        mapSpawned = true;

        startCamera = GameObject.Find("WaitCamera").GetComponent<StartCamera>();
        startCamera.Initialize();
    }
}
