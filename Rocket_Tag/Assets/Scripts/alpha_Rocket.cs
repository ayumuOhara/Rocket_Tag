using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Alpha_Rocket : MonoBehaviourPunCallbacks
{
    // @何が起きてる？
    enum DecreaseLevel
    {
        slowest, veryslow, slow, normal, fast, veryfast, fastest
    }

    DecreaseLevel decreaseLevel = DecreaseLevel.slowest;
    float rocketLimit = 0;
    public float rocketCount = 100;
    public float initialCount = 100;
    float vibingPower = 0.2f;
    float vibingDuration = 2;
    float[] vibeStartTime = { 2, 3.2f, 6, 12, 18, 24, 35 };
    float floatStartTime = 2;
    float floatSpeed = 2;
    float explodeRiseSpeed = 18;
    float posessingTime = 0;
    float secToExplode = 0;
    float evacuateStarPos_Y = 40;
    float[] decreaseValue = { 0.4f, 1, 1.8f, 5, 12, 30, 100 };
    float[] decreaseUpTime = { 5, 10, 15, 20, 25, 30, 35 };
    bool isExploded = false;

    [SerializeField] GameObject player;
    GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        Initialize();
    }

    void Update()
    {
        CountDown();
        if (IsLimitOver())
        {
            ResetRocketCount();
            StartCoroutine(Explosion());
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
        Rigidbody _rb = player.GetComponent<Rigidbody>();
        _rb.useGravity = false;
        player.transform.position += Vector3.up * speed * Time.deltaTime;
    }

    bool IsVeryHigh()
    {
        return player.transform.position.y > evacuateStarPos_Y;
    }

    void DropOut()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(gameManager.ChooseRocketPlayer());
        }

        PhotonView photonView = player.GetComponent<PhotonView>();
        photonView.RPC("SetPlayerDead", RpcTarget.All, true);
    }

    void ResetRocketCount()
    {
        rocketCount = initialCount;
    }
}
