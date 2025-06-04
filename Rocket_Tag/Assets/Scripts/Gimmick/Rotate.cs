using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Rotate : MonoBehaviourPun, IPunObservable
{
    private string targetTag = "Player";  // "Player"タグを持つオブジェクトにのみ影響

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

            // 回転床に乗っているプレイヤーを回転させる
            MovePlayersWithPlatform();
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

    // プレイヤーを回転床と一緒に回転させる
    private void MovePlayersWithPlatform()
    {
        foreach (var player in playersOnObject)
        {
            if (player != null && player.GetComponent<PhotonView>().IsMine)
            {
                // プレイヤーを回転オブジェクトの回転に合わせて動かす
                player.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // プレイヤーが回転床に乗った場合
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Transform playerTransform = collision.transform;
            playersOnObject.Add(playerTransform);

           // Debug.Log($"プレイヤー {collision.gameObject.name} が回転オブジェクトに乗った。");
        }
    }

    // プレイヤーが回転床から降りた場合
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Transform playerTransform = collision.transform;
            playersOnObject.Remove(playerTransform);

           // Debug.Log($"プレイヤー {collision.gameObject.name} が回転オブジェクトから降りた。");
        }
    }

    // 回転方向を反転する（外部からも呼び出せる）
    public void ReverseRotation()
    {
        rotationSpeed = -rotationSpeed;
    }
}
