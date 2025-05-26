using Photon.Pun;
using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    SetPlayerBool setPlayerBool;
    SkillManager skillManager;

    [SerializeField] private Vector3 movingVelocity;             // 移動方向
    [SerializeField] private float moveSpeed = 7.5f;            // 移動速度
    [SerializeField] private float defaultMoveSpeed = 7.5f;     // 通常の移動速度
    [SerializeField] private float applySpeed = 0.2f;            // 回転の適用速度
    [SerializeField] private float jumpForce = 20.0f;            // ジャンプ力
    private bool isGround = false;                               // 接地判定
    private float groundLimit = 0.7f;                            // 接地判定のしきい値
    [SerializeField] private CameraController refCamera;      　 // カメラの水平回転を参照する用

    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider _collider;
    [SerializeField] PhysicsMaterial defaultFriction;       // 通常状態の摩擦
    [SerializeField] PhysicsMaterial noneFriction;          // 方向キー入力中の摩擦

    [SerializeField] private float acceleration = 30f;  // 加速度
    [SerializeField] private float deceleration = 25f;  // 減速度
    private Vector3 currentVelocity = Vector3.zero;     // 実際に使う現在の移動速度

    float stunTime = 1.5f;                                  // スタン時間
    bool isDash = false;                                    // ダッシュ中か
    bool isReverse = false;                                 // 操作反転中か

    Animator animator;
    [SerializeField] private AudioSource footstepAudioSource; // 足音用のAudioSource
    //private bool isPlayingFootstep = false;  // 足音SEの再生管理


    void Start()
    {
        if(photonView.IsMine)
        {
            refCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CameraController>();
            setPlayerBool = GetComponent<SetPlayerBool>();
            skillManager = GetComponent<SkillManager>();
            animator = GetComponent<Animator>();
        }
    }

    [PunRPC]
    public void SetMoveSpeed(float _moveSpeed)
    {
        moveSpeed = _moveSpeed;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetDefaultMoveSpeed()
    {
        return defaultMoveSpeed;
    }

    // 押下された移動キーに応じてベクトルを取得
    public void GetVelocity()
    {
        movingVelocity = Vector3.zero;
        // GetAxisRawを使って移動する方向を取得
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if(isReverse)
        {
            x *= -1;
            z *= -1;
        }
        else
        {
            x *= 1;
            z *= 1;
        }
        
        Vector3 movingDirection = new Vector3(x, 0, z);
        // 斜め移動が速くならないようにする
        movingDirection.Normalize();

        if(setPlayerBool.hasRocket)
        {
            movingVelocity = movingDirection * moveSpeed * 1.25f;
        }
        else
        {
            movingVelocity = movingDirection * moveSpeed;
        }
    }

    // 取得したベクトルの方向に移動&回転させる+ジャンプ処理
    public void PlayerMove()
    {
        if (setPlayerBool != null && setPlayerBool.isStun)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        RunAnimation();
        HandleFootstepSE();

        // カメラ方向を反映
        Vector3 cameraForward = refCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        Vector3 cameraRight = refCamera.transform.right;
        Vector3 adjustedVelocity = cameraForward * movingVelocity.z + cameraRight * movingVelocity.x;

        // 加速または減速
        if (adjustedVelocity.magnitude > 0.1f)
        {
            _collider.material = noneFriction;
            currentVelocity = Vector3.MoveTowards(currentVelocity, adjustedVelocity, acceleration * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, applySpeed);
        }
        else
        {
            _collider.material = defaultFriction;
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);
    }


    void RunAnimation()
    {
        if(movingVelocity.magnitude > 0 && photonView.IsMine)
        {
            if (setPlayerBool.hasRocket)
            {
                animator.SetBool("RunTagger", true);
                animator.SetBool("RunRunner", false);
            }
            else
            {
                animator.SetBool("RunRunner", true);
                animator.SetBool("RunTagger", false);
            }
        }
        else
        {
            animator.SetBool("RunTagger", false);
            animator.SetBool("RunRunner", false);
        }
    }

    // 🆕 足音SEを管理する関数
    void HandleFootstepSE()
    {
        if (movingVelocity.magnitude > 0 && photonView.IsMine)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Play(); // 足音SEを再生
            }
        }
        else
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop(); // 足音SEを停止
            }
        }
    }

    public void JumpAction()
    {
        // ジャンプ処理
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            isGround = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // 衝突判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // 接触点の法線が上向き（地面）に近い場合のみ接地判定を行う
                if (Vector3.Dot(contact.normal, Vector3.up) > groundLimit)
                {
                    isGround = true;
                    break; // 接地を検出したらループを終了
                }
            }
        }

        //if(isDash && collision.gameObject.CompareTag("Player"))
        //{
        //    skillManager.KnockBackTarget(collision.gameObject);
        //}
    }

    // タッチされたときに停止
    public IEnumerator StunPlayer()
    {
        _collider.material = defaultFriction;

        yield return new WaitForSeconds(stunTime);
        photonView.RPC("SetIsStun", RpcTarget.All, false);

        yield break;
    }

    // 操作反転
    public IEnumerator ReverseControll()
    {
        isReverse = true;

        yield return new WaitForSeconds(stunTime);

        isReverse = false;

        yield break;
    }
}
