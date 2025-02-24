using UnityEngine;

public class TestMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveSpeed = 3.0f;
        Vector3 movingVelocity = Vector3.zero;
        // GetAxisRawを使って移動する方向を取得
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 movingDirection = new Vector3(x, 0, z);
        // 斜め移動が速くならないようにする
        movingDirection.Normalize();

        movingVelocity = movingDirection * moveSpeed;

        transform.position += movingVelocity * moveSpeed * Time.deltaTime;
    }
}
