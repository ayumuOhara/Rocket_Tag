using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class MoveUpDown : MonoBehaviourPun, IPunObservable
{
    [Header("移動設定")]
    public float moveRange = 2f;
    public float moveSpeed = 2f;
    public float stopDurationTop = 1f;
    public float stopDurationBottom = 0.5f;

    private Vector3 startPosition;
    private float stopTimer = 0f;
    private enum MoveState { MovingUp, MovingDown, Stopping }
    private MoveState currentState = MoveState.MovingUp;

    private Vector3 networkPosition;

    void Start()
    {
        startPosition = transform.position;
        networkPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            UpdateMovementLogic();
        }
        else
        {
            // 他プレイヤーの位置をスムーズに補間
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
        }
    }

    private void UpdateMovementLogic()
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

    // Photon同期
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 自分のオブジェクトの位置を送信
            stream.SendNext(transform.position);
        }
        else
        {
            // 他人のオブジェクトの位置を受信
            networkPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
