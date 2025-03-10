using UnityEngine;

public class CameraRotateAndZoom : MonoBehaviour
{
    public Transform target; // 回転の中心となるターゲット
    public float rotationDuration = 5f; // 回転を続ける時間（秒）
    public float rotationSpeed = 45f; // 1秒あたりの回転角度（度）
    public float startRadius = 10f; // 初期のカメラ距離
    public float endRadius = 3f; // 最終的なカメラ距離
    public float zoomSpeed = 1f; // ズーム速度（1で通常速度、2で2倍速など）
    public Vector3 axis = Vector3.up; // 回転軸

    private float timer = 0f;
    private bool isRotating = true; // 回転を制御するフラグ
    private float currentAngle = 0f; // 現在の回転角度
    private float currentRadius; // 現在のカメラ距離

    void Start()
    {
        currentRadius = startRadius; // 初期距離設定
    }

    void Update()
    {
        if (target == null || !isRotating) return;

        timer += Time.deltaTime;

        if (timer <= rotationDuration)
        {
            // 回転角度を時間経過に応じて加算
            currentAngle += rotationSpeed * Time.deltaTime;
            float radian = currentAngle * Mathf.Deg2Rad;

            // 徐々にズームイン（zoomSpeedを反映）
            float zoomFactor = Mathf.Clamp01(timer / (rotationDuration / zoomSpeed));
            currentRadius = Mathf.Lerp(startRadius, endRadius, zoomFactor);

            // 新しい位置を計算
            Vector3 offset = new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian)) * currentRadius;
            transform.position = target.position + offset;

            // ターゲットを向く
            transform.LookAt(target);
        }
        else
        {
            // 一定時間経過したら回転を停止
            isRotating = false;
        }
    }
}