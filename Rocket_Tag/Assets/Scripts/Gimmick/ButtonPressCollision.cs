using UnityEngine;

public class ButtonPressCollision : MonoBehaviour
{
    private Vector3 originalPosition;
    public float pressDepth = 0.2f; // �ւ��ސ[��
    private bool isPressed = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        // �v���C���[�̃^�O��"Player"�Ɖ���
        if (collision.gameObject.CompareTag("Player") && !isPressed)
        {
            isPressed = true;
            transform.position = originalPosition - new Vector3(0, pressDepth, 0);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isPressed)
        {
            isPressed = false;
            transform.position = originalPosition;
        }
    }
}