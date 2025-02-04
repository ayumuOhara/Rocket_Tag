using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RotateObject : MonoBehaviourPun, IPunObservable
{
    private string targetTag = "Player";
    [SerializeField] private float maxDistance = 5.0f;  // 検知する最大距離（Inspector で設定可）

    [Header("回転設定")]  // Inspector で分かりやすくするためのヘッダー
    [SerializeField] private float rotationSpeed = 100f;   // 初期回転速度
    [SerializeField] private float switchInterval = 3f;    // 方向を切り替える間隔（秒）

    private float timer = 0f;           // 経過時間を追跡するためのタイマー
    private Quaternion networkRotation; // ネットワーク同期用

    void Start()
    {
        networkRotation = transform.rotation;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // 一定時間ごとに回転方向を反転
            timer += Time.deltaTime;
            if (timer >= switchInterval)
            {
                rotationSpeed = -rotationSpeed; // 回転方向を反転
                timer = 0f;                     // タイマーをリセット
            }

            // オブジェクトを回転させる
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        else
        {
            // 他プレイヤーの回転を補間して同期
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }

    // 回転データを同期
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 自分のオブジェクトの回転を送信
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 他プレイヤーのオブジェクトの回転を受信
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    // 近くのターゲットを取得
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
