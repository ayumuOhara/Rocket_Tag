using UnityEngine;

public class RandomMovementXZ : MonoBehaviour
{
    // �ړ��͈́iX����Z���j
    public Vector2 moveAreaX = new Vector2(-10f, 10f);
    public Vector2 moveAreaZ = new Vector2(-10f, 10f);

    // �ړ����x
    public float moveSpeed = 3f;

    // ���̖ړI�n
    private Vector3 targetPosition;

    void Start()
    {
        // �ŏ��̖ړI�n��ݒ�
        SetNewTargetPosition();
    }

    void Update()
    {
        // ���݈ʒu����ړI�n�܂ł̈ړ�
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // �ړI�n�ɓ��B������V�����ړI�n��ݒ�
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTargetPosition();
        }
    }

    // �V���������_���ȖړI�n��ݒ肷��
    void SetNewTargetPosition()
    {
        float randomX = Random.Range(moveAreaX.x, moveAreaX.y);
        float randomZ = Random.Range(moveAreaZ.x, moveAreaZ.y);

        // Y���W�͕ύX�����A���݂̍�����ێ�
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);
    }
}
