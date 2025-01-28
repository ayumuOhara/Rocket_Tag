using UnityEngine;

public class RollInCircle : MonoBehaviour
{
    // 周回の中心点
    public Transform centerPoint;

    // 周回の半径
    public float radius = 5f;

    // 周回速度 (角速度, 単位: 度/秒)
    public float orbitSpeed = 30f;

    // 球の半径 (見た目上の大きさに対応)
    public float objectRadius = 0.5f;

    // 現在の角度
    private float currentAngle;

    void Start()
    {
        if (centerPoint == null)
        {
            Debug.LogError("中心点 (centerPoint) が設定されていません。");
            return;
        }

        // 初期位置を中心点からの半径で計算
        Vector3 direction = transform.position - centerPoint.position;
        radius = direction.magnitude; // 中心点からの距離を半径とする
        currentAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
    }

    void Update()
    {
        if (centerPoint == null) return;

        // 角度を更新
        currentAngle += orbitSpeed * Time.deltaTime;

        // ラジアンに変換
        float radian = currentAngle * Mathf.Deg2Rad;

        // 新しい位置を計算 (中心点を基準に円周上を移動)
        float x = Mathf.Cos(radian) * radius;
        float z = Mathf.Sin(radian) * radius;

        // Y軸の高さを維持
        Vector3 newPosition = new Vector3(
            centerPoint.position.x + x,
            centerPoint.position.y,
            centerPoint.position.z + z
        );

        // 転がりの回転を計算
        float distanceTraveled = orbitSpeed * Mathf.Deg2Rad * radius * Time.deltaTime;
        float rotationAngle = (distanceTraveled / objectRadius) * Mathf.Rad2Deg;

        // オブジェクトの回転を更新
        Vector3 rollAxis = new Vector3(-Mathf.Sin(radian), 0, Mathf.Cos(radian)); // 転がり軸
        transform.Rotate(rollAxis, rotationAngle, Space.World);

        // 位置を更新
        transform.position = newPosition;
    }
}
