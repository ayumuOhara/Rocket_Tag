using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // ‰ñ“]‘¬“x (1•b‚ ‚½‚è‚Ì‰ñ“]Šp“x)
    public float rotationSpeed = 100f;

    void Update()
    {
        // ‰¡•ûŒü (YŽ²) ‚É‰ñ“]‚³‚¹‚é
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
