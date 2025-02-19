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
        // GetAxisRaw‚ðŽg‚Á‚ÄˆÚ“®‚·‚é•ûŒü‚ðŽæ“¾
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 movingDirection = new Vector3(x, 0, z);
        // ŽÎ‚ßˆÚ“®‚ª‘¬‚­‚È‚ç‚È‚¢‚æ‚¤‚É‚·‚é
        movingDirection.Normalize();

        movingVelocity = movingDirection * moveSpeed;

        transform.position += movingVelocity * moveSpeed * Time.deltaTime;
    }
}
