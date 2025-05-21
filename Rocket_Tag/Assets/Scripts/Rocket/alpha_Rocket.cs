using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Alpha_Rocket : MonoBehaviourPunCallbacks
{
    float explodeRiseSpeed = 20f;
    float riseSpdAcceleration = 1.002f;
    bool isExploding = false;

    GameManager gameManager;
    TimeManager timeManager;
    UILogManager uiLogManager;
    [SerializeField] RocketEffect rocketEffect;
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody playerRb;

    void OnEnable()
    {

        //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.SEARCH_ROCKET);
        //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_FRAMES);
    }

    void Start()
    {
        rocketEffect = GameObject.Find("RocketEffect").GetComponent<RocketEffect>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        uiLogManager = GameObject.Find("UILogManager").GetComponent<UILogManager>();

        //Debug.Log($"gameManager：{gameManager}");
        //Debug.Log($"timeManager：{timeManager}");
        //Debug.Log($"uiLogManager：{uiLogManager}");

        StartCoroutine(CheckOverTime());
    }

    void Update()
    {
        if (timeManager.IsLimitOver() && !isExploding)
        {
            isExploding = true;
            Debug.Log("タイマーが０になりました");
            StartCoroutine(Explosion());
        }
    }
    IEnumerator Explosion()
    {
        //Debug.Log("ロケット爆発");
        float time = 0;

        while (time < 3.0f)
        {
            time += Time.deltaTime;
            Floating(player, (explodeRiseSpeed *= riseSpdAcceleration));
            yield return null;
        }
        DropOut();
        yield break;
    }

    void Floating(GameObject floated, float floatSpeed)
    {
        //Debug.Log($"floated確認：{floated}");

        playerRb.useGravity = false;
        Collider collider = player.GetComponent<CapsuleCollider>();
        collider.isTrigger = true;
        player.transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    void DropOut()
    {
        Debug.Log("脱落処理開始");

        PhotonView playerPhoton = this.player.GetComponent<PhotonView>();
        PhotonView timePhoton = GameObject.Find("TimeManager").GetComponent<PhotonView>();

        if (playerPhoton.IsMine)
        {
            Debug.Log("死亡処理開始");

            timePhoton.RPC("IsTimeStart", RpcTarget.All, false);
            timeManager.ResetRocketCount();

            uiLogManager.AddLog("player", UILogManager.LogType.Dead);
            playerPhoton.RPC("SetPlayerDead", RpcTarget.All, true);

            gameManager.ChooseRocketPlayer();
            timePhoton.RPC("IsTimeStart", RpcTarget.All, true);
        }
        rocketEffect._IsDestoroyRocket = true;
        this.gameObject.SetActive(false);
    }

    IEnumerator CheckOverTime()
    {
        SetPlayerBool spb = player.GetComponent<SetPlayerBool>();

        while (true)
        {
            if (timeManager.rocketTime < -20.0f)
            {
                if (spb.hasRocket && spb.isDead!)
                {
                    PhotonView photon = player.GetComponent<PhotonView>();
                    photon.RPC("SetPlayerDead", RpcTarget.All, true);
                }

                this.gameObject.SetActive(false);
            }            
            yield return null;
        }
    }
}
