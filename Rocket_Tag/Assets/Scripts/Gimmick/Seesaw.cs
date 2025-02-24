using Photon.Realtime;
using UnityEngine;

public class Seesaw:MonoBehaviour
{
    [Header("傾きの設定")]
    public float tiltSpeed = 5f;      // 傾く速さ
    public float maxTiltAngle = 15f;  // 最大傾斜角度

    private Transform player;
    private Quaternion originalRotation;

    void Start()
    {
        // 初期の回転を保存
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (player != null)
        {
            // プレイヤーが乗っている位置を取得
            Vector3 direction = (player.position - transform.position).normalized;

            // X軸とZ軸の傾きを制限
            float tiltX = Mathf.Clamp(direction.z * maxTiltAngle, -maxTiltAngle, maxTiltAngle);
            float tiltZ = Mathf.Clamp(-direction.x * maxTiltAngle, -maxTiltAngle, maxTiltAngle);

            // 新しい回転を適用 (元の回転を基準にする)
            Quaternion targetRotation = Quaternion.Euler(tiltX, 0, tiltZ) * originalRotation;

            // 徐々に傾ける
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
        }
        else
        {
            // プレイヤーが降りたら元の角度に戻る
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * tiltSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }
}
