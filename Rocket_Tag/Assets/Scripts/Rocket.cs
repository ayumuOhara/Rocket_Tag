//using Photon.Pun;
//using Photon.Realtime;
//using System.Collections;
//using System.Runtime.CompilerServices;
//using Unity.VisualScripting;
//using UnityEngine;

//public class Rocket : MonoBehaviour
//{
//    IState currentState;
//    float riseSpeed { get; set; }

//    GameObject Player;
//    Rigidbody PlayerRB;

//    public TimeManager timeMgr;

//    void Start()
//    {
//        Initialize();
//    }
//    public void ChangeState(IState newState)
//    {
//        if (currentState != null)
//        {
//            currentState.Exit(this);
//        }
//        currentState = newState;
//        currentState.Enter(this);
//    }
//    void Update()
//    {
//        currentState.Update(this);
//    }
//    void Initialize()    //  初期化
//    {
//        Player = GameObject.Find("Player(Clone)");
//        Rigidbody PlayerRB = Player.GetComponent<Rigidbody>();
//        timeMgr = GameObject.Find("TimeManager").GetComponent<TimeManager>();

//        ChangeState(new NoActionState());
//    }
//    internal void Rising(float floatSpeed)    //  プレイヤー上昇
//    {
//        Player.transform.position += Vector3.up * floatSpeed * Time.deltaTime;
//    }
//    internal void SetPlayerCravity(bool arg)
//    {
//        PlayerRB.useGravity = arg;
//    }
//    IEnumerator Explosion()
//    {
        
//        Debug.Log("ロケット爆発");
//        while (!IsVeryHigh())
//        {
//            Rising()
//            yield return null;
//        }
//        DropOut();
//    }

//}
//public interface IState
//{
//    void Enter(Rocket arg);
//    void Update(Rocket arg);
//    void Exit(Rocket arg);
//}
//public class NoActionState : IState
//{
//    public void Enter(Rocket rocket)
//    {
//    }
//    public void Update(Rocket rocket)
//    {
//        if (rocket.timeMgr.IsFloatTime())
//        {
//            rocket.ChangeState(new FloatState());
//        }
//    }
//    public void Exit(Rocket rocket)
//    {
//    }
//}
//public class FloatState : IState
//{
//    float floatSpd = 5.7f;
//    public void Enter(Rocket rocket)
//    {
//        rocket.SetPlayerCravity(false);
//    }
//    public void Update(Rocket rocket)
//    {
//        if(rocket.timeMgr.IsLimitOver())
//        {
//            rocket.ChangeState(new ExplosionState());
//        }
//    }
//    public void Exit(Rocket rocket)
//    {
        
//    }
//}
//public class ExplosionState : IState
//{
//    float riseSpd = 18.8f;
//    public void Enter(Rocket rocket)
//    {
        
//    }
//    public void Update(Rocket rocket)
//    {
        
//    }
//    public void Exit(Rocket rocket)
//    {

//    }
//}
////public class Rocket : MonoBehaviourPunCallbacks
////{
////    float vibingPower;
////    float vibingDuration;
////    float explodeRiseSpeed;
////    float evacuatePos_Y;
////    bool isExplode;
////    bool isThrowed;
////    bool isReturning;
////    bool isHoldRocket;
////    bool isDropOut;

////    //[SerializeField] GameObject player;
////    GameObject player;
////    //[SerializeField] GameObject _camera;
////    [SerializeField] GameObject _camera;
////    GameObject rocket;
////    Transform playerTransform;
////    Rigidbody playerRB;
////    GameManager gameManager;
////    TimeManager timeManager;
////    CameraController cameraController;

////    void Start()
////    {
////        Initialize();
////    }
////    void Update()
////    {
////        {
////            if (IsVibeTime())
////            {
////                StartCoroutine(cameraController.Shake(vibingDuration, vibingPower));
////            }
////        }
////        if (isFloatingTime() && !IsStopePos())
////        {
////            SetGravity(playerRB, false);
////            Floating(playerTransform, floatSpeed);
////        }
////        if (IsLimitOver())
////        {
////            StartCoroutine(Explosion());
////            ResetRocketCount();
////            ResetPossesing();
////        }
////        if (IsDecreeseUpTime())
////        {
////            DecreeseLevelUp();
////        }
////        //if (Mathf.Abs(transform.position.x - playerTransform.position.x) < 2 && isReturning)
////        //{
////        //    // 運動エネルギー停止
////        //    ApproachPos(player, rocket, rocketOffset);
////        //    isReturning = false;
////        //    isHoldRocket = true;
////        //    isThrowed = false;
////        //    throwedTime = 0;
////        //}
////        if (Input.GetKeyDown(KeyCode.R))
////        {
////            this.transform.position = STARTPOS;
////        }
////    }
////    void Initialize()    //  初期化
////    {
////        vibingPower = 0.2f;
////        vibingDuration = 0.2f;
////        explodeRiseSpeed = 18;
////        evacuatePos_Y = 40;
////        isExplode = false;
////        isThrowed = false;
////        isReturning = false;
////        isHoldRocket = true;
////        isDropOut = false;

////        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
////        player = GameObject.Find("Player");
////        playerTransform = player.transform;
////        evacuatePos_Y = GetPos_YFromStart(40);
////        _camera = GameObject.Find("PlayerCamera");     // ゲームプレイで使う
////        rocket = GameObject.Find("Rocket");
////        cameraController = _camera.GetComponent<CameraController>();
////    }
////    // ロケットのカウントを全プレイヤーで同期
////    //void Explosion()    //  爆弾爆発
////    //{
////    //    // isExplode = true;
////    //    if (!IsVeryHigh())
////    //    {
////    //        Floating(playerTransform, explodeRiseSpeed);
////    //        ResetRocketCount();
////    //        ResetPossesing();
////    //        if (!isDropOut)
////    //        {
////    //            // マスタークライアントのみロケット付与処理を実行
////    //            if (PhotonNetwork.IsMasterClient)
////    //            {
////    //                gameManager.ChooseRocketPlayer();
////    //            }
////    //            // プレイヤーの死亡判定
////    //            PhotonView targetPhotonView = player.GetComponent<PhotonView>();
////    //            if (targetPhotonView != null)
////    //            {
////    //                targetPhotonView.RPC("SetPlayerDead", RpcTarget.All, true);
////    //                targetPhotonView.RPC("SetHasRocket", RpcTarget.All, false);
////    //            }
////    //            isDropOut = true;
////    //        }
////    //    }

////    //}
////    IEnumerator Explosion()    //  ロケット爆発
////    {
////        DropOut();

////        while (!IsStopePos())
////        {
////            Floating(playerTransform, explodeRiseSpeed);
////            yield return null;
////        }
////        yield break;
////    }
////    void DropOut()
////    {
////        if (PhotonNetwork.IsMasterClient)
////        {
////            // マスタークライアントのみロケット付与処理を実行
////            if (PhotonNetwork.IsMasterClient)
////            {
////                Debug.Log("コルーチン開始");
////                //StartCoroutine(gameManager.ChooseRocketPlayer());
////            }
////            // プレイヤーの死亡判定
////            PhotonView targetPhotonView = player.GetComponent<PhotonView>();
////            if (targetPhotonView != null)
////            {
////                targetPhotonView.RPC("SetPlayerDead", RpcTarget.All, true);
////            }
////            gameManager.ChooseRocketPlayer();
////        }

////        PhotonView photonView = player.GetComponent<PhotonView>();
////        photonView.RPC("SetPlayerDead", RpcTarget.All, true);
////    }

////    // ロケットのカウントをリセット
////    public void ResetRocketCount()
////    {
////        rocketCount = initialCount; // デフォルト値
////        UpdateRocketCount(rocketCount);
////    }
////    void DecreeseLevelUp()    //  ロケットカウント加速
////    {
////        decreeseLevel += 1;
////        Debug.Log(decreeseLevel);
////        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
////        Debug.Log(secToExplode);
////    }
////    public void ResetPossesing()    //  所持における数値の変動リセット
////    {
////        possesingTime = 0;
////        decreeseLevel = 0;
////    }



////    void Floating(Transform floated, float floatForce)    //  オブジェクト浮遊
////    {
////        floated.position += Vector3.up * floatForce * Time.deltaTime;
////    }



////    private void OnCollisionEnter(Collision collision)
////    {
////        string collidedObjectTag = collision.gameObject.tag;
////        Debug.Log(collidedObjectTag);
////        if (collidedObjectTag != "Player" && collidedObjectTag != "Ground")
////        {
////            isReturning = true;
////        }
////        if (collidedObjectTag == "Player")
////        {
////            //    プレイヤーに当たった処理
////        }
////    }


////    //void BomCouuntDecreese(int value)    //  ロケットカウントを減らす;
////    //{
////    //    bombCount -= value * Time.deltaTime;
////    //}


////    // 上書きされたカウントを反映（コールバック）
////    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
////    {
////        if (!ForTest)
////        {
////            if (changedProps.ContainsKey("RocketCount"))
////            {
////                rocketCount = (float)changedProps["RocketCount"];
////                Debug.Log("RocketCount updated: " + rocketCount);
////            }
////        }
////    }
////    bool IsVibeTime()    //  カメラ振動時間か判定
////    {
////        return vibeStartTime[(int)decreeseLevel] > rocketCount;
////    }
////    bool isFloatingTime()    //  浮く時間か判定
////    {
////        return floatStartTime > rocketCount;
////    }
////    float GetPos_YFromStart(float farFromStartPos)    //  開始位置から一定の距離にあるY座標を代入
////    {
////        return playerTransform.position.y + farFromStartPos;
////    }


////    bool IsDecreeseUpTime()
////    {
////        return decreeseLevel != DecreeseLevel.fastest && possesingTime > decreeseUpTime[(int)decreeseLevel];
////    }
////    bool IsLimitOver()　　　　//  カウントがリミットを下回ったか判定
////    {
////        return rocketLimit > rocketCount;
////    }
////    bool IsStopePos()    //  動作停止位置か判定
////    {
////        return playerTransform.position.y > evacuatePos_Y;
////    }
////    void SetGravity(Rigidbody rB, bool value)    //  RBのuseGravityをセット
////    {
////        rB.useGravity = value;
////    }
////}

//////修正必