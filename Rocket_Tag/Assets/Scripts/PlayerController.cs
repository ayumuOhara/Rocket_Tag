using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;

// PUN�̃R�[���o�b�N���󂯎���悤�ɂ���ׂ�MonoBehaviourPunCallbacks
public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject rocketObj;          // ���P�b�g

    [SerializeField] private Vector3 velocity;              // �ړ�����
    [SerializeField] private float moveSpeed = 10.0f;       // �ړ����x
    [SerializeField] private float applySpeed = 0.2f;       // ��]�̓K�p���x
    [SerializeField] private float jumpForce = 20.0f;       // �W�����v��
    private bool isGround = false;                          // �ڒn����
    [SerializeField] private CameraController refCamera; �@ // �J�����̐�����]���Q�Ƃ���p
    [SerializeField] Rigidbody rb;
    private string targetTag = "Player";                    // �^�b�`���̌��m�Ώۂ�tag(�������ɂ�Player�ɕύX����)
    public float maxDistance = 1f;                         // ���m����ő勗��
    [SerializeField] private bool hasRocket;                // ���P�b�g���������Ă��邩

    private void Awake()
    {
        // ���P�b�g�̏�Ԃ�������
        photonView.RPC("SetHasRocket", RpcTarget.All, false);
    }

    void Start()
    {   
        refCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CameraController>();
    }

    void Update()
    {
        if(photonView.IsMine)
        {
            GetVelocity();
            MovePlayer();

            if (hasRocket)
            {
                PlayerAction();
            }
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

    // �擾�����x�N�g���̕����Ɉړ�&��]������+�W�����v����
    void MovePlayer()
    {
        // �W�����v����
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            isGround = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("�X�L���P���g�p����");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("�X�L���Q���g�p����");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("���P�b�g�𓊝�����");
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject target = GetTargetDistance();
            if (target != null)
            {
                // ������ hasRocket ��؂�ւ�
                photonView.RPC("ToggleHasRocket", RpcTarget.All, !hasRocket);

                // �^�[�Q�b�g�� hasRocket ��؂�ւ�
                PhotonView targetPhotonView = target.GetComponent<PhotonView>();
                if (targetPhotonView != null)
                {
                    targetPhotonView.RPC("ToggleHasRocket", RpcTarget.All, !target.GetComponent<PlayerController>().hasRocket);
                }
            }
        }
    }

    [PunRPC]
    void ToggleHasRocket(bool newHasRocket)
    {
        hasRocket = newHasRocket;
        rocketObj.SetActive(hasRocket);
        Debug.Log($"hasRocket �� {hasRocket} �ɍX�V���܂���");
    }

    // hasRocket ��ݒ肵�A����
    [PunRPC]
    public void SetHasRocket(bool newHasRocket)
    {
        hasRocket = newHasRocket;
        rocketObj.SetActive(hasRocket);
        Debug.Log($"{photonView.Owner.NickName} �� hasRocket �� {hasRocket} �ɐݒ肵�܂���");
    }

    // ���̃v���C���[�Ƃ̋����𑪂�
    GameObject GetTargetDistance()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        GameObject nearestObject = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            Vector3 direction = target.transform.position - transform.position;
            float distance = direction.magnitude;

            // �������͈͓����`�F�b�N
            if (distance <= maxDistance)
            {
                // Raycast�𔭎˂��ď�Q�����Ȃ����m�F
                Ray ray = new Ray(transform.position, direction.normalized);
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                {
                    if (hit.collider.gameObject == target)
                    {
                        // �ŒZ�������X�V
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestObject = target;
                        }
                    }
                }
            }
        }

        if (nearestObject != null)
        {
            Debug.Log($"�ł��߂��I�u�W�F�N�g: {nearestObject.name}, ����: {nearestDistance}");
        }
        else
        {
            Debug.Log("���m�Ώۂ�������܂���ł���");
        }

        return nearestObject;
    }

    // �ڒn����
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }
}
