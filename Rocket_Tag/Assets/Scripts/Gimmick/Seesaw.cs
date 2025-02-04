using Photon.Realtime;
using UnityEngine;

public class Seesaw:MonoBehaviour
{
    [Header("ŒX‚«‚Ìİ’è")]
    public float tiltSpeed = 5f;      // ŒX‚­‘¬‚³
    public float maxTiltAngle = 15f;  // Å‘åŒXÎŠp“x

    private Transform player;
    private Quaternion originalRotation;

    void Start()
    {
        // ‰Šú‚Ì‰ñ“]‚ğ•Û‘¶
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (player != null)
        {
            // ƒvƒŒƒCƒ„[‚ªæ‚Á‚Ä‚¢‚éˆÊ’u‚ğæ“¾
            Vector3 direction = (player.position - transform.position).normalized;

            // X²‚ÆZ²‚ÌŒX‚«‚ğ§ŒÀ
            float tiltX = Mathf.Clamp(direction.z * maxTiltAngle, -maxTiltAngle, maxTiltAngle);
            float tiltZ = Mathf.Clamp(-direction.x * maxTiltAngle, -maxTiltAngle, maxTiltAngle);

            // V‚µ‚¢‰ñ“]‚ğ“K—p (Œ³‚Ì‰ñ“]‚ğŠî€‚É‚·‚é)
            Quaternion targetRotation = Quaternion.Euler(tiltX, 0, tiltZ) * originalRotation;

            // ™X‚ÉŒX‚¯‚é
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
        }
        else
        {
            // ƒvƒŒƒCƒ„[‚ª~‚è‚½‚çŒ³‚ÌŠp“x‚É–ß‚é
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * tiltSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }
}
