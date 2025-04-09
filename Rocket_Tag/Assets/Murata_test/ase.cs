using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ase : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    private CharacterController controller;
    private Camera mainCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * input.z + camRight * input.x;

        if (move.magnitude > 0.1f)
        {
            // ÉvÉåÉCÉÑÅ[ÇÃâÒì]
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}

