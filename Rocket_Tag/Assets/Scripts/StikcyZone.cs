using UnityEngine;

public class StikcyZone : MonoBehaviour
{
    bool onPlayer = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            onPlayer = true;
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            float playerSpeed = player.GetDefaultMoveSpeed();
            player.SetMoveSpeed(playerSpeed * 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            onPlayer = false;
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            float playerSpeed = player.GetDefaultMoveSpeed();
            player.SetMoveSpeed(playerSpeed);
        }
    }
}
