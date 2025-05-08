using Photon.Pun;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPlayerBool : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject resultUI;

    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] RocketEffect rocketEffect;
    public TimeManager timeManager;
    //public ResultScreen resultScreen;
    public PlayerRankManager playerRankManager;
    public SEManager seManager;

    [SerializeField] GameObject rocketObj;  // ロケット
    [SerializeField] GameObject compas;

    [SerializeField] public bool hasRocket; // ロケットを所持しているか
    [SerializeField] public bool isDead;    // 死亡判定
    [SerializeField] public bool isStun;    // スタン判定

    private void Awake()
    {
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        //resultScreen = GameObject.Find("GameManager").GetComponent<ResultScreen>();
        playerRankManager = GameObject.Find("GameManager").GetComponent<PlayerRankManager>();
        seManager = GameObject.Find("SE_Audio").GetComponent<SEManager>();
        resultUI = GameObject.Find("ResultUI");
        if(SceneManager.GetActiveScene().name == "PlayScene")
        {
            rocketEffect = GameObject.Find("RocketEffect").GetComponent<RocketEffect>();
        }
    }

    private void Start()
    {       
        if (resultUI != null)
            resultUI.SetActive(false);
    }

    private void Update()
    {
        if (hasRocket)
        {
            seManager.PlayFuseSE();
        }
        if(!hasRocket)
        {
            seManager.StopFuseSE();
        }
    }

    // プレイヤーの状態の初期化
    public void SetPlayerCondition()
    {
        photonView.RPC("SetHasRocket", RpcTarget.All, false);
        photonView.RPC("SetPlayerDead", RpcTarget.All, false);
        photonView.RPC("SetIsStun", RpcTarget.All, false);
    }

    // 死亡処理
    [PunRPC]
    public void SetPlayerDead(bool newIsDead)
    {
        isDead = newIsDead;

        if (isDead && photonView.IsMine)
        {
            if (resultUI != null)
            {
                resultUI.SetActive(true);
            }
            else
            {
                Debug.Log("ResultUIが取得できていません");
            }

            if (playerRankManager != null)
            {
                playerRankManager.SetPlayerRank();
            }
        }
    }

    [PunRPC]
    public void SetIsStun(bool newIsStun)
    {
        isStun = newIsStun;
        if(isStun)
        {
            StartCoroutine(playerMovement.StunPlayer());
        }
    }

    // hasRocket を設定し、同期
    [PunRPC]
    public void SetHasRocket(bool newHasRocket)
    {
        hasRocket = newHasRocket;
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        if (hasRocket)
        {
            rocketObj.SetActive(true);
            compas.SetActive(true);
            Debug.Log("ロケットを受け取ります");
            rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.SEARCH_ROCKET);
            rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_FRAMES);
        }
        else
        {
            rocketObj.SetActive(false);
            compas.SetActive(false);
        }

        if (timeManager != null)
        {
            timeManager.ResetAcceleration();
        }
        else
        {
            Debug.Log("timeManagerがnullです");
        }
        //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.SEARCH_ROCKET);

    }
}
