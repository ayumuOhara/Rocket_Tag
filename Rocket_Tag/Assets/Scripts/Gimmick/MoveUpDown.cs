using UnityEngine;

public class MoveUpDown : MonoBehaviour
{
    [Header("ˆÚ“®İ’è")]
    public float moveRange = 2f;   // ã‰ºˆÚ“®”ÍˆÍ
    public float moveSpeed = 2f;   // ˆÚ“®‘¬“x

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // ã‰º‚ÉŒJ‚è•Ô‚µˆÚ“®‚·‚é“®‚«
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
