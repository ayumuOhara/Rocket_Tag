using Photon.Pun;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class SetPlayerBool : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject resultUI;

    [SerializeField] PlayerMovement playerMovement;
    //RocketEffect rocketEffect;
    public TimeManager timeManager;
    public ResultScreen resultScreen;
    public PlayerRankManager playerRankManager;

    [SerializeField] GameObject rocketObj;  // ロケット

    [SerializeField] public bool hasRocket; // ロケットを所持しているか
    [SerializeField] public bool isDead;    // 死亡判定
    [SerializeField] public bool isStun;    // スタン判定

    private void Start()
    {
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        resultScreen = GameObject.Find("GameManager").GetComponent<ResultScreen>();
        playerRankManager = GameObject.Find("GameManager").GetComponent<PlayerRankManager>();
        resultUI = GameObject.Find("ResultUI");
        if (resultUI != null)
            resultUI.SetActive(false);
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
        Debug.Log("死亡判定：" + newIsDead);

        isDead = newIsDead;
        resultUI.SetActive(true);

        if (playerRankManager != null)
        {
            playerRankManager.SetPlayerRank();
        }

        if (resultScreen != null)
        {
            resultScreen.ShowMyResult();
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
            Debug.Log("ロケットを受け取ります");
            rocketObj.SetActive(true);
        }
        else
        {
            rocketObj.SetActive(false);
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
