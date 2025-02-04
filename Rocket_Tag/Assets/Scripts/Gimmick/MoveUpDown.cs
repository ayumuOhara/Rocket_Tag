using UnityEngine;

public class MoveUpDown : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float moveRange = 2f;   // �㉺�ړ��͈�
    public float moveSpeed = 2f;   // �ړ����x

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // �㉺�ɌJ��Ԃ��ړ����铮��
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
