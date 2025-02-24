using UnityEngine;

public class MoveUpDown : MonoBehaviour
{
    [Header("移動設定")]
    public float moveRange = 2f;   // 上下移動範囲
    public float moveSpeed = 2f;   // 移動速度

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // 上下に繰り返し移動する動き
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
