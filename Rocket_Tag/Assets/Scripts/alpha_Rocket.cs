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
    float floatStartTime = 2;
    float floatSpeed = 2f;
    float explodeRiseSpeed = 18f;
    float possesingTime = 0;
    float secToExplode = 0;
    float evacuateStarPos_Y = 40;
    float[] decreeseValue = { 0.4f, 1, 1.8f, 5f, 12f, 30f, 100f };
    float[] decreeseUpTime = { 5f, 10f, 15f, 20f, 25f, 30f, 35f };
    bool isExplode = false;
    bool isVeryHigh = false;
    bool isDropOut = false;

    [SerializeField] GameObject player;
    [SerializeField] GameObject camera;
    Rigidbody playerRB;
    Renderer playerRenderer;
    Material playerMaterial;
    CameraController cameraController;
    Transform playerTransform;
    void Start()
    {
        playerTransform = player.transform;
        camera = GameObject.Find("PlayerCamera");     // ゲームプレイで使う
        playerRB = player.GetComponent<Rigidbody>();
        playerRenderer = player.GetComponent<Renderer>();
        playerMaterial = playerRenderer.material;
        cameraController = camera.GetComponent<CameraController>();
        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
        UpdateRocketCount(rocketCount);
    }
    void Update()
    {
        CountElaps();
        if (IsLimitOver() || isExplode) 
        { Explosion(); }
        if (isFloatingTime() && !IsVeryHigh())
        {
            playerRB.useGravity = false;
            StartCoroutine(cameraController.Shake(2f, 0.2f));
            Floating(playerTransform, floatSpeed);
        }
        if (decreeseLevel != DecreeseLevel.fastest && possesingTime > decreeseUpTime[(int)decreeseLevel]) { DecreeseLevelUp(); }
    }
    // ロケットのカウントを全プレイヤーで同期
    float GetSecUntilZero(float limit, float minusValue, float runUnit)    //  ０になるまでの時間を計算(minusValueはrunUnitでの計算後の減少量)
    {
        return limit / (minusValue * (1 / runUnit));
    }
    public void UpdateRocketCount(float newRocketCount)
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
    bool IsLimitOver()
    { return rocketLimit > rocketCount; }
    void Explosion()    //  爆弾爆発
    {
        if (!IsVeryHigh())
        {
            Floating(playerTransform, explodeRiseSpeed);
            ResetRocketCount();
            ResetPossesing();
            if (!isDropOut)
            {
                // マスタークライアントのみ処理を実行
                if (PhotonNetwork.IsMasterClient)
                {
                    GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                    gameManager.ChooseRocketPlayer();
                }
                // プレイヤーの死亡判定
                PhotonView targetPhotonView = player.GetComponent<PhotonView>();
                if (targetPhotonView != null)
                {
                    targetPhotonView.RPC("SetPlayerDead", RpcTarget.All, true);
                    targetPhotonView.RPC("SetHasRocket", RpcTarget.All, false);
                }
                isDropOut = true;
            }
        }
    }
    bool isFloatingTime()
    { return floatStartTime > rocketCount; }
    void Floating(Transform floated, float floatForce)    //  オブジェクト浮遊
    { floated.position += Vector3.up * floatForce * Time.deltaTime; }
    // ロケットのカウントをリセット
    void ResetRocketCount()
    {
        rocketCount = initialCount; // デフォルト値
        UpdateRocketCount(rocketCount);
    }

    void DecreeseLevelUp()    //  ロケットカウント加速
    {
        decreeseLevel += 1;
        Debug.Log(decreeseLevel);
        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
        Debug.Log(secToExplode);
    }
    bool IsVeryHigh()
    {
        return playerTransform.position.y > evacuateStarPos_Y;
    }
    public void ResetPossesing()    //  所持における数値の変動リセット
    {
        possesingTime = 0;
        decreeseLevel = 0;
    }
    Vector3 GetLineDir()
    { return player.transform.position - this.transform.position; }
    // 上書きされたカウントを反映（コールバック）
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("RocketCount"))
        {
            rocketCount = (float)changedProps["RocketCount"];
        }
    }
    //void ApproachPos(GameObject axis, GameObject Approcher, Vector3 offset)    //  オブジェクトの位置を近づける
    //{
    //    Approcher.transform.position = axis.transform.position + offset;
    //}
}