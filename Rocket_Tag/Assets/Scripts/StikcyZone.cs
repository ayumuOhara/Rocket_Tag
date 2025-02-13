using UnityEngine;

public class StikcyZone : MonoBehaviour
{
    bool onPlayer = false;
    float time = 0;

    private void Update()
    {
        time += Time.deltaTime;
        Debug.Log(time);
        if(time > 5.0f && !onPlayer)
        {
            Destroy(gameObject);
        }
    }

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
