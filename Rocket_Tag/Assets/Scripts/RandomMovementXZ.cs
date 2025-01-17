using UnityEngine;

public class RandomMovementXZ : MonoBehaviour
{
    // 移動範囲（X軸とZ軸）
    public Vector2 moveAreaX = new Vector2(-10f, 10f);
    public Vector2 moveAreaZ = new Vector2(-10f, 10f);

    // 移動速度
    public float moveSpeed = 3f;

    // 次の目的地
    private Vector3 targetPosition;

    void Start()
    {
        // 最初の目的地を設定
        SetNewTargetPosition();
    }

    void Update()
    {
        // 現在位置から目的地までの移動
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 目的地に到達したら新しい目的地を設定
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTargetPosition();
        }
    }

    // 新しいランダムな目的地を設定する
    void SetNewTargetPosition()
    {
        float randomX = Random.Range(moveAreaX.x, moveAreaX.y);
        float randomZ = Random.Range(moveAreaZ.x, moveAreaZ.y);

        // Y座標は変更せず、現在の高さを保持
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);
    }
}
