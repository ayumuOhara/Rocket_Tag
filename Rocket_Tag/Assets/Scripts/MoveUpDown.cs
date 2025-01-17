using UnityEngine;

public class MoveUpDown : MonoBehaviour
{
    // �ړ��͈́i�㉺�̍ő勗���j
    public float moveRange = 2f;

    // �ړ����x
    public float moveSpeed = 2f;

    // �����ʒu��ۑ�����ϐ�
    private Vector3 startPosition;

    void Start()
    {
        // �����ʒu���L�^
        startPosition = transform.position;
    }

    void Update()
    {
        // �㉺�ɌJ��Ԃ��ړ����铮��
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
