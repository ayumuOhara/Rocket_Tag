using Photon.Pun;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class alpha_Rocket : MonoBehaviourPunCallbacks
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
    public float rocketCount = 1000;
    float vibeTime;
    float vibeStartTime = 0.5f;
    float riseSpeed = 10;
    float floatingTime = 2;
    float floatSpeed = 1f;
    float possesingTime = 0;
    float secToExplode = 0;
    float playerPosX;
    float[] decreeseValue = { 0.4f, 1, 1.8f, 5f, 12f, 30f, 100f };
    float[] decreeseUpTime = { 5f, 10f, 15f, 20f, 25f, 30f, 35f };
    float throwedTime = 0;
    bool isExplode = false;
    bool isReturning = false;
    bool isHoldRocket = true;
    bool isNeedHold = true;
    bool isThrowed = false;
    Vector3 playerOffset = new Vector3(0, 5, 5);
    Vector3 rocketOffset = new Vector3(1, 0, 0);
    Vector3 thorowRocketOffset = new Vector3(0, 3f, 0);
    Vector3 explodeInpact;

    Rigidbody rocketRB;
    [SerializeField] GameObject player;
    [SerializeField] GameObject camera;
    [SerializeField] GameObject rocket;
    Transform playerTransform;
    Transform cameraTransform;

    bool ForTest = false;
    Vector3 startpos;
    void Start()
    {
        //if (ForTest)    //  TestScene用
        //{
        //    player = GameObject.Find("Player");
        //    camera = GameObject.Find("PlayerCamera");
        //    rocket = GameObject.Find("Bomb");
        //}
        riseSpeed = 10;
        vibeTime = 3;
        rocketRB = this.GetComponent<Rigidbody>();
        camera = GameObject.Find("PlayerCamera");     // ゲームプレイで使う
        playerTransform = player.transform;
        cameraTransform = camera.transform;
        explodeInpact = new Vector3(0.2f, cameraTransform.position.y, cameraTransform.position.z);
        startpos = this.transform.position;

        UpdateRocketCount(rocketCount);
        rocketRB.useGravity = false;
        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
    }

    void Update()
    {
        CountElaps();
        //if (IsVibeTime())
        //{ CameraVibe(explodeInpact, vibeTime); }
        if (rocketCount <= rocketLimit)
        {
            Explosion(); 
        }
        if (isExplode)
        {
            ApproachPos(rocket, player, playerOffset);
        }
        DecreeseLevelUp();
        if (Input.GetKeyDown(KeyCode.E) && isHoldRocket)
        {
            rocketRB.useGravity = true;
        }
        if (Mathf.Abs(rocketRB.position.x - playerPosX) < 2 && isReturning)
        {
            isNeedHold = true;
            // 運動エネルギー停止
            rocketRB.linearVelocity = Vector3.zero;
            ApproachPos(player, rocket, rocketOffset);
            isReturning = false;
            isHoldRocket = true;
            rocketRB.useGravity = false;
            isThrowed = false;
            throwedTime = 0;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.transform.position = startpos;
            rocketRB.linearVelocity = new Vector3(0, 0, 0);
        }
        if (isNeedHold)
        {
            RocketFix(player, rocket, rocketOffset);
        }
    }
    // ロケットのカウントを全プレイヤーで同期
    public void UpdateRocketCount(float newRocketCount)
    {
        if (!ForTest)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "RocketCount", rocketCount } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
    float GetSecUntilZero(float limit, float minusValue, float runUnit)    //  ０になるまでの時間を計算(minusValueはrunUnitでの計算後の減少量)
    {
        return limit / (minusValue * (1 / runUnit));
    }
    void CountElaps()    //  経過秒数カウント
    {
        rocketCount -= Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime;
        possesingTime += Time.deltaTime;
        UpdateRocketCount(rocketCount);
        if (isThrowed)
        {
            throwedTime += Time.deltaTime;
            if (throwedTime > 1.5f)
            {
                ApproachPos(player, rocket, rocketOffset);
                isHoldRocket = true;
                isNeedHold = true;
                isReturning = false;
                isThrowed = false;
            }
        }
    }

    void Explosion()    //  爆弾爆発
    {
        isNeedHold = false;
        {
            if ((floatingTime -= Time.deltaTime) > 0)
            {
                Floating(transform, floatSpeed);
            }
            else
            {
                rocketRB.linearVelocity = new Vector3(0, riseSpeed, 0);
                isExplode = true;
                ResetRocketCount();
                ResetPossesing();
            }

            // プレイヤーの死亡判定
            PhotonView targetPhotonView = player.GetComponent<PhotonView>();
            if (targetPhotonView != null)
            {
                targetPhotonView.RPC("SetPlayerDead", RpcTarget.All, true);
            }

            GameManager gameManager = player.GetComponent<GameManager>();
            gameManager.ChooseRocketPlayer();
        }
    }

    // ロケットのカウントをリセット
    public void ResetRocketCount()
    {
        rocketCount = 1000; // デフォルト値
        UpdateRocketCount(rocketCount);
    }

    //public void CameraVibe(Vector3 vibeInpact, float duration)    //  カメラ振動
    //{
    //    cameraTransform.transform.position += explodeInpact;
    //    explodeInpact.x *= -1;
    //    vibeTime -= Time.deltaTime;
    //}

    void DecreeseLevelUp()    //  ロケットカウント加速
    {
        if (decreeseLevel != DecreeseLevel.fastest && possesingTime > decreeseUpTime[(int)decreeseLevel])
        {
            decreeseLevel += 1;
            Debug.Log(decreeseLevel);
            secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
            Debug.Log(secToExplode);
        }
    }
    public void ResetPossesing()    //  所持における数値の変動リセット
    {
        possesingTime = 0;
        decreeseLevel = 0;
    }

    void ApproachPos(GameObject axis, GameObject Approcher, Vector3 offset)    //  オブジェクトの位置を近づける
    {
        Approcher.transform.position = axis.transform.position + offset;
    }

    void RocketFix(GameObject axis, GameObject Approcher, Vector3 offset)    //  オブジェクトの位置を近づける
    {
        float distance = offset.magnitude;
        // カメラの位置(transform.position)の更新
        transform.position = playerTransform.position + new Vector3(0, 0.3f, 0) - transform.rotation * Vector3.forward * distance;
    }

    void Floating(Transform floated, float floatForce)    //  オブジェクト浮遊
    {
        floated.position += Vector3.up * floatForce * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        string collidedObjectTag = collision.gameObject.tag;
        Debug.Log(collidedObjectTag);
        if (collidedObjectTag != "Player" && collidedObjectTag != "Ground")
        {
            isReturning = true;
        }
    }

    Vector3 GetLineDir()
    {
        Vector3 dir = player.transform.position - this.transform.position;
        return dir;
    }

    // 上書きされたカウントを反映（コールバック）
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!ForTest)
        {
            if (changedProps.ContainsKey("RocketCount"))
            {
                rocketCount = (float)changedProps["RocketCount"];
            }
        }
    }

    //public bool IsVibeTime()
    //{
    //    return ((secToExplode -= Time.deltaTime) < vibeStartTime || isExplode) && vibeTime > 0;
    //}
}

//修正必