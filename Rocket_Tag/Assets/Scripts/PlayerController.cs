using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject player;     // プレイヤーのGameObjectを取得
    [SerializeField] GameObject cameraObj;  // カメラのGameObjectを取得
    float moveSpeed;                        // プレイヤーの移動速度

    void Start()
    {
        moveSpeed = 10.0f;
    }

    void Update()
    {
        // WASDキーで移動
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
