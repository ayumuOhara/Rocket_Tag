using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public PlayerController playerController;
    public SetPlayerBool setPlayerBool;

    [SerializeField] EventManager eventManager;
    [SerializeField] TimeManager timeManager;
    [SerializeField] InstantiatePlayer instantiatePlayer;
    [SerializeField] PlayerReady playerReady;
    [SerializeField] TextMeshProUGUI playerCntText;     // Ready完了しているプレイヤー数
    [SerializeField] TextMeshProUGUI infoText;          // playerCntTextの説明文
    [SerializeField] GameObject eventTextObj;
    [SerializeField] TextMeshProUGUI eventText;

    //[SerializeField] GameObject rocketEffect;           // ロケットのエフェクト管理オブジェクト
    //[SerializeField] RocketEffect rocketEffect;           // ロケットエフェクトのインスタンス    for debug--------------------------
    private const int JOIN_CNT_MIN = 2;                 // 参加人数の最小値
    private bool isGameStarted = false;                 // ゲームが開始されたかどうかのフラグ
    private bool hasPlayedCountdownSE = false;          // カウントダウンSEが再生されたかどうかの判定
    private Player currentRocketHolder;                 // 現在のロケット保持者
    private List<GameObject> cachedPlayerList = new List<GameObject>(); // プレイヤーリストのキャッシュ

    int waitTime = 10;

    void Start()
    {
        
    }

    [PunRPC]
    IEnumerator WaitTimer()
    {
        photonView.RPC("PlayerCntText", RpcTarget.All, GetPlayerList().Count, "参加人数");
        waitTime = 10;
        eventTextObj.SetActive(true);

        while (true)
        {
            waitTime--;

            eventText.text = $"{waitTime}秒後にゲームを開始します";

            if(waitTime <= 3 && !hasPlayedCountdownSE)
            {
                AudioManager.Instance.PlaySE(SEManager.SEType.Countdown);
                hasPlayedCountdownSE = true;
            }

            if (waitTime <= 0)
            {
                eventTextObj.SetActive(false);
                photonView.RPC(nameof(StartGame), RpcTarget.All);
                yield break;
            }

            yield return new WaitForSeconds(1);
        }
    }

    public bool CheckJoinedPlayer()
    {
        var currentCnt = instantiatePlayer.GetCurrentPlayerCount();
        return currentCnt >= JOIN_CNT_MIN;
    }

    //bool CheckAllPlayersReady()
    //{
    //    Player[] players = PhotonNetwork.PlayerList;
    //    foreach (var player in players)
    //    {
    //        if (!player.CustomProperties.ContainsKey("IsReady") || !(bool)player.CustomProperties["IsReady"])
    //        {
    //            Debug.Log($"プレイヤー {player.NickName} がまだ準備完了していません");
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    //int GetReadyPlayerCount()
    //{
    //    Player[] players = PhotonNetwork.PlayerList;
    //    int readyCount = 0;

    //    foreach (var player in players)
    //    {
    //        if (player.CustomProperties.ContainsKey("IsReady") && (bool)player.CustomProperties["IsReady"])
    //        {
    //            readyCount++;
    //        }
    //    }
    //    return readyCount;
    //}

    [PunRPC]
    void StartGame()
    {
        if (isGameStarted) return;

        Debug.Log("ゲームを開始します");
        isGameStarted = true;
        timeManager.isTimeStart = true;
        StartCoroutine(CheckSurvivorCount());

        if (PhotonNetwork.IsMasterClient)
        {
            ChooseRocketPlayer();
            StartCoroutine(eventManager.TriggerRandomEvent());
            //StartCoroutine(CheckOverTime());
            //StartCoroutine(CheckRocketCnt());
        }
    }

    public void ChooseRocketPlayer()
    {
        // プレイヤーリストをキャッシュ
        cachedPlayerList = GetPlayerList();
        cachedPlayerList.RemoveAll(player => player.GetComponent<PhotonView>().Owner == currentRocketHolder); // 既存保持者を除外

        if (cachedPlayerList.Count == 0)
        {
            Debug.LogWarning("候補者がいません");
            return;
        }

        int rnd = Random.Range(0, cachedPlayerList.Count);
        GameObject selectedPlayer = cachedPlayerList[rnd];
        PhotonView targetPhotonView = selectedPlayer.GetComponent<PhotonView>();

        if (targetPhotonView != null)
        {
            Debug.Log("プレイヤーにロケットを渡しました");
            currentRocketHolder = targetPhotonView.Owner;
            targetPhotonView.RPC("SetHasRocket", RpcTarget.All, true);
            //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.SEARCH_ROCKET);    //  ロケット取得　for debug---------------
            //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_FRAMES);    //  ロケット炎生成  for debug------------
        }
        else
        {
            Debug.LogWarning("PhotonView が見つかりません");
        }
    }

    IEnumerator CheckSurvivorCount()
    {
        while (true)
        {
            int playerCount = GetPlayerList().Count;
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("PlayerCntText", RpcTarget.All, playerCount, "生存人数");
            }

            if (playerCount <= 1)
            {
                Debug.Log("生存人数が１人になったのでゲームを終了します");
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene("Result");
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator CheckOverTime()
    {
        while (true)
        {
            if(timeManager.rocketTime < -20.0f)
            {
                timeManager.ResetRocketCount();
                ChooseRocketPlayer();
            }
            yield return null;
        }
    }

    IEnumerator CheckRocketCnt()
    {
        Debug.Log("コルーチンを開始します");
        int rocketCnt = 0;

        while (true)
        {
            List<GameObject> players = GetPlayerList();

            Debug.Log("人数：" + GetPlayerList().Count);
            rocketCnt = 0;

            for(int i = 0; i < players.Count; i++)
            {
                SetPlayerBool spb = players[i].GetComponent<SetPlayerBool>();
                if(spb.hasRocket)
                {
                    rocketCnt++;
                }
            }

            Debug.Log("rocketCnt：" +  rocketCnt);

            if(rocketCnt != 1)
            {
                Debug.Log("ロケットを再配布");

                for(int i = 0;i < players.Count;i++)
                {
                    PhotonView photon = players[i].GetComponent<PhotonView>();
                    photon.RPC("SetHasRocket", RpcTarget.All, false);
                }

                ChooseRocketPlayer();
            }

            yield return null;
        }
    }

    [PunRPC]
    void PlayerCntText(int playerCnt, string text)
    {
        playerCntText.text = $"{playerCnt} / {instantiatePlayer.GetCurrentPlayerCount()}";
        infoText.text = $"{text} / 参加人数";
    }

    public List<GameObject> GetPlayerList()
    {
        List<GameObject> players = new List<GameObject>();
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        players.RemoveAll(player =>
        {
            SetPlayerBool spb = player.GetComponent<SetPlayerBool>();
            return spb != null && spb.isDead;
        });

        return players;
    }

    public Player GetCurrentRocketHolder()
    {
        return currentRocketHolder;
    }

}
