using Photon.Pun;
using TMPro;
using UnityEngine;

public class SetPlayerBool : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] RocketEffect rocketEffect;
    public TimeManager timeManager;
    public ResultScreen resultScreen;
    public PlayerRankManager playerRankManager;
    

    [SerializeField] GameObject rocketObj;  // ロケット

    [SerializeField] public bool hasRocket; // ロケットを所持しているか
    [SerializeField] public bool isDead;    // 死亡判定
    [SerializeField] public bool isStun;    // スタン判定

    private void Start()
    {
        timeManager       = GameObject.Find("TimeManager" ).GetComponent<TimeManager>();
        resultScreen      = GameObject.Find("Result"      ).GetComponent<ResultScreen>();
        playerRankManager = GameObject.Find("GameManager" ).GetComponent<PlayerRankManager>();
    }

    // プレイヤーの状態の初期化
    public void SetPlayerCondition()
    {
        // ロケットの状態を初期化
        photonView.RPC("SetHasRocket", RpcTarget.All, false);

        photonView.RPC("SetPlayerDead", RpcTarget.All, false);
        isStun = false;
    }

    // 死亡処理
    [PunRPC]
    public void SetPlayerDead(bool newIsDead)
    {
        isDead = newIsDead;

        playerRankManager.SetPlayerRank();
        resultScreen.ShowMyResult();

        this.gameObject.SetActive(false);
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

        rocketObj.SetActive(hasRocket);
        timeManager.ResetAcceleration();
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.SEARCH_ROCKET);
    }
}
