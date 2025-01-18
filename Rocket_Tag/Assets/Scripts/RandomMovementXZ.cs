using UnityEngine;

public class RandomMovementXZ : MonoBehaviour
{
    // 移動範囲（X軸とZ軸）
    public Vector2 moveRangeX = new Vector2(-10f, 10f); // 基準位置からのX軸の範囲
    public Vector2 moveRangeZ = new Vector2(-10f, 10f); // 基準位置からのZ軸の範囲

    // 移動速度
    public float moveSpeed = 3f;

    // 基準位置
    private Vector3 basePosition;

    // 次の目的地
    private Vector3 targetPosition;

    void Start()
    {
        // 初期位置を基準位置として記録
        basePosition = transform.position;

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
        float randomX = Random.Range(moveRangeX.x, moveRangeX.y);
        float randomZ = Random.Range(moveRangeZ.x, moveRangeZ.y);

        // 基準位置を中心に新しい目的地を設定
        targetPosition = new Vector3(basePosition.x + randomX, transform.position.y, basePosition.z + randomZ);
    }

    // 移動範囲を可視化（エディタ用）
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            basePosition,
            new Vector3(moveRangeX.y - moveRangeX.x, 0, moveRangeZ.y - moveRangeZ.x)
        );
    }
}
