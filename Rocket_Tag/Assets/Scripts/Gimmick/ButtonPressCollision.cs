using UnityEngine;
using System.Collections;

public class ButtonPressCollision : MonoBehaviour
{
    private Vector3 originalPosition;
    public float pressDepth = 0.2f;    // へこむ深さ
    public float pressSpeed = 5f;      // へこむ速度
    public float pressedDuration = 2f; // ボタンから離れた後、戻るまでの時間
    private bool isPressed = false;
    private bool playerOnButton = false; // プレイヤーが乗っているか判定

    void Start()
    {
        originalPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnButton = true;
            if (!isPressed)
            {
                isPressed = true;
                StopAllCoroutines();
                StartCoroutine(PressButton());
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnButton = false;
            StartCoroutine(WaitAndReleaseButton());
        }
    }

    private IEnumerator PressButton()
    {
        Vector3 targetPosition = originalPosition - new Vector3(0, pressDepth, 0);
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, pressSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }

    private IEnumerator WaitAndReleaseButton()
    {
        yield return new WaitForSeconds(pressedDuration);

        // もしプレイヤーがまだ乗っていたら戻さない
        if (playerOnButton) yield break;

        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, pressSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = originalPosition;
        isPressed = false;
    }
}