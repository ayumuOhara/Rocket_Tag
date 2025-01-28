using Photon.Pun;
using Photon.Realtime;
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
        fastest,
    }

    DecreaseLevel decreaseLevel = DecreaseLevel.slowest;

    float rocketLimit = 0;
    public float rocketCount = 100;
    public float initialCount = 100;
    float vibingPower = 0.2f;
    float vibingDuration = 2;
    float[] vibeStartTime = { 2, 3.2f, 6, 12, 18, 24, 35 };
    float floatStartTime = 2;
    float floatSpeed = 20;
    float explodeRiseSpeed = 18;
    float posessingTime = 0;
    float secToExplode = 0;
    float evacuateStarPos_Y = 40;
    float[] decreaseValue = { 0.4f, 1, 1.8f, 5, 12, 30, 100 };
    float[] decreaseUpTime = { 5, 10, 15, 20, 25, 30, 35 };
    bool isExploded = false;
    public bool isTimeStop = false;

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
            if (isTimeStop == false)
            {
                CountDown();
            }

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
        if (PhotonNetwork.IsMasterClient)
        {
            SyncRocketCount(initialCount); // マスタークライアントが初期値を設定
        }
        else
        {
            rocketCount = GetSyncedRocketCount(); // 他プレイヤーは同期値を取得
        }

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
        if (PhotonNetwork.IsMasterClient)
        {
            rocketCount -= Time.deltaTime + decreaseValue[(int)decreaseLevel] * Time.deltaTime;
            SyncRocketCount(rocketCount); // マスタークライアントがタイマーを更新
        }
        else
        {
            rocketCount = GetSyncedRocketCount(); // 他プレイヤーは同期値を取得
        }

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
        SyncRocketCount(initialCount); // ロケットカウントの同期
    }

    void CheckForLevelUp()
    {
        if (decreaseLevel < DecreaseLevel.fastest && posessingTime > decreaseUpTime[(int)decreaseLevel])
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        decreaseLevel++;
        Debug.Log($"タイマーの減少速度がアップしました: {decreaseLevel}");
    }

    void ResetAcceleration()
    {
        Debug.Log("加速度をリセットします");
        decreaseLevel = DecreaseLevel.slowest; // 初期状態に戻す
        posessingTime = 0;                     // 経過時間をリセット
        ResetRocketCount();                    // ロケットカウントもリセット
    }

    /// <summary>
    /// ロケットカウントを全プレイヤーで同期
    /// </summary>
    void SyncRocketCount(float count)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "RocketCount", count }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    /// <summary>
    /// 同期されたロケットカウントを取得
    /// </summary>
    float GetSyncedRocketCount()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RocketCount", out object value))
        {
            return (float)value;
        }
        return initialCount; // デフォルト値を返す
    }
}
