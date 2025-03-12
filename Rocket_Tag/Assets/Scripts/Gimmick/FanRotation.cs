using UnityEngine;

public class FanRotation : MonoBehaviour
{
    // 振り子運動の中心角度 (初期角度)
    public float centerAngle = 0f;

    // 振り幅 (往復する最大角度)
    public float swingAngle = 45f;

    // 振り子の速度
    public float swingSpeed = 2f;

    // Y軸の回転 (Inspectorで設定可能)
    public float rotationY = 0f;

    private float time;

    void Update()
    {
        // 時間を使ってスムーズな振り子運動を計算
        time += Time.deltaTime * swingSpeed;

        // 振り子の角度を計算 (sin波で往復運動)
        float angle = centerAngle + Mathf.Sin(time) * swingAngle;

        // 計算された角度を適用 (Y軸の回転をInspectorで調整可能にする)
        transform.localRotation = Quaternion.Euler(0, rotationY, angle);
    }
}
