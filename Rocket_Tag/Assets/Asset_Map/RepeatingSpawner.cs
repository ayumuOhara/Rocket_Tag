using UnityEngine;
using System.Collections;

public class RepeatingSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;

    [Header("時間設定")]
    public float spawnDelay = 2f;         // 初回スポーンまでの待ち時間
    public float activeDuration = 5f;     // 自動消滅までの時間
    public float respawnDelay = 3f;       // 消滅後の待ち時間

    private GameObject currentInstance;
    private bool isWaiting = false;

    private void Start()
    {
        StartCoroutine(SpawnCycle());
    }

    private IEnumerator SpawnCycle()
    {
        yield return new WaitForSeconds(spawnDelay);

        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(activeDuration);

            if (currentInstance != null)
            {
                Destroy(currentInstance);
                currentInstance = null;
            }

            isWaiting = true;
            yield return new WaitForSeconds(respawnDelay);
            isWaiting = false;
        }
    }

    private void SpawnObject()
    {
        currentInstance = Instantiate(prefabToSpawn, transform.position, transform.rotation);

        // DestroyAndKnockbackにSpawner参照を渡す
        var destroyScript = currentInstance.GetComponent<DestroyAndKnockback>();
        if (destroyScript != null)
        {
            destroyScript.SetSpawner(this);
        }
    }

    // プレイヤーに当たって即消されたとき呼ばれる
    public void OnPrefabDestroyedEarly()
    {
        if (isWaiting || currentInstance == null) return;

        StopAllCoroutines(); // 待機をキャンセル
        currentInstance = null;
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        isWaiting = true;
        yield return new WaitForSeconds(respawnDelay);
        isWaiting = false;
        StartCoroutine(SpawnCycle());
    }
}
