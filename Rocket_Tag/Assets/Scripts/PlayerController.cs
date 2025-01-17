using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;
using TMPro;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.GraphicsBuffer;

// PUN�̃R�[���o�b�N���󂯎���悤�ɂ���ׂ�MonoBehaviourPunCallbacks
public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject rocketObj;          // ���P�b�g
    [SerializeField]
    List<Material> colorMaterial = new List<Material>();
    // [0] DefaultBodyColor
    // [1] DefaultEyeColor
    // [2] UseSkillBodyColor

    private float skillCT; // �X�L���̃N�[���^�C��(���ł̂݁B�}�X�^�[�łł�CSV�t�@�C�����g�p)
    [SerializeField] TextMeshProUGUI skillTimerText;
    [SerializeField] GameObject skillCTUI;

    [SerializeField] private Vector3 velocity;              // �ړ�����
    private float defaultMoveSpeed = 10.0f;                 // �ړ����x(�����l)
    [SerializeField] private float moveSpeed = 10.0f;       // �ړ����x
    [SerializeField] private float applySpeed = 0.2f;       // ��]�̓K�p���x
    [SerializeField] private float jumpForce = 20.0f;       // �W�����v��
    private bool isGround = false;                          // �ڒn����
    private float groundLimit = 0.7f;                       // �ڒn����̂������l
    [SerializeField] private CameraController refCamera; �@ // �J�����̐�����]���Q�Ƃ���p
    [SerializeField] Rigidbody rb;
    private string targetTag = "Player";                    // �^�b�`���̌��m�Ώۂ�tag(�������ɂ�Player�ɕύX����)
    public float maxDistance = 5;                           // ���m����ő勗��
    [SerializeField] private bool hasRocket;                // ���P�b�g���������Ă��邩
    public bool isDead;                                     // ���S����

    private void Awake()
    {
        SetPlayerCondition();
        skillCTUI = GameObject.Find("SkillCTUI");
        skillTimerText = GameObject.Find("SkillTimerText").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {   
        refCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CameraController>();
        skillCTUI.SetActive(false);
    }

    void Update()
    {
        if(photonView.IsMine)
        {
            GetVelocity();
            MovePlayer();
            if (skillCT >= 0 && finishSkill)
            {
                if (skillCT <= 0)
                {
                    skillCTUI.SetActive(false);
                }
                else
                {
                    // ���ł̂ݎg�p(�}�X�^�[�łł͍폜)
                    SkillCool();
                }
            }

            if (hasRocket)
            {
                PlayerAction();
            }
        }        
    }

    // �v���C���[�̏�����
    public void SetPlayerCondition()
    {
        // ���P�b�g�̏�Ԃ�������
        photonView.RPC("SetHasRocket", RpcTarget.All, false);

        maxDistance = 2f;

        // ���ňȊO�ł͍폜
        skillCT = 0;
    }

    // ���S����
    void PlayerDead()
    {
        isDead = true;
    }


    //--- �v���C���[�̈ړ����� ---//

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


    //--- �v���C���[�̓���A�N�V�������� ---//

    // �������ꂽ�L�[�ɉ����ăA�N�V����
    void PlayerAction()
    {
        UseSkill();
        RocketAction();        
    }
    // �X�L���g�p
    void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"�X�L��CT�F{skillCT}");
            if(skillCT <= 0f)
            {
                Debug.Log("�X�L���P���g�p����");
                StartCoroutine(DashSkill());
            }
            else
            {
                Debug.Log("�X�L���P�̎g�p�����𖞂����Ă��܂���");
            }            
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (skillCT >= 30.0f)
            {
                Debug.Log("�X�L���Q���g�p����");
            }
            else
            {
                Debug.Log("�X�L���Q�̎g�p�����𖞂����Ă��܂���");
            }
        }
    }
    // �^�b�`/�����A�N�V����
    void RocketAction()
    {
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


    //--- �^�b�`�A�N�V�����֌W ---//

    // ���g�� hasRocket ��ύX����Ƃ��̂ݎg�p
    [PunRPC]
    void ToggleHasRocket(bool newHasRocket)
    {
        hasRocket = newHasRocket;
        rocketObj.SetActive(hasRocket);
        Debug.Log($"hasRocket �� {hasRocket} �ɍX�V���܂���");
    }
    // hasRocket ��ݒ肵�A����
    // ���v���C���[���� hasRocket ��ύX����Ƃ��̂ݎg�p
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


    //--- �X�L���֌W ---//

    // �_�b�V���X�L������(�C����)
    IEnumerator DashSkill()
    {
        skillCT = 30.0f;
        finishSkill = false;
        skillCTUI.SetActive(true);
        skillTimerText.text = skillCT.ToString();

        // �X�L�����g�p�������
        moveSpeed *= 3.0f;
        photonView.RPC("ChangeColor", RpcTarget.All, colorMaterial[2].color.r, colorMaterial[2].color.g, colorMaterial[2].color.b, colorMaterial[2].color.a);

        yield return new WaitForSeconds(2.0f);

        // �X�L�����g�p����O�̏��
        moveSpeed = defaultMoveSpeed;
        photonView.RPC("ChangeColor", RpcTarget.All, colorMaterial[0].color.r, colorMaterial[0].color.g, colorMaterial[0].color.b, colorMaterial[0].color.a);

        finishSkill = true;

        yield break;
    }

    // �v���C���[�̐F�ύX (�C����)
    [PunRPC]
    void ChangeColor(float r, float g, float b, float a)
    {
        Color newColor = new Color(r, g, b, a);
        GetComponent<Renderer>().material.color = newColor;
    }

    float time = 0;
    public bool finishSkill = true;

    // �X�L���N�[���^�C���Ǘ�
    void SkillCool()
    {
        time += Time.deltaTime;
        if(time > 1)
        {
            skillCT = Mathf.Clamp(skillCT - 1, 0, 30.0f);
            skillTimerText.text = skillCT.ToString();
            time = 0;
        }
    }
}
