using System.Collections;
using UnityEngine;

public class TeleportOnTouch : MonoBehaviour
{
    [Header("���[�v�ݒ�")]
    [Tooltip("���[�v���Transform")]
    [SerializeField] private Transform teleportDestination; // ���[�v��

    [Tooltip("���[�v�Ώۂ̃^�O")]
    [SerializeField] private string targetTag = "Player";   // ���[�v�Ώۂ̃^�O

    [Header("�I�v�V�����ݒ�")]
    [Tooltip("���[�v��ɑ��x�����Z�b�g���邩")]
    [SerializeField] private bool resetVelocity = true; // ���x�����Z�b�g���邩

    [Tooltip("���[�v�G�t�F�N�g (�C��)")]
    [SerializeField] private ParticleSystem teleportEffect; // ���[�v�G�t�F�N�g

    [Tooltip("���[�v���̉� (�C��)")]
    [SerializeField] private AudioClip teleportSound;       // ���[�v���̉�

    [Tooltip("���[�v��̃N�[���_�E������ (�b)")]
    [SerializeField] private float teleportCooldown = 3.0f; // ���[�v��̃N�[���_�E������

    private AudioSource audioSource;
    private bool canTeleport = true; // ���[�v�\�t���O

    private void Start()
    {
        if (teleportSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canTeleport && other.CompareTag(targetTag) && teleportDestination != null)
        {
            StartCoroutine(Teleport(other.transform));
        }
    }

    private IEnumerator Teleport(Transform target)
    {
        canTeleport = false; // ���[�v���ꎞ�I�ɖ�����

        // �G�t�F�N�g�Đ�
        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, target.position, Quaternion.identity);
        }

        // ���[�v���s
        target.position = teleportDestination.position;

        // ���[�v���̉����Đ�
        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        // ���x���Z�b�g
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null && resetVelocity)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // �N�[���_�E�����ԑҋ@
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true; // ���[�v�\�ɖ߂�
    }
}
