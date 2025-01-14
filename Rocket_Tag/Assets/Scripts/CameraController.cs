using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject player;
    Vector3 targetPos;         // 回転の中心点
    Vector3 cameraPos;         // 回転の軸
    float mouseMoveX;          // X軸のマウス移動量
    float mouseSensitivity;    // マウス感度

    private void Awake()
    {
        targetPos = player.transform.position;
        cameraPos = this.transform.position;
        mouseSensitivity = 5.0f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = player.transform.position;

        mouseMoveX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.RotateAround(targetPos, cameraPos, mouseMoveX);
    }
}
