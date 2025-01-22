using Photon.Pun;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;
using System;

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
    bool isShaking = false; // カメラ振動防止用フラグ

    Vector3 startPos;

    [SerializeField] GameObject player;
    [SerializeField] GameObject camera;
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

        if (IsVibeTime() && !isShaking)
        {
            isShaking = true;
            StartCoroutine(Vibration());
        }

        if (isFloatingTime() && !IsVeryHigh() && !isExplode)
        {
            SetGravity(playerRB, false);
            Floating(playerTransform, floatSpeed);
        }

        if (IsLimitOver() && !isExplode)
        {
            isExplode = true; // 爆発中フラグを設定
            StartCoroutine(Explosion());
            ResetRocketCount();
            ResetPossesing();
        }

        if (decreeseLevel != DecreeseLevel.fastest && possesingTime > decreeseUpTime[(int)decreeseLevel])
        {
            DecreeseLevelUp();
        }
    }

    void Initialize()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerTransform = player.transform;
        startPos = playerTransform.position;
        SetEvacuatePos(40);
        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
        camera = GameObject.Find("PlayerCamera");
        playerRB = player.GetComponent<Rigidbody>();
        cameraController = camera.GetComponent<CameraController>();
        UpdateRocketCount(rocketCount);
    }

    float GetSecUntilZero(float limit, float minusValue, float runUnit)
    {
        return limit / (minusValue * (1 / runUnit));
    }

    public void UpdateRocketCount(float newRocketCount)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "RocketCount", rocketCount } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    void CountElaps()
    {
        rocketCount -= Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime;
        possesingTime += Time.deltaTime;
        UpdateRocketCount(rocketCount);
    }

    bool IsVibeTime()
    {
        return vibeStartTime[(int)decreeseLevel] > rocketCount;
    }

    bool isFloatingTime()
    {
        return floatStartTime > rocketCount;
    }

    bool IsVeryHigh()
    {
        return playerTransform.position.y > evacuateStarPos_Y;
    }

    void SetGravity(Rigidbody rB, bool value)
    {
        rB.useGravity = value;
    }

    void Floating(Transform floated, float floatForce)
    {
        floated.position += Vector3.up * floatForce * Time.deltaTime;
    }

    bool IsLimitOver()
    {
        return rocketLimit > rocketCount;
    }

    IEnumerator Explosion()
    {
        DropOut();

        while (!IsVeryHigh())
        {
            Floating(playerTransform, explodeRiseSpeed);
            yield return null;
        }

        isExplode = false; // 爆発後にフラグ解除
    }

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

    IEnumerator Vibration()
    {
        yield return cameraController.Shake(vibingDuration, vibingPower);
        isShaking = false; // 振動終了後にフラグ解除
    }

    void DecreeseLevelUp()
    {
        decreeseLevel += 1;
        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
    }

    void ResetRocketCount()
    {
        rocketCount = initialCount;
        UpdateRocketCount(rocketCount);
    }

    public void ResetPossesing()
    {
        possesingTime = 0;
        decreeseLevel = 0;
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("RocketCount"))
        {
            rocketCount = Convert.ToSingle(changedProps["RocketCount"]);
        }
    }

    void SetEvacuatePos(float farFromStartPos)
    {
        evacuateStarPos_Y = startPos.y + farFromStartPos;
    }
}
