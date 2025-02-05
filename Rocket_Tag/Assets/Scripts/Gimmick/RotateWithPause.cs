using UnityEngine;
using System.Collections;

public class RotateWithPause : MonoBehaviour
{
    [Header("��]�ݒ�")]
    [SerializeField] private float rotationSpeed = 50f; // ��]���x�i�x/�b�j
    [SerializeField] private float pauseDuration = 2f;  // ��~���ԁi�b�j

    private float currentTargetAngle = 180f; // �ŏ��̖ڕW�p�x��180�x
    private bool isRotating = true;   // ��]�����ǂ����̃t���O

    void Start()
    {
        StartCoroutine(RotateRoutine());
    }

    IEnumerator RotateRoutine()
    {
        while (true)
        {
            // �ڕW�p�x�܂ŉ�]
            while (isRotating)
            {
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, currentTargetAngle, rotationSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);

                if (Mathf.Approximately(angle, currentTargetAngle))
                {
                    isRotating = false; // ��]�I��
                    yield return new WaitForSeconds(pauseDuration); // ��~����

                    // ����0�x�ɉ�]����悤�ɐݒ�
                    if (currentTargetAngle == 180f)
                    {
                        currentTargetAngle = 0f; // 180�x����0�x�ɉ�]
                    }
                    else
                    {
                        currentTargetAngle = 180f; // 0�x����180�x�ɉ�]
                    }

                    isRotating = true; // ��]�ĊJ
                }
                yield return null; // ���̃t���[���܂őҋ@
            }
        }
    }
}
