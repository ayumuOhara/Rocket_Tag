using UnityEngine;

public class MoveUpDown : MonoBehaviour
{
    [Header("ˆÚ“®Ý’è")]
    public float moveRange = 2f;              // ã‰ºˆÚ“®”ÍˆÍ
    public float moveSpeed = 2f;              // ˆÚ“®‘¬“x
    public float stopDurationTop = 1f;        // Å‚“_‚Å‚Ì’âŽ~ŽžŠÔ
    public float stopDurationBottom = 0.5f;   // Å’á“_‚Å‚Ì’âŽ~ŽžŠÔ

    private Vector3 startPosition;

    private enum MoveState { MovingUp, MovingDown, Stopping }
    private MoveState currentState = MoveState.MovingUp;

    private float stopTimer = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        switch (currentState)
        {
            case MoveState.MovingUp:
                pos.y += moveSpeed * Time.deltaTime;
                if (pos.y >= startPosition.y + moveRange)
                {
                    pos.y = startPosition.y + moveRange;
                    currentState = MoveState.Stopping;
                    stopTimer = stopDurationTop;
                }
                break;

            case MoveState.MovingDown:
                pos.y -= moveSpeed * Time.deltaTime;
                if (pos.y <= startPosition.y)
                {
                    pos.y = startPosition.y;
                    currentState = MoveState.Stopping;
                    stopTimer = stopDurationBottom;
                }
                break;

            case MoveState.Stopping:
                stopTimer -= Time.deltaTime;
                if (stopTimer <= 0f)
                {
                    currentState = (transform.position.y >= startPosition.y + moveRange)
                        ? MoveState.MovingDown
                        : MoveState.MovingUp;
                }
                break;
        }

        transform.position = pos;
    }
}
