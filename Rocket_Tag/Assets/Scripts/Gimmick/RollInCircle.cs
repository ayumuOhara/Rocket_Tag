using UnityEngine;

public class RollInCircle : MonoBehaviour
{
    // ����̒��S�_
    public Transform centerPoint;

    // ����̔��a
    public float radius = 5f;

    // ���񑬓x (�p���x, �P��: �x/�b)
    public float orbitSpeed = 30f;

    // ���̔��a (�����ڏ�̑傫���ɑΉ�)
    public float objectRadius = 0.5f;

    // ���݂̊p�x
    private float currentAngle;

    void Start()
    {
        if (centerPoint == null)
        {
            Debug.LogError("���S�_ (centerPoint) ���ݒ肳��Ă��܂���B");
            return;
        }

        // �����ʒu�𒆐S�_����̔��a�Ōv�Z
        Vector3 direction = transform.position - centerPoint.position;
        radius = direction.magnitude; // ���S�_����̋����𔼌a�Ƃ���
        currentAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
    }

    void Update()
    {
        if (centerPoint == null) return;

        // �p�x���X�V
        currentAngle += orbitSpeed * Time.deltaTime;

        // ���W�A���ɕϊ�
        float radian = currentAngle * Mathf.Deg2Rad;

        // �V�����ʒu���v�Z (���S�_����ɉ~������ړ�)
        float x = Mathf.Cos(radian) * radius;
        float z = Mathf.Sin(radian) * radius;

        // Y���̍������ێ�
        Vector3 newPosition = new Vector3(
            centerPoint.position.x + x,
            centerPoint.position.y,
            centerPoint.position.z + z
        );

        // �]����̉�]���v�Z
        float distanceTraveled = orbitSpeed * Mathf.Deg2Rad * radius * Time.deltaTime;
        float rotationAngle = (distanceTraveled / objectRadius) * Mathf.Rad2Deg;

        // �I�u�W�F�N�g�̉�]���X�V
        Vector3 rollAxis = new Vector3(-Mathf.Sin(radian), 0, Mathf.Cos(radian)); // �]���莲
        transform.Rotate(rollAxis, rotationAngle, Space.World);

        // �ʒu���X�V
        transform.position = newPosition;
    }
}
