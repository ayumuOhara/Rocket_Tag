using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    GameObject player;
    Transform playerTransform;                                    // �����Ώۃv���C���[
    [SerializeField] private CameraController refCamera; �@       // �J�����̐�����]���Q�Ƃ���p
    SetPlayerBool setPlayerBool;

    [SerializeField] private float distance = 5.0f;               // �����Ώۃv���C���[����J�����𗣂�����
    [SerializeField] private float verticalAngle = 20.0f;         // ������]�p�x
    [SerializeField] private float minVerticalAngle = 10.0f;      // ������]�̍ŏ��p�x
    [SerializeField] private float maxVerticalAngle = 50.0f;      // ������]�̍ő�p�x
    [SerializeField] private Quaternion vRotation;                // �J�����̐�����](�����낵��])
    [SerializeField] public Quaternion hRotation;                 // �J�����̐�����]
    [SerializeField] private float turnSpeed = 5.0f;              // ��]���x
    [SerializeField] private Vector3 velocity;                    // �ړ�����
    private float moveSpeed = 30.0f;                              // �ړ����x
    public bool isShaking = false;                                       // �J�������U�����Ă��邩

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.GetComponent<Transform>();
        setPlayerBool = player.GetComponent<SetPlayerBool>();

        // ��]�̏�����
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        vRotation = Quaternion.Euler(verticalAngle, 0, 0);  // �����̐�����]
        hRotation = Quaternion.identity;                    // �����̐�����]
        transform.rotation = hRotation * vRotation;         // ������]

        // �ʒu�̏�����
        transform.position = playerTransform.position - transform.rotation * Vector3.forward * distance;
    }

    void Update()
    {
        if (isShaking == false)
        {
            RotationCamera();
            if (setPlayerBool.isDead == true)
            {
                GetVelocity();
                CameraMovement();
            }
            else
            {
                TrackingTarget();
            }
        }       
    }

    // �J�����̉�]�̐���
    void RotationCamera()
    {
        // ������]�̍X�V
        if (Input.GetMouseButton(0))
        {
            hRotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * turnSpeed, 0);

            if (setPlayerBool.isDead == true)
            {
                var minVerticalAngle = -90.0f;         // �J�����Ɨ���̐�����]�̍ŏ��p�x
                var maxVerticalAngle = 90.0f;      // �J�����Ɨ���̐�����]�̍ő�p�x
                // ������]�̍X�V
                verticalAngle -= Input.GetAxis("Mouse Y") * turnSpeed;
                verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle); // ������K�p
            }
            else
            {
                // ������]�̍X�V
                verticalAngle -= Input.GetAxis("Mouse Y") * turnSpeed;
                verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle); // ������K�p
            }            

            vRotation = Quaternion.Euler(verticalAngle, 0, 0);
        }

        // �J�����̉�](transform.rotation)�̍X�V
        transform.rotation = hRotation * vRotation;
    }

    // �������ꂽ�ړ��L�[�ɉ����ăx�N�g�����擾
    void GetVelocity()
    {
        // WASD���͂���AXZ����(�����Ȓn��)���ړ��������(velocity)�𓾂܂�
        velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            velocity.z += 1;
        if (Input.GetKey(KeyCode.A))
            velocity.x -= 1;
        if (Input.GetKey(KeyCode.S))
            velocity.z -= 1;
        if (Input.GetKey(KeyCode.D))
            velocity.x += 1;
        if (Input.GetKey(KeyCode.Space))
            velocity.y += 1;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            velocity.y -= 1;

        // ���x�x�N�g���̒�����1�b��moveSpeed�����i�ނ悤�ɒ������܂�
        velocity = velocity.normalized * moveSpeed * Time.deltaTime;
    }

    // �擾�����x�N�g���̕����Ɉړ�������
    // �擾�����x�N�g���̕����Ɉړ�&��]������+�W�����v����
    void CameraMovement()
    {
        // �����ꂩ�̕����Ɉړ����Ă���ꍇ
        if (velocity.magnitude > 0)
        {
            transform.position += refCamera.hRotation * velocity;
        }
    }

    // �w�肵���I�u�W�F�N�g��ǐՂ��鏈��
    void TrackingTarget()
    {
        // �J�����̈ʒu(transform.position)�̍X�V
        transform.position = playerTransform.position + new Vector3(0, 1.5f, 0) - transform.rotation * Vector3.forward * distance;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        isShaking = true;

        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = originalPosition + Random.insideUnitSphere * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;

        isShaking = false;
        yield break;
    }
}
