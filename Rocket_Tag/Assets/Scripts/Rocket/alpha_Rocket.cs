using Photon.Pun;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Alpha_Rocket : MonoBehaviourPunCallbacks
{
    float floatSpeed = 5f;
    float explodeRiseSpeed = 20f;
    float evacuateStarPos_Y = 40;
    bool isExploding = false;

    Vector3 effectOffset = new Vector3(0, -1, 0);
    Vector3 smokeDiffusion = new Vector3(3, 0, 3);

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
        timeManager.ResetRocketCount();

        Debug.Log("ロケット爆発");
        while (!IsVeryHigh())
        {
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

    bool IsVeryHigh()
    {
        return transform.position.y > evacuateStarPos_Y;
    }

    void DropOut()
    {
        PhotonView photonView = player.GetComponent<PhotonView>();

        //string playerName = PhotonNetwork.NickName;
        uiLogManager.AddLog("player", UILogManager.LogType.Dead);

        if (photonView.IsMine)
        {
            Debug.Log("ロケットを抽選");
            gameManager.ChooseRocketPlayer();
            photonView.RPC("SetPlayerDead", RpcTarget.All, true);
        }
    }
}
