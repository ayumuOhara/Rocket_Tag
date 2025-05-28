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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        // プレイヤーの PlayerMovement スクリプトを取得
    //        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
    //        if (playerMovement != null)
    //        {
    //            // 操作反転コルーチンを開始
    //            playerMovement.StartCoroutine(playerMovement.ReverseControll());
    //        }

    //        // エフェクトを再生
    //        if (hitEffectPrefab != null)
    //        {
    //            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
    //            Destroy(effect, 2f); // 2秒後に自動破棄
    //        }

    //        // Spawner に通知して自身を削除
    //        spawner?.OnPrefabDestroyedEarly();
    //        Destroy(gameObject);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // PlayerMovement を取得して操作反転
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.StartCoroutine(playerMovement.ReverseControll());
            }

            // エフェクトの再生位置を Player の上に
            if (hitEffectPrefab != null)
            {
                // プレイヤーの位置を取得
                Vector3 effectPos = other.transform.position;

                // 高さを調整（例：+1.5f）
                effectPos.y += 1.75f;

                // 回転を指定（X軸 -90度）
                Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);

                // エフェクトを生成
                GameObject effect = Instantiate(hitEffectPrefab, effectPos, rotation);

                // 任意：プレイヤーに追従させたい場合（移動に合わせて表示位置も動く）
                effect.transform.SetParent(other.transform);


                Destroy(effect, 3f); // 3秒後に自動削除
            }

            // Spawner に通知 & 自分を削除
            spawner?.OnPrefabDestroyedEarly();
            Destroy(gameObject);
        }
    }
}
