using UnityEngine;

public class MoveUpDown : MonoBehaviour
{
    // 移動範囲（上下の最大距離）
    public float moveRange = 2f;

    // 移動速度
    public float moveSpeed = 2f;

    // 初期位置を保存する変数
    private Vector3 startPosition;

    // 回転状態を管理
    private bool isRotated = false;

    void Start()
    {
        // 初期位置を記録
        startPosition = transform.position;
    }

    void Update()
    {
        // 上下に繰り返し移動する動き
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // 下の最大距離（最も低い位置）に到達したか確認
        if (Mathf.Approximately(newY, startPosition.y - moveRange))
        {
            // Z軸を180度回転させる
            if (!isRotated)
            {
                transform.Rotate(0, 0, 180);
                isRotated = true;
            }
        }
        else
        {
            // 回転フラグをリセット
            isRotated = false;
        }
    }
}
