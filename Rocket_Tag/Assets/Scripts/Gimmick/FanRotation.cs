using UnityEngine;

public class FanRotation : MonoBehaviour
{
    // �U��q�^���̒��S�p�x (�����p�x)
    public float centerAngle = 0f;

    // �U�蕝 (��������ő�p�x)
    public float swingAngle = 45f;

    // �U��q�̑��x
    public float swingSpeed = 2f;

    private float time;

    void Update()
    {
        // ���Ԃ��g���ăX���[�Y�ȐU��q�^�����v�Z
        time += Time.deltaTime * swingSpeed;

        // �U��q�̊p�x���v�Z (sin�g�ŉ����^��)
        float angle = centerAngle + Mathf.Sin(time) * swingAngle;

        // �v�Z���ꂽ�p�x��K�p
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
