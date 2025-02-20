using UnityEngine;

public class RotationController : MonoBehaviour
{
    [SerializeField] private Rotate rotateObject;
    private bool isPlayerNearby = false;

    void Start()
    {
        if (rotateObject == null)
        {
            rotateObject = FindFirstObjectByType<Rotate>(); // �V�[��������擾
        }
    }

    void Update()
    {
        if (isPlayerNearby)
        {
            rotateObject?.ReverseRotation(); // ��]�����𔽓]
            isPlayerNearby = false; // ��x���s�����烊�Z�b�g
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // �G�ꂽ�I�u�W�F�N�g���v���C���[�̏ꍇ
        {
            isPlayerNearby = true;
        }
    }
}
