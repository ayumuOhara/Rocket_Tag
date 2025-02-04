using UnityEngine;
using System.Collections;

public class RotateWithPause : MonoBehaviour
{
    [Header("回転設定")]
    [SerializeField] private float rotationSpeed = 50f; // 回転速度（度/秒）
    [SerializeField] private float pauseDuration = 2f;  // 停止時間（秒）

    private float currentTargetAngle = 180f; // 最初の目標角度は180度
    private bool isRotating = true;   // 回転中かどうかのフラグ

    void Start()
    {
        StartCoroutine(RotateRoutine());
    }

    IEnumerator RotateRoutine()
    {
        while (true)
        {
            // 目標角度まで回転
            while (isRotating)
            {
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, currentTargetAngle, rotationSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);

                if (Mathf.Approximately(angle, currentTargetAngle))
                {
                    isRotating = false; // 回転終了
                    yield return new WaitForSeconds(pauseDuration); // 停止時間

                    // 次に0度に回転するように設定
                    if (currentTargetAngle == 180f)
                    {
                        currentTargetAngle = 0f; // 180度から0度に回転
                    }
                    else
                    {
                        currentTargetAngle = 180f; // 0度から180度に回転
                    }

                    isRotating = true; // 回転再開
                }
                yield return null; // 次のフレームまで待機
            }
        }
    }
}
