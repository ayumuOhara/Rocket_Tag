using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Alpha_Rocket : MonoBehaviourPunCallbacks
{
    float floatSpeed = 5f;
    float explodeRiseSpeed = 10f;
    float evacuateStarPos_Y = 40;

    GameManager gameManager;
    TimeManager timeManager;
    [SerializeField] GameObject player;
    Rigidbody playerRb;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        playerRb = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(timeManager.IsFloatTime() && !timeManager.IsLimitOver())
        {
            playerRb.useGravity = false;
            Floating(player, floatSpeed);
        }
        if(timeManager.IsLimitOver())
        {
            timeManager.ResetRocketCount();
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
            gameManager.ChooseRocketPlayer();
        }

        PhotonView photonView = player.GetComponent<PhotonView>();
        photonView.RPC("SetPlayerDead", RpcTarget.All, true);
    }
}
