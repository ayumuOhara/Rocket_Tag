using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Rotate : MonoBehaviourPun, IPunObservable
{
    private string targetTag = "Player";  // "Player"タグを持つオブジェクトにのみ影響
    [SerializeField] private float maxDistance = 5.0f;  // 検知する最大距離（Inspectorで設定可）

    [Header("回転設定")]
    [SerializeField] private float rotationSpeed = 100f;   // 初期回転速度

    private Quaternion networkRotation; // ネットワーク同期用
    private HashSet<Transform> playersOnObject = new HashSet<Transform>(); // 回転オブジェクトに乗っているプレイヤー

    void Start()
    {
        networkRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            // 回転床を回転させる
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

            // 回転オブジェクトに乗っているプレイヤーに影響を与える
            ApplyRotationToPlayersOnObject();
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
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    // 回転オブジェクトに乗っているプレイヤー全員に影響を与える
    private void ApplyRotationToPlayersOnObject()
    {
        foreach (var player in playersOnObject)
        {
            if (player != null)
            {
                ApplyRotationToPlayer(player);
            }
        }
    }

    // プレイヤーを回転させる（Rigidbodyを使わずに）
    private void ApplyRotationToPlayer(Transform player)
    {
        Vector3 centerOffset = player.position - transform.position;
        centerOffset.y = 0; // 高さの変化を防ぐ

        float angle = rotationSpeed * Time.deltaTime;
        player.position = transform.position + Quaternion.Euler(0, angle, 0) * centerOffset;
    }

    // プレイヤーが回転オブジェクトに乗った場合
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            playersOnObject.Add(other.transform);
            Debug.Log($"プレイヤー {other.name} が回転オブジェクトに乗った。現在のプレイヤー数: {playersOnObject.Count}");
        }
    }

    // プレイヤーが回転オブジェクトから降りた場合
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            playersOnObject.Remove(other.transform);
            Debug.Log($"プレイヤー {other.name} が回転オブジェクトから降りた。現在のプレイヤー数: {playersOnObject.Count}");
        }
    }

    // 回転方向を反転する（外部からも呼び出せる）
    public void ReverseRotation()
    {
        rotationSpeed = -rotationSpeed;
    }
}
