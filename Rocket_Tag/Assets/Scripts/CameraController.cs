using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;          // �����Ώۃv���C���[

    [SerializeField] private float distance = 15.0f;    // �����Ώۃv���C���[����J�����𗣂�����
    [SerializeField] private Quaternion vRotation;      // �J�����̐�����](�����낵��])
    [SerializeField] public Quaternion hRotation;      // �J�����̐�����]
    [SerializeField] private float turnSpeed = 10.0f;   // ��]���x

    private void Awake()
    {
        
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        // ��]�̏�����
        vRotation = Quaternion.Euler(30, 0, 0);         // ������](X�������Ƃ����])�́A30�x�����낷��]
        hRotation = Quaternion.identity;                // ������](Y�������Ƃ����])�́A����]
        transform.rotation = hRotation * vRotation;     // �ŏI�I�ȃJ�����̉�]�́A������]���Ă��琅����]���鍇����]

        // �ʒu�̏�����
        // player�ʒu���狗��distance������O�Ɉ������ʒu��ݒ肵�܂�
        transform.position = player.position - transform.rotation * Vector3.forward * distance;
    }

    // Update is called once per frame
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
        }

        // �J�����̉�](transform.rotation)�̍X�V
        // ���@1 : ������]���Ă��琅����]���鍇����]�Ƃ��܂�
        transform.rotation = hRotation * vRotation;
    }

    // �w�肵���I�u�W�F�N�g��ǐՂ��鏈��
    void TrackingTarget()
    {
        // �J�����̈ʒu(transform.position)�̍X�V
        // player�ʒu���狗��distance������O�Ɉ������ʒu��ݒ肵�܂�(�ʒu�␳��)
        transform.position = player.position + new Vector3(0, 1.5f, 0) - transform.rotation * Vector3.forward * distance;
    }
}
