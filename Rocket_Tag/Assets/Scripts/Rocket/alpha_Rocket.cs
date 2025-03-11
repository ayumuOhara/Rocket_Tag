using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Alpha_Rocket : MonoBehaviourPunCallbacks
{
    float explodeRiseSpeed = 20f;
    bool isExploding = false;

    GameManager gameManager;
    TimeManager timeManager;
    UILogManager uiLogManager;
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody playerRb;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        uiLogManager = GameObject.Find("UILogManager").GetComponent<UILogManager>();

        Debug.Log($"gameManager：{gameManager}");
        Debug.Log($"timeManager：{timeManager}");
        Debug.Log($"uiLogManager：{uiLogManager}");
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
        Debug.Log("ロケット爆発");
        float time = 0;

        while (time < 3.0f)
        {
            time += Time.deltaTime;
            Floating(player, explodeRiseSpeed);
            yield return null;
        }
        DropOut();
        yield break;
    }

    void Floating(GameObject floated, float floatSpeed)
    {
        Debug.Log($"floated確認：{floated}");

        playerRb.useGravity = false;
        Collider collider = player.GetComponent<CapsuleCollider>();
        collider.isTrigger = true;
        player.transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    void DropOut()
    {
        Debug.Log("脱落処理開始");

        SetPlayerBool spb = player.GetComponent<SetPlayerBool>();
        if (spb.isDead) return;

        PhotonView photonView = player.GetComponent<PhotonView>();
        PhotonView timePhoton = GameObject.Find("TimeManager").GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            timePhoton.RPC("IsTimeStart", RpcTarget.All, false);
            timeManager.ResetRocketCount();
        }

        uiLogManager.AddLog("player", UILogManager.LogType.Dead);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ロケットを配る");
            gameManager.ChooseRocketPlayer();
            timePhoton.RPC("IsTimeStart", RpcTarget.All, true);
            photonView.RPC("SetPlayerDead", RpcTarget.All, true);
        }
    }

}
