using Photon.Pun;
using UnityEngine;

public class ObserveDistance : MonoBehaviour
{
    private string targetTag = "Player";                    // タッチ時の検知対象のtag(実装時にはPlayerに変更する)
    public float maxDistance = 3.0f;                        // 検知する最大距離

    private void Start()
    {
        maxDistance = 3f;
    }

    // 他のプレイヤーとの距離を測る
    public GameObject GetTargetDistance()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        GameObject nearestObject = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            Vector3 direction = target.transform.position - transform.position;
            float distance = direction.magnitude;

            // 距離が範囲内かチェック
            if (distance <= maxDistance)
            {
                // Raycastを発射して障害物がないか確認
                Ray ray = new Ray(transform.position, direction.normalized);
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                {
                    if (hit.collider.gameObject == target)
                    {
                        // 最短距離を更新
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestObject = target;
                        }
                    }
                }
            }
        }

        return nearestObject;
    }
}
