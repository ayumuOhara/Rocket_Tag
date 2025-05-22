using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FanRotation : MonoBehaviour
{
    public float centerAngle = 0f;
    public float swingAngle = 45f;
    public float swingSpeed = 2f;
    public float rotationY = 0f;

    private float time;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // âÒì]ÇæÇØéËìÆÇ≈êßå‰
    }

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime * swingSpeed;
        float angle = centerAngle + Mathf.Sin(time) * swingAngle;

        Quaternion targetRotation = Quaternion.Euler(0, rotationY, angle);
        rb.MoveRotation(targetRotation);
    }
}