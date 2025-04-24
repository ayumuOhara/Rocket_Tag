using UnityEngine;

public class DestroyAndKnockback : MonoBehaviour
{
    public float knockbackForce = 10f;
    public float upwardForce = 2f;

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
            // ノックバック処理
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (other.transform.position - transform.position).normalized;
                dir.y = 0;
                rb.AddForce(dir * knockbackForce + Vector3.up * upwardForce, ForceMode.Impulse);
            }

            // エフェクトを再生
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 2f); // 2秒後に自動破棄（必要に応じて調整）
            }

            // Spawner に通知して自身を削除
            spawner?.OnPrefabDestroyedEarly();
            Destroy(gameObject);
        }
    }
}
