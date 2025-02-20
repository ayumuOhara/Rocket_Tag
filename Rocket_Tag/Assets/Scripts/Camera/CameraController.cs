using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    GameObject player;
    Transform playerTransform;                                    // �����Ώۃv���C���[
    Transform playerRightHandTransform;
    [SerializeField] private CameraController refCamera; �@       // �J�����̐�����]���Q�Ƃ���p
    SetPlayerBool setPlayerBool;
    PlayerMovement playerMovement;

    [SerializeField] private float distance = 2.0f;               // �����Ώۃv���C���[����J�����𗣂�����
    [SerializeField] private float verticalAngle = 20.0f;         // ������]�p�x
    [SerializeField] private float minVerticalAngle = 20.0f;      // ������]�̍ŏ��p�x
    [SerializeField] private float maxVerticalAngle = 50.0f;      // ������]�̍ő�p�x
    [SerializeField] private Quaternion vRotation;                // �J�����̐�����](�����낵��])
    [SerializeField] public  Quaternion hRotation;                // �J�����̐�����]
    [SerializeField] private float turnSpeed = 5.0f;              // ��]���x
    [SerializeField] private Vector3 velocity;                    // �ړ�����
    private float moveSpeed = 30.0f;                              // �ړ����x
    private float aimMoveSpeed = 2.0f;                              // �ړ����x
    private float tmpPlayerMoveSpeed;                             // �f�t�H���g�v���C���[�ړ����x
    private float aimDis = 3.2f;                                  //  ADS���̃J��������
    private float tmpDis = 5.0f;                                  //  �f�t�H���g�̃J�����ʒu
    private float minAimVerticalAngle = -20f;                     //  ADS���̃J��������
    private float tmpMinverticalAngle = 20f;                      //  �f�t�H���g�̃J�����̍Œ�p�x
    public bool isShaking = false;                                //  �J�������U�����Ă��邩
    public bool isAiming = false;                                 //  �G�C������

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        setPlayerBool = player.GetComponent<SetPlayerBool>();
        playerTransform = player.GetComponent<Transform>();
        playerRightHandTransform = GameObject.Find("RightHand").GetComponent<Transform>();
        playerMovement = player.GetComponent<PlayerMovement>();

        // ��]�̏�����
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        vRotation = Quaternion.Euler(verticalAngle, 0, 0);  // �����̐�����]
        hRotation = Quaternion.identity;                    // �����̐�����]
        transform.rotation = hRotation * vRotation;         // ������]

        tmpPlayerMoveSpeed = playerMovement.GetMoveSpeed();
        // �ʒu�̏�����
        transform.position = playerTransform.position - transform.rotation * Vector3.forward * distance;

        // �}�E�X�J�[�\������ʓ��͈̔͂̂ݓ�������悤�ɂ���
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        CursorVisible();

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

    void CursorVisible()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }

    }

    // �J�����̉�]�̐���
    void RotationCamera()
    {
        // ������]�̍X�V
        if (!Input.GetKey(KeyCode.LeftAlt))
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
        if(!isAiming)
        {
            distance = tmpDis;
           // playerMovement.SetMoveSpeed(tmpPlayerMoveSpeed);
            // �J�����̈ʒu(transform.position)�̍X�V
            transform.position = playerTransform.position + new Vector3(0, 1.0f, 0) - transform.rotation * Vector3.forward * distance;
        }
        else
        {
            distance = aimDis;
            minVerticalAngle = minAimVerticalAngle;
           // playerMovement.SetMoveSpeed(aimMoveSpeed);
            //transform.position = playerTransform.position + new Vector3(-1.02f, 1.74f, 2.75f) - transform.rotation * Vector3.forward * distance;
            //transform.position = playerTransform.position + new Vector3(playerTransform.position.x + 1, playerTransform.position.y, 0f) - transform.rotation * Vector3.forward * distance;
            //transform.position = Vector3.Lerp(transform.position, playerTransform.position + new Vector3(playerRightHandTransform.localPosition.x + 0.2f, playerRightHandTransform.localPosition.y + 0.8f, 0f) - transform.rotation * Vector3.forward * distance, 20 * Time.deltaTime); 
            transform.position = playerTransform.position + new Vector3(playerRightHandTransform.localPosition.x + 0.2f, playerRightHandTransform.localPosition.y + 0.8f, 0f) - transform.rotation * Vector3.forward * distance; 
        }
        
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
        Debug.Log("�U���I��");
        isShaking = false;
        transform.position = originalPosition;
        Debug.Log($"isShaking�F{isShaking}");
        yield break;
    }
}
