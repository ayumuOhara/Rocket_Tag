using UnityEngine;

public class ButtonPressCollision : MonoBehaviour
{
    private Vector3 originalPosition;
    public float pressDepth = 0.2f; // ‚Ö‚±‚Þ[‚³
    private bool isPressed = false;
    public float pressSpeed = 5f; // ‚Ö‚±‚Þ‘¬“x

    void Start()
    {
        originalPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isPressed)
        {
            isPressed = true;
            StopAllCoroutines();
            StartCoroutine(PressButton());
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isPressed)
        {
            isPressed = false;
            StopAllCoroutines();
            StartCoroutine(ReleaseButton());
        }
    }

    private System.Collections.IEnumerator PressButton()
    {
        Vector3 targetPosition = originalPosition - new Vector3(0, pressDepth, 0);
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, pressSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }

    private System.Collections.IEnumerator ReleaseButton()
    {
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, pressSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = originalPosition;
    }
}