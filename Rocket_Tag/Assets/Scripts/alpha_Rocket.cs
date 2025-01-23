using Photon.Pun;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class Alpha_Rocket : MonoBehaviourPunCallbacks
{
    enum DecreeseLevel    //  爆弾カウント減少レベル
    {
        slowest,
        veryslow,
        slow,
        normal,
        fast,
        velyfast,
        fastest
    }
    DecreeseLevel decreeseLevel = DecreeseLevel.slowest;

    float rocketLimit = 0;
    public float rocketCount = 100;
    public float initialCount = 100;
    float vibingPower = 0.2f;
    float vibingDuration = 2;
    float[] vibeStartTime = { 2, 3.2f, 6, 12, 18, 24, 35 };
    float floatStartTime = 2;
    float floatSpeed = 2;
    float explodeRiseSpeed = 18;
    float possesingTime = 0;
    float secToExplode = 0;
    float evacuateStarPos_Y = 40;
    float[] decreeseValue = { 0.4f, 1, 1.8f, 5, 12, 30, 100 };
    float[] decreeseUpTime = { 5, 10, 15, 20, 25, 30, 35 };
    bool isExplode = false;

    Vector3 startPos;

    [SerializeField] GameObject player;
    [SerializeField] GameObject _camera;
    Transform playerTransform;
    Rigidbody playerRB;
    GameManager gameManager;
    CameraController cameraController;

    void Start()
    { 
        Initialize(); 
    }

    void Update()
    {
        CountElaps();
        if(IsVibeTime())
        { 
            StartCoroutine(cameraController.Shake(vibingDuration, vibingPower)); 
        }
        if (isFloatingTime() && !IsVeryHigh())
        {
            SetGravity(playerRB, false);
            Floating(playerTransform, floatSpeed);
        }
        if (IsLimitOver())
        {
            ResetRocketCount();
            photonView.RPC("ResetPossesing", RpcTarget.All);
            StartCoroutine(Explosion());
        }
        if (decreeseLevel != DecreeseLevel.fastest && possesingTime > decreeseUpTime[(int)decreeseLevel])
        { 
            DecreeseLevelUp(); 
        }
    }

    void Initialize()       // 初期化
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Debug.Log(gameManager);
        playerTransform = player.transform;
        startPos = playerTransform.position;
        SetEvacuatePos(200);
        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
        _camera = GameObject.Find("PlayerCamera");     // ゲームプレイで使う
        playerRB = player.GetComponent<Rigidbody>();
        cameraController = _camera.GetComponent<CameraController>();
        UpdateRocketCount(rocketCount);
    }

    float GetSecUntilZero(float limit, float minusValue, float runUnit)    //  ０になるまでの時間を計算(minusValueはrunUnitでの計算後の減少量)
    { 
        return limit / (minusValue * (1 / runUnit));
    }
    
    public void UpdateRocketCount(float newRocketCount)     // ロケットのカウントを全プレイヤーで同期
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "RocketCount", rocketCount } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    void CountElaps()    //  経過秒数カウント
    {
        rocketCount -= Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime;
        possesingTime += Time.deltaTime;
        UpdateRocketCount(rocketCount);
    }

    bool IsVibeTime()    //  カメラ振動時間か判定
    { 
        return vibeStartTime[(int)decreeseLevel] > rocketCount; 
    }

    bool isFloatingTime()    //  浮く時間か判定
    { 
        return floatStartTime > rocketCount;
    }

    bool IsVeryHigh()    //  凄い高いか判定
    { 
        return playerTransform.position.y > evacuateStarPos_Y; 
    }

    void SetGravity(Rigidbody rB, bool value)    //  RBのuseGravityをセット
    { 
        rB.useGravity = value;
    }

    void Floating(Transform floated, float floatForce)    //  オブジェクト浮遊
    { 
        floated.position += Vector3.up * floatForce * Time.deltaTime;
    }

    bool IsLimitOver()　　　　//  カウントがリミットを下回ったか判定
    { 
        return rocketLimit > rocketCount; 
    }

    IEnumerator Explosion()    //  ロケット爆発
    {
        Debug.Log("コルーチン開始");

        while (!IsVeryHigh())
        {
            Floating(playerTransform, explodeRiseSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(10.0f);
        DropOut();

        yield break;
    }

    void DropOut()      // 脱落処理
    {
        // マスタークライアントのみロケット付与処理を実行
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("コルーチン開始");
            StartCoroutine(gameManager.ChooseRocketPlayer());
        }
        // プレイヤーの死亡判定
        PhotonView targetPhotonView = player.GetComponent<PhotonView>();
        if (targetPhotonView != null)
        {
            targetPhotonView.RPC("SetPlayerDead", RpcTarget.All, true);
        }
    }

    void DecreeseLevelUp()    //  ロケットカウント加速
    {
        decreeseLevel += 1;
        //Debug.Log(decreeseLevel);
        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
        Debug.Log(secToExplode);
    }

    // ロケットのカウントをリセット
    void ResetRocketCount()
    {
        Debug.Log("カウントをリセット");
        rocketCount = initialCount; // デフォルト値
        UpdateRocketCount(rocketCount);
    }

    [PunRPC]
    public void ResetPossesing()    //  所持における数値の変動リセット
    {
        possesingTime = 0;
        decreeseLevel = 0;
    }

    Vector3 GetLineDir()
    { 
        return player.transform.position - this.transform.position;
    }

    // 上書きされたカウントを反映（コールバック）
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("RocketCount"))
        {
            rocketCount = (float)changedProps["RocketCount"];
        }
    }

    void SetEvacuatePos(float farFromStartPos)    //  開始位置から一定の距離にあるY座標を代入
    { 
        evacuateStarPos_Y = startPos.y + farFromStartPos;
    }

    //void ApproachPos(GameObject axis, GameObject Approcher, Vector3 offset)    //  オブジェクトの位置を近づける
    //{ Approcher.transform.position = axis.transform.position + offset; }
}
//  floatingとexpolderiseの処理が同時に行われているか検証