using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Alpha_Rocket : MonoBehaviourPunCallbacks
{
    float floatSpeed = 5f;
    float explodeRiseSpeed = 20f;
    float evacuateStarPos_Y = 40;
    int rocketStage = 0;

    Vector3 effectOffset = new Vector3(0, -1, 0);
    Vector3 smokeDiffusion = new Vector3(3, 0, 3);

    GameObject effect;
    Transform smoke;
    GameManager gameManager;
    TimeManager timeManager;
    RocketEffect rocketEffect;
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody playerRb;

    void Start()
    {
        //smoke = Resources.Load<GameObject>("FireSmoke");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        rocketEffect = GameObject.Find("RocketEffect").GetComponent<RocketEffect>();
        //rocketEffect.SetRocket(transform);
        playerRb = player.GetComponent<Rigidbody>();
     
        //effect.transform.localPosition = effectOffset;
    }

    void Update()
    {
        //if (rocketStage > 2 && timeManager.isSecondStageTime())
        //{
        //    effect = rocketEffectPrefab[(rocketstage += 1)];
        //}
        //if (rocketStage > 3 && timeManager.isSecondStageTime())
        //{
        //    effect = rocketEffectPrefab[(rocketstage += 1)];
        //smoke.transform.localScale = Vector3.Scale(smoke.transform.localScale, smokeDiffusion);
        //}
        //if (timeManager.IsFloatTime() && !timeManager.IsLimitOver())
        //{
        //    Floating(player, floatSpeed);
        //}
        if (timeManager.IsLimitOver())
        {
            StartCoroutine(Explosion());
        }
    }

    IEnumerator Explosion()
    {
        Debug.Log("ロケット爆発");
        while (!IsVeryHigh())
        {
            Floating(player, explodeRiseSpeed);
            yield return null;
        }
        DropOut();
    }

    void Floating(GameObject floated, float floatSpeed)
    {
        playerRb.useGravity = false;
        floated.transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    bool IsVeryHigh()
    {
        return transform.position.y > evacuateStarPos_Y;
    }

    void DropOut()
    {
        timeManager.ResetRocketCount();

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ロケットを抽選");
            gameManager.ChooseRocketPlayer();
        }

        PhotonView photonView = player.GetComponent<PhotonView>();
        photonView.RPC("SetPlayerDead", RpcTarget.All, true);
    }
}
