using UnityEngine;

public class FanRotation : MonoBehaviour
{
    // U่q^ฎฬSpx (๚px)
    public float centerAngle = 0f;

    // U่ (ท้ลๅpx)
    public float swingAngle = 45f;

    // U่qฬฌx
    public float swingSpeed = 2f;

    private float time;

    void Update()
    {
        // ิ๐gมฤX[YศU่q^ฎ๐vZ
        time += Time.deltaTime * swingSpeed;

        // U่qฬpx๐vZ (singล^ฎ)
        float angle = centerAngle + Mathf.Sin(time) * swingAngle;

        // vZณ๊ฝpx๐Kp
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
