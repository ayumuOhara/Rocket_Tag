using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Alpha_Rocket : MonoBehaviourPunCallbacks
{
    enum DecreaseLevel
    {
        slowest,
        veryslow,
        slow,
        normal,
        fast,
        veryfast,
        fastest
    }

    DecreaseLevel decreaseLevel = DecreaseLevel.slowest;
    float rocketLimit = 0;
    public float rocketCount = 100;
    public float initialCount = 100;
    float vibingPower = 0.2f;
    float vibingDuration = 2;
    float[] vibeStartTime = { 2, 3.2f, 6, 12, 18, 24, 35 };
    float floatStartTime = 2;
    float floatSpeed = 10;
    float explodeRiseSpeed = 18;
    float posessingTime = 0;
    float secToExplode = 0;
    float evacuateStarPos_Y = 40;
    float[] decreaseValue = { 0.4f, 1, 1.8f, 5, 12, 30, 100 };
    float[] decreaseUpTime = { 5, 10, 15, 20, 25, 30, 35 };
    bool isExploded = false;

    [SerializeField] SetPlayerBool setPlayerBool;
    [SerializeField] GameObject player;
    Rigidbody playerRb;
    GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerRb = player.GetComponent<Rigidbody>();
        Initialize();
    }

    void Update()
    {
        if (setPlayerBool.isDead == false)
        {
            CountDown();

            if (IsLimitOver())
            {
                ResetRocketCount();
                StartCoroutine(Explosion());
            }

            CheckForLevelUp();
        }
    }

    void Initialize()
    {
        secToExplode = GetSecUntilZero(rocketCount, decreaseValue[(int)decreaseLevel], Time.deltaTime);
    }

    float GetSecUntilZero(float limit, float minusValuePerSecond, float timeStep)
    {
        if (minusValuePerSecond <= 0)
        {
            Debug.LogWarning("減少量が0以下です。計算できません。");
            return float.MaxValue; // 無限大を返す
        }

        return limit / (minusValuePerSecond * (1 / timeStep));
    }

    void CountDown()
    {
        rocketCount -= Time.deltaTime + decreaseValue[(int)decreaseLevel] * Time.deltaTime;
        posessingTime += Time.deltaTime;
    }

    bool IsLimitOver()
    {
        return rocketLimit > rocketCount;
    }

    IEnumerator Explosion()
    {
        ResetAcceleration();

        Debug.Log("ロケット爆発");
        while (!IsVeryHigh())
        {
            Floating(floatSpeed);
            yield return null;
        }

        DropOut();
    }

    void Floating(float speed)
    {
        playerRb.useGravity = false;
        player.transform.position += Vector3.up * speed * Time.deltaTime;
    }

    bool IsVeryHigh()
    { 
        return transform.position.y > evacuateStarPos_Y;
    }

    void DropOut()
    {
        PhotonView photonView = player.GetComponent<PhotonView>();
        photonView.RPC("SetPlayerDead", RpcTarget.All, true);
        Debug.Log("死亡");

        if (PhotonNetwork.IsMasterClient)
        {
            gameManager.ChooseRocketPlayer();
        }        
    }

    void ResetRocketCount()
    {
        rocketCount = initialCount;
    }

    // 一定時間ごとに減少速度を上げる
    void CheckForLevelUp()
    {
        if (decreaseLevel < DecreaseLevel.fastest && posessingTime > decreaseUpTime[(int)decreaseLevel])
        {
            LevelUp();
        }
    }

    // 減少速度を次のレベルにアップ
    void LevelUp()
    {
        decreaseLevel++;
        Debug.Log($"タイマーの減少速度がアップしました: {decreaseLevel}");
    }

    // 加速度をリセットし、関連カウントを初期化
    void ResetAcceleration()
    {
        Debug.Log("加速度をリセットします");
        decreaseLevel = DecreaseLevel.slowest; // 初期状態に戻す
        posessingTime = 0;                     // 経過時間をリセット
        ResetRocketCount();                    // ロケットカウントもリセット
    }
}
