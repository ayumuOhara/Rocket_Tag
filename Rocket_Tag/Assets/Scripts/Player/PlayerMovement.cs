using Photon.Pun;
using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    SetPlayerBool setPlayerBool;
    ChangeObjColor changeObjColor;

    [SerializeField] private Vector3 movingVelocity;        // �ړ�����
    [SerializeField] private float moveSpeed = 10.0f;       // �ړ����x
    [SerializeField] private float applySpeed = 0.2f;       // ��]�̓K�p���x
    [SerializeField] private float jumpForce = 20.0f;       // �W�����v��
    private bool isGround = false;                          // �ڒn����
    private float groundLimit = 0.7f;                       // �ڒn����̂������l
    [SerializeField] private CameraController refCamera; �@ // �J�����̐�����]���Q�Ƃ���p

    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider _collider;
    [SerializeField] PhysicsMaterial defaultFriction;       // �ʏ��Ԃ̖��C
    [SerializeField] PhysicsMaterial noneFriction;          // �����L�[���͒��̖��C

    float stunTime = 3.0f;                                  // �X�^������

    void Start()
    {
        setPlayerBool = GetComponent<SetPlayerBool>();
        refCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CameraController>();
        changeObjColor = GetComponent<ChangeObjColor>();
    }

    public void SetMoveSpeed(float _moveSpeed)
    {
        moveSpeed = _moveSpeed;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    // �������ꂽ�ړ��L�[�ɉ����ăx�N�g�����擾
    public void GetVelocity()
    {
        movingVelocity = Vector3.zero;
        // GetAxisRaw���g���Ĉړ�����������擾
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 movingDirection = new Vector3(x, 0, z);
        // �΂߈ړ��������Ȃ�Ȃ��悤�ɂ���
        movingDirection.Normalize();

        if (setPlayerBool.hasRocket)
            moveSpeed = 12f;
        else
            moveSpeed = 10f;

        movingVelocity = movingDirection * moveSpeed;
    }

    // �擾�����x�N�g���̕����Ɉړ�&��]������+�W�����v����
    public void PlayerMove()
    {
        // �����ꂩ�̕����Ɉړ����Ă���ꍇ
        if (movingVelocity.magnitude > 0)
        {
            _collider.material = noneFriction;

            // �J�����̑O������XZ���ʂɓ��e
            Vector3 cameraForward = refCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            // �J�����̉E�������擾
            Vector3 cameraRight = refCamera.transform.right;

            // �J������ňړ��������Čv�Z
            Vector3 adjustedVelocity = cameraForward * movingVelocity.z + cameraRight * movingVelocity.x;

            // �v���C���[�̉�](transform.rotation)�̍X�V
            if (adjustedVelocity.magnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(adjustedVelocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, applySpeed);
            }

            // �v���C���[�̈ʒu�̍X�V
            rb.linearVelocity = new Vector3(adjustedVelocity.x, rb.linearVelocity.y, adjustedVelocity.z);
        }
        else
        {
            _collider.material = defaultFriction; 
        }
    }

    public void JumpAction()
    {
        // �W�����v����
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            isGround = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // �Փ˔���
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // �ڐG�_�̖@����������i�n�ʁj�ɋ߂��ꍇ�̂ݐڒn������s��
                if (Vector3.Dot(contact.normal, Vector3.up) > groundLimit)
                {
                    isGround = true;
                    break; // �ڒn�����o�����烋�[�v���I��
                }
            }
        }
    }

    // �^�b�`���ꂽ�Ƃ��ɒ�~
    public IEnumerator StunPlayer()
    {
        _collider.material = defaultFriction;

        photonView.RPC("ChangeColor", RpcTarget.All,
                        changeObjColor.colorMaterial[3].color.r,
                        changeObjColor.colorMaterial[3].color.g,
                        changeObjColor.colorMaterial[3].color.b,
                        changeObjColor.colorMaterial[3].color.a);

        yield return new WaitForSeconds(stunTime);
        photonView.RPC("SetIsStun", RpcTarget.All, false);

        photonView.RPC("ChangeColor", RpcTarget.All,
                        changeObjColor.colorMaterial[0].color.r,
                        changeObjColor.colorMaterial[0].color.g,
                        changeObjColor.colorMaterial[0].color.b,
                        changeObjColor.colorMaterial[0].color.a);

        yield break;
    }
}
