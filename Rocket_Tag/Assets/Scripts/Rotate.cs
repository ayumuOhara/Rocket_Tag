using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // ��]���x (1�b������̉�]�p�x)
    public float rotationSpeed = 100f;

    void Update()
    {
        // ������ (Y��) �ɉ�]������
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
