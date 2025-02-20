using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Rotate : MonoBehaviourPun, IPunObservable
{
    private string targetTag = "Player";  // "Player"�^�O�����I�u�W�F�N�g�ɂ̂݉e��
    [SerializeField] private float maxDistance = 5.0f;  // ���m����ő勗���iInspector�Őݒ�j

    [Header("��]�ݒ�")]
    [SerializeField] private float rotationSpeed = 100f;   // ������]���x

    private Quaternion networkRotation; // �l�b�g���[�N�����p
    private HashSet<GameObject> playersOnObject = new HashSet<GameObject>(); // ��]�I�u�W�F�N�g�ɏ���Ă���v���C���[

    void Start()
    {
        networkRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            // ��]������]������
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

            // ��]�I�u�W�F�N�g�ɏ���Ă���v���C���[�ɉe����^����
            ApplyRotationToPlayersOnObject();
        }
        else
        {
            // ���v���C���[�̉�]���Ԃ��ē���
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }

    // ��]�f�[�^�𓯊�
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    // ��]�I�u�W�F�N�g�ɏ���Ă���v���C���[�ɉ�]��^����
    private void ApplyRotationToPlayersOnObject()
    {
        foreach (var player in playersOnObject)
        {
            ApplyRotationToPlayer(player);
        }
    }

    // �v���C���[�ɉ�]���̉e����^����
    private void ApplyRotationToPlayer(GameObject player)
    {
        // �v���C���[�̈ʒu�Ɖ�]���̈ʒu���r
        Vector3 directionToCenter = player.transform.position - transform.position;
        directionToCenter.y = 0; // Y���𖳎����Đ����ʂŉ�]

        // ��]���̉�]�ɍ��킹�āA�e����^����
        float angle = rotationSpeed * Time.deltaTime;
        player.transform.RotateAround(transform.position, Vector3.up, angle);

        // �v���C���[�ɉ�]��^����
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            // �v���C���[�ɏ��̉�]��͕킷��悤�ȗ͂�������
            playerRb.angularVelocity = Vector3.zero; // �����̉�]�����Z�b�g
            playerRb.AddTorque(Vector3.up * rotationSpeed * 0.1f, ForceMode.VelocityChange);
        }
    }

    // �v���C���[����]�I�u�W�F�N�g�ɏ�����ꍇ
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            playersOnObject.Add(collision.gameObject); // ��]�I�u�W�F�N�g�ɏ�����v���C���[���L�^
        }
    }

    // �v���C���[����]�I�u�W�F�N�g����~�肽�ꍇ
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            playersOnObject.Remove(collision.gameObject); // ��]�I�u�W�F�N�g����~�肽�v���C���[���폜
        }
    }

    // ��]�����𔽓]����i�O��������Ăяo����j
    public void ReverseRotation()
    {
        rotationSpeed = -rotationSpeed;
    }
}
