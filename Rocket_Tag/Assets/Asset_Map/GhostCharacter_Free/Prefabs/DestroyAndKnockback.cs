using UnityEngine;

public class DestroyAndKnockback : MonoBehaviour
{
    [Header("エフェクト設定")]
    public GameObject hitEffectPrefab;    // 再生するエフェクト

    private RepeatingSpawner spawner;

    public void SetSpawner(RepeatingSpawner s)
    {
        spawner = s;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーの PlayerMovement スクリプトを取得
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // 操作反転コルーチンを開始
                playerMovement.StartCoroutine(playerMovement.ReverseControll());
            }

            // エフェクトを再生
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 2f); // 2秒後に自動破棄
            }

            // Spawner に通知して自身を削除
            spawner?.OnPrefabDestroyedEarly();
            Destroy(gameObject);
        }
    }
}
