using UnityEngine;

public class RandomMovementXZ : MonoBehaviour
{
    // �ړ��͈́iX����Z���j
    public Vector2 moveRangeX = new Vector2(-10f, 10f); // ��ʒu�����X���͈̔�
    public Vector2 moveRangeZ = new Vector2(-10f, 10f); // ��ʒu�����Z���͈̔�

    // �ړ����x
    public float moveSpeed = 3f;

    // ��ʒu
    private Vector3 basePosition;

    // ���̖ړI�n
    private Vector3 targetPosition;

    void Start()
    {
        // �����ʒu����ʒu�Ƃ��ċL�^
        basePosition = transform.position;

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
        float randomX = Random.Range(moveRangeX.x, moveRangeX.y);
        float randomZ = Random.Range(moveRangeZ.x, moveRangeZ.y);

        // ��ʒu�𒆐S�ɐV�����ړI�n��ݒ�
        targetPosition = new Vector3(basePosition.x + randomX, transform.position.y, basePosition.z + randomZ);
    }

    // �ړ��͈͂������i�G�f�B�^�p�j
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            basePosition,
            new Vector3(moveRangeX.y - moveRangeX.x, 0, moveRangeZ.y - moveRangeZ.x)
        );
    }
}
