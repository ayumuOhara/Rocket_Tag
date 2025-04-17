using UnityEngine;
using Photon.Pun;

public class MoveUpDown : MonoBehaviourPun
{
    [Header("移動設定")]
    public float moveRange = 2f;
    public float moveSpeed = 2f;
    public float stopDurationTop = 1f;
    public float stopDurationBottom = 0.5f;

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
        if (photonView.IsMine) // 自分のプレイヤーだけが動作ロジックを持つ
        {
            UpdateMovementLogic();
        }
    }

    void UpdateMovementLogic()
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
