using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;          // �����Ώۃv���C���[

    [SerializeField] private float distance = 5.0f;    // �����Ώۃv���C���[����J�����𗣂�����
    [SerializeField] private float verticalAngle = 20.0f; // ������]�p�x
    [SerializeField] private float minVerticalAngle = 10.0f; // ������]�̍ŏ��p�x
    [SerializeField] private float maxVerticalAngle = 50.0f; // ������]�̍ő�p�x
    [SerializeField] private Quaternion vRotation;      // �J�����̐�����](�����낵��])
    [SerializeField] public Quaternion hRotation;      // �J�����̐�����]
    [SerializeField] private float turnSpeed = 5.0f;   // ��]���x

    private void Awake()
    {
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        // ��]�̏�����
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        vRotation = Quaternion.Euler(verticalAngle, 0, 0);  // �����̐�����]
        hRotation = Quaternion.identity;                   // �����̐�����]
        transform.rotation = hRotation * vRotation;        // ������]

        // �ʒu�̏�����
        transform.position = player.position - transform.rotation * Vector3.forward * distance;
    }

    void Update()
    {
        RotationCamera();
        TrackingTarget();
    }

    // �J�����̉�]�̐���
    void RotationCamera()
    {
        // ������]�̍X�V
        if (Input.GetMouseButton(0))
        {
            hRotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * turnSpeed, 0);

            // ������]�̍X�V
            verticalAngle -= Input.GetAxis("Mouse Y") * turnSpeed;
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle); // ������K�p

            vRotation = Quaternion.Euler(verticalAngle, 0, 0);
        }

        // �J�����̉�](transform.rotation)�̍X�V
        transform.rotation = hRotation * vRotation;
    }

    // �w�肵���I�u�W�F�N�g��ǐՂ��鏈��
    void TrackingTarget()
    {
        // �J�����̈ʒu(transform.position)�̍X�V
        transform.position = player.position + new Vector3(0, 1.5f, 0) - transform.rotation * Vector3.forward * distance;
    }
}
