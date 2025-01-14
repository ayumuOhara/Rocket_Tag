using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject player;     // �v���C���[��GameObject���擾
    [SerializeField] GameObject cameraObj;  // �J������GameObject���擾
    float moveSpeed;                        // �v���C���[�̈ړ����x

    void Start()
    {
        moveSpeed = 10.0f;
    }

    void Update()
    {
        // WASD�L�[�ňړ�
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(cameraObj.transform.forward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(cameraObj.transform.forward * -1.0f * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(cameraObj.transform.right * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(cameraObj.transform.right * -1.0f * moveSpeed * Time.deltaTime);
        }
    }
}
