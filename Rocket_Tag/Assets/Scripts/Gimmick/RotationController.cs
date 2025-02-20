using UnityEngine;

public class RotationController : MonoBehaviour
{
    [SerializeField] private Rotate rotateObject;
    private bool isPlayerNearby = false;

    void Start()
    {
        if (rotateObject == null)
        {
            rotateObject = FindFirstObjectByType<Rotate>(); // シーン内から取得
        }
    }

    void Update()
    {
        if (isPlayerNearby)
        {
            rotateObject?.ReverseRotation(); // 回転方向を反転
            isPlayerNearby = false; // 一度実行したらリセット
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // 触れたオブジェクトがプレイヤーの場合
        {
            isPlayerNearby = true;
        }
    }
}
