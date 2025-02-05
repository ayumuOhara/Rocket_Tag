using System.Collections;
using UnityEngine;

public class TeleportOnTouch : MonoBehaviour
{
    [Header("ワープ設定")]
    [Tooltip("ワープ先のTransform")]
    [SerializeField] private Transform teleportDestination; // ワープ先

    [Tooltip("ワープ対象のタグ")]
    [SerializeField] private string targetTag = "Player";   // ワープ対象のタグ

    [Header("オプション設定")]
    [Tooltip("ワープ後に速度をリセットするか")]
    [SerializeField] private bool resetVelocity = true; // 速度をリセットするか

    [Tooltip("ワープエフェクト (任意)")]
    [SerializeField] private ParticleSystem teleportEffect; // ワープエフェクト

    [Tooltip("ワープ時の音 (任意)")]
    [SerializeField] private AudioClip teleportSound;       // ワープ時の音

    [Tooltip("ワープ後のクールダウン時間 (秒)")]
    [SerializeField] private float teleportCooldown = 3.0f; // ワープ後のクールダウン時間

    private AudioSource audioSource;
    private bool canTeleport = true; // ワープ可能フラグ

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
        canTeleport = false; // ワープを一時的に無効化

        // エフェクト再生
        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, target.position, Quaternion.identity);
        }

        // ワープ実行
        target.position = teleportDestination.position;

        // ワープ時の音を再生
        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        // 速度リセット
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null && resetVelocity)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // クールダウン時間待機
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true; // ワープ可能に戻す
    }
}
