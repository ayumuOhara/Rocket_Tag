using Photon.Pun;
using UnityEngine;
using System.Collections;
using System;

public class Alpha_Rocket : MonoBehaviourPunCallbacks
{
    // ロケットのカウント減少レベルを定義
    enum DecreeseLevel
    {
        slowest,   // 最も遅い
        veryslow,
        slow,
        normal,
        fast,
        velyfast,
        fastest    // 最も速い
    }
    DecreeseLevel decreeseLevel = DecreeseLevel.slowest;

    // ロケット関連のパラメータ
    float rocketLimit = 0;                  // ロケットの最低カウント
    public float rocketCount = 100;        // 現在のロケットカウント
    public float initialCount = 100;       // 初期のロケットカウント
    float vibingPower = 0.2f;              // カメラ振動の強さ
    float vibingDuration = 2;              // カメラ振動の持続時間
    float[] vibeStartTime = { 2, 3.2f, 6, 12, 18, 24, 35 }; // 振動開始時間
    float floatStartTime = 2;              // 浮遊開始時間
    float floatSpeed = 2;                  // 浮遊の速度
    float explodeRiseSpeed = 18;           // 爆発後の上昇速度
    float possesingTime = 0;               // ロケット保持時間
    float secToExplode = 0;                // 爆発までの時間
    float evacuateStarPos_Y = 40;          // 高度制限（脱出基準）
    float[] decreeseValue = { 0.4f, 1, 1.8f, 5, 12, 30, 100 }; // カウント減少値
    float[] decreeseUpTime = { 5, 10, 15, 20, 25, 30, 35 };    // 減少レベルアップの時間
    bool isExplode = false;                // 爆発中フラグ
    bool isShaking = false;                // カメラ振動フラグ

    // 各種オブジェクトやコンポーネント
    Vector3 startPos;
    [SerializeField] GameObject player;    // プレイヤーオブジェクト
    [SerializeField] GameObject camera;    // カメラオブジェクト
    Transform playerTransform;             // プレイヤーのTransform
    Rigidbody playerRB;                    // プレイヤーのRigidbody
    GameManager gameManager;               // ゲームマネージャー
    CameraController cameraController;     // カメラコントローラー

    // 初期化処理
    void Start()
    {
        Initialize();
    }

    // 毎フレームの更新処理
    void Update()
    {
        CountElaps(); // ロケットカウントの経過を計算

        // カメラ振動処理
        if (IsVibeTime() && !isShaking)
        {
            isShaking = true;
            StartCoroutine(Vibration());
        }

        // 浮遊処理（爆発中は無効化）
        if (isFloatingTime() && !IsVeryHigh() && !isExplode)
        {
            SetGravity(playerRB, false); // 重力を無効化
            Floating(playerTransform, floatSpeed);
        }

        // 爆発判定
        if (IsLimitOver() && !isExplode)
        {
            isExplode = true; // 爆発中フラグをセット
            StartCoroutine(Explosion());
            ResetRocketCount();
            ResetPossesing();
        }

        // 減少レベルアップ判定
        if (decreeseLevel != DecreeseLevel.fastest && possesingTime > decreeseUpTime[(int)decreeseLevel])
        {
            DecreeseLevelUp();
        }
    }

    // 初期設定
    void Initialize()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerTransform = player.transform;
        startPos = playerTransform.position;
        SetEvacuatePos(40); // 高度制限を設定
        secToExplode = GetSecUntilZero(rocketCount, Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime, Time.deltaTime);
        camera = GameObject.Find("PlayerCamera");
        playerRB = player.GetComponent<Rigidbody>();
        cameraController = camera.GetComponent<CameraController>();
        UpdateRocketCount(rocketCount); // 初期ロケットカウントを同期
    }

    // 0になるまでの時間を計算
    float GetSecUntilZero(float limit, float minusValue, float runUnit)
    {
        return limit / (minusValue * (1 / runUnit));
    }

    // ロケットカウントの同期
    public void UpdateRocketCount(float newRocketCount)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "RocketCount", rocketCount } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    // ロケットカウントの経過処理
    void CountElaps()
    {
        rocketCount -= Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime;
        possesingTime += Time.deltaTime;
        UpdateRocketCount(rocketCount);
    }

    // カメラ振動が必要な時間か判定
    bool IsVibeTime()
    {
        return vibeStartTime[(int)decreeseLevel] > rocketCount;
    }

    // 浮遊開始時間か判定
    bool isFloatingTime()
    {
        return floatStartTime > rocketCount;
    }

    // 高度制限を超えたか判定
    bool IsVeryHigh()
    {
        return playerTransform.position.y > evacuateStarPos_Y;
    }

    // 重力の有効/無効を設定
    void SetGravity(Rigidbody rB, bool value)
    {
        rB.useGravity = value;
    }

    // 浮遊処理
    void Floating(Transform floated, float floatForce)
    {
        floated.position += Vector3.up * floatForce * Time.deltaTime;
    }

    // ロケットカウントがリミットを超えたか判定
    bool IsLimitOver()
    {
        return rocketLimit > rocketCount;
    }

    // 爆発処理
    IEnumerator Explosion()
    {
        DropOut(); // 脱落処理

        // 高度制限を超えるまで上昇
        while (!IsVeryHigh())
        {
            Floating(playerTransform, explodeRiseSpeed);
            yield return null;
        }

        isExplode = false; // 爆発終了後にフラグ解除
    }

    // 脱落処理
    void DropOut()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(gameManager.ChooseRocketPlayer());
        }

        PhotonView targetPhotonView = player.GetComponent<PhotonView>();
        if (targetPhotonView != null)
        {
            targetPhotonView.RPC("SetPlayerDead", RpcTarget.All, true);
        }
    }

    // カメラ振動処理
    IEnumerator Vibration()
    {
        yield return cameraController.Shake(vibingDuration, vibingPower);
        isShaking = false; // 振動終了後フラグを解除
    }

    // 減少レベルアップ
    void DecreeseLevelUp()
    {
        decreeseLevel += 1;
        secToExplode = GetSecUntilZero(rocketCount, Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime, Time.deltaTime);
    }

    // ロケットカウントをリセット
    void ResetRocketCount()
    {
        rocketCount = initialCount;
        UpdateRocketCount(rocketCount);
    }

    // ロケット保持時間をリセット
    public void ResetPossesing()
    {
        possesingTime = 0;
        decreeseLevel = 0;
    }

    // カウント同期時の更新処理
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("RocketCount"))
        {
            rocketCount = Convert.ToSingle(changedProps["RocketCount"]);
        }
    }

    // 高度制限を設定
    void SetEvacuatePos(float farFromStartPos)
    {
        evacuateStarPos_Y = startPos.y + farFromStartPos;
    }
}
