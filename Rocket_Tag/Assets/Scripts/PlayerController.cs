using UnityEngine;
using Photon.Pun;

// PUN�̃R�[���o�b�N���󂯎���悤�ɂ���ׂ�MonoBehaviourPunCallbacks
public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Vector3 velocity;              // �ړ�����
    [SerializeField] private float moveSpeed = 6.0f;        // �ړ����x
    [SerializeField] private float applySpeed = 0.2f;       // ��]�̓K�p���x
    [SerializeField] private CameraController refCamera; �@ // �J�����̐�����]���Q�Ƃ���p
    [SerializeField] Rigidbody rb;
    [SerializeField] public bool hasRocket { get; private set; }  // ���P�b�g���������Ă��邩

    private void Awake()
    {
        
    }

    void Start()
    {
        hasRocket = false;
        refCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CameraController>();
    }

    void Update()
    {
        if(photonView.IsMine)
        {
            GetVelocity();
            MovePlayer();
            PlayerAction();
        }
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

        // ���x�x�N�g���̒�����1�b��moveSpeed�����i�ނ悤�ɒ������܂�
        velocity = velocity.normalized * moveSpeed * Time.deltaTime;
    }

    // �擾�����x�N�g���̕����Ɉړ�&��]������
    void MovePlayer()
    {
        // �����ꂩ�̕����Ɉړ����Ă���ꍇ
        if (velocity.magnitude > 0)
        {
            // �v���C���[�̉�](transform.rotation)�̍X�V
            // ����]��Ԃ̃v���C���[��Z+����(�㓪��)���A
            // �J�����̐�����](refCamera.hRotation)�ŉ񂵂��ړ��̔��Ε���(-velocity)�ɉ񂷉�]�ɒi�X�߂Â��܂�
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(refCamera.hRotation * -velocity),
                                                  applySpeed);

            // �v���C���[�̈ʒu(transform.position)�̍X�V
            // �J�����̐�����](refCamera.hRotation)�ŉ񂵂��ړ�����(velocity)�𑫂����݂܂�
            transform.position += refCamera.hRotation * velocity;
        }
    }

    // �������ꂽ�L�[�ɉ����ăA�N�V����
    void PlayerAction()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("���P�b�g�𓊝�����");
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("�X�L�����g�p����");
        }

        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log("�^�b�`");
        }
    }
}
