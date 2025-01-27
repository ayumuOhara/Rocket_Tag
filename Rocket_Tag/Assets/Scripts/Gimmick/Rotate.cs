using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RotateObject : MonoBehaviourPun, IPunObservable
{

    private string targetTag = "Player";
    public float maxDistance = 5.0f;
    // 回転速度 (1秒あたりの回転角度)
    public float rotationSpeed = 100f;

    // 同期用の現在の回転
    private Quaternion networkRotation;

    void Start()
    {
        // 初期の回転を設定
        networkRotation = transform.rotation;
        maxDistance = 5.0f;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // 自分が所有者の場合、回転を制御
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        else
        {
            // 他プレイヤーの回転を補間して同期
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }

    // 回転データを同期するためのシリアライズ処理
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
