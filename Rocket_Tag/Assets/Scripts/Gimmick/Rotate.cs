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
    private HashSet<GameObject> playersOnObject = new HashSet<GameObject>(); // 回転オブジェクトに乗っているプレイヤー

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

    // 回転オブジェクトに乗っているプレイヤーに回転を与える
    private void ApplyRotationToPlayersOnObject()
    {
        foreach (var player in playersOnObject)
        {
            ApplyRotationToPlayer(player);
        }
    }

    // プレイヤーに回転床の影響を与える
    private void ApplyRotationToPlayer(GameObject player)
    {
        // プレイヤーの位置と回転床の位置を比較
        Vector3 directionToCenter = player.transform.position - transform.position;
        directionToCenter.y = 0; // Y軸を無視して水平面で回転

        // 回転床の回転に合わせて、影響を与える
        float angle = rotationSpeed * Time.deltaTime;
        player.transform.RotateAround(transform.position, Vector3.up, angle);

        // プレイヤーに回転を与える
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            // プレイヤーに床の回転を模倣するような力を加える
            playerRb.angularVelocity = Vector3.zero; // 既存の回転をリセット
            playerRb.AddTorque(Vector3.up * rotationSpeed * 0.1f, ForceMode.VelocityChange);
        }
    }

    // プレイヤーが回転オブジェクトに乗った場合
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            playersOnObject.Add(collision.gameObject); // 回転オブジェクトに乗ったプレイヤーを記録
        }
    }

    // プレイヤーが回転オブジェクトから降りた場合
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            playersOnObject.Remove(collision.gameObject); // 回転オブジェクトから降りたプレイヤーを削除
        }
    }

    // 回転方向を反転する（外部からも呼び出せる）
    public void ReverseRotation()
    {
        rotationSpeed = -rotationSpeed;
    }
}
