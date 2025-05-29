using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using Photon.Chat.Demo;

public class EventManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameManager gameManager;
    [SerializeField] UILogManager uiLogManager;
    [SerializeField] EnoguEvent enoguEvent;
    [SerializeField] GameObject eventTextObj;
    [SerializeField] TextMeshProUGUI eventText;
    [SerializeField] EventEffect eventEffect;
    [SerializeField] private EventData eventData;          // EventDataの参照
    [SerializeField] private SkillDataBase skillDataBase;  // SkillDataの参照
    [SerializeField] float time = 0;
    [SerializeField] float triggerTime = 20.0f;

    private void Start()
    {
        eventTextObj.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(TriggerRandomEvent());
        }
    }

    // ランダムにイベントを選択するメソッド
    public IEnumerator TriggerRandomEvent()
    {
        Debug.Log("イベント抽選開始");

        while (true)
        {
            if (time > triggerTime)
            {
                int totalPercent = 0;

                // イベントの確率の合計を計算
                foreach (var eventSetting in eventData.EventSettings)
                {
                    totalPercent += eventSetting.eventPercent;
                }

                // ランダムな数を生成（0からtotalPercentの間）
                int randomValue = Random.Range(0, totalPercent);

                int eventPer = 0;

                // 確率に基づいてランダムにイベントを選択
                foreach (var eventSetting in eventData.EventSettings)
                {
                    eventPer += eventSetting.eventPercent;

                    // ランダムな値が現在のイベントの範囲内に収まった場合、そのイベントを選択
                    if (randomValue < eventPer)
                    {
                        HandleEvent(eventSetting.EVENT_TYPE);
                        break;
                    }
                }
                time = 0;
                yield return new WaitForSeconds(1);
            }
            else
            {
                time++;
                
                if (time >= triggerTime - 3.0 && triggerTime - time >= 0)
                {
                    float cntTime = 0;
                    cntTime = triggerTime - time;
                    photonView.RPC("CntDownText",RpcTarget.All, cntTime);
                }

                yield return new WaitForSeconds(1);
            }
        }
    }

    [PunRPC]
    void CntDownText(float time)
    {
        eventTextObj.SetActive(true);
        eventText.text = $"イベント発生まで{time}";
    }

    // イベントを処理するメソッド
    void HandleEvent(EventData.EventType EVENT_TYPE)
    {
        eventTextObj.SetActive(true);
        // イベントごとの処理を記述
        switch (EVENT_TYPE)
        {
            case EventData.EventType.BLIND:
                Debug.Log("目隠しイベント開始");
                StartCoroutine(BlindEvent());
                uiLogManager.AddLog("メカクシ", UILogManager.LogType.Event);
                break;

            case EventData.EventType.CHANGE_POS:
                Debug.Log("位置入れ替えイベント開始");
                photonView.RPC("ChangePos", RpcTarget.All);
                uiLogManager.AddLog("位置入れ替え", UILogManager.LogType.Event);
                break;

            case EventData.EventType.RANDOM_SPEED:
                Debug.Log("速度変化イベント開始");
                StartCoroutine(RandomSpeedEvent());
                uiLogManager.AddLog("速度変化", UILogManager.LogType.Event);
                break;

            default:
                Debug.Log("存在しません");
                break;
        }

        photonView.RPC("TextOnDisplay",RpcTarget.All,EVENT_TYPE);
    }

    [PunRPC]
    public IEnumerator TextOnDisplay(EventData.EventType EVENT_TYPE)
    {
        eventTextObj.SetActive(true);
        switch (EVENT_TYPE)
        {
            case EventData.EventType.BLIND:
                Debug.Log("目隠しイベント開始");
                eventText.text = $"画面がインクで見えない！";
                break;

            case EventData.EventType.CHANGE_POS:
                Debug.Log("位置入れ替えイベント開始");
                eventText.text = $"プレイヤーの\n位置が入れ替わった！";
                break;

            case EventData.EventType.RANDOM_SPEED:
                Debug.Log("速度変化イベント開始");
                eventText.text = $"プレイヤーの\n運動能力が変化した";
                break;

            default:
                Debug.Log("存在しません");
                break;
        }
        yield return new WaitForSeconds(5.0f);
        eventTextObj.SetActive(false);
    }

    // 目つぶしイベント
    IEnumerator BlindEvent()
    {
        //AudioManager.Instance.PlaySE(SEManager.SEType.Event_ink); //インクSE
        //List<GameObject> playerList = gameManager.GetPlayerList();
        float eventTime = 10.0f;
        enoguEvent.PaintOpen();
        yield return new WaitForSeconds(eventTime);
        enoguEvent.PaintClose();
        eventEffect._AssignPlayerTF();
        yield break;
    }

    // プレイヤーの位置入れ替えイベント    
    [PunRPC]
    void ChangePos()
    {
        List<GameObject> playerList = gameManager.GetPlayerList();
        List<Vector3> playerPos = new List<Vector3>();
        AudioManager.Instance.PlaySE(SEManager.SEType.Event_warp); //ワープSE

        // 現在のプレイヤーの座標を保存
        foreach (GameObject player in playerList)
        {
            playerPos.Add(player.transform.position);
        }

        // プレイヤーの座標をシャッフル
        for (int i = 0; i < playerPos.Count; i++)
        {
            int rnd = Random.Range(0, playerPos.Count);
            (playerPos[i], playerPos[rnd]) = (playerPos[rnd], playerPos[i]); // C# のタプルスワップ
        }

        photonView.RPC("CallEffectProcces", RpcTarget.All, (int)EventEffect.EventEffectProcess.TELEPORT_SMOKE);    //  テレポートエフェクト生成

        // 新しい座標をプレイヤーに適用
        for (int i = 0; i < playerList.Count; i++)
        {
            Debug.Log(playerList[i]);
            //photonView.RPC("GenerateEffect", RpcTarget.All, (int)EventEffect.EventEffectNo.TELEPORT_SMOKE, playerList[i].transform.position, i);
            //eventEffect.GenerateEffect((int)EventEffect.EventEffectNo.TELEPORT_SMOKE, playerList[i].transform, i);    //  エフェクト生成
            playerList[i].gameObject.transform.position = playerPos[i];
        }
        eventEffect._IsGeneratedSmoke = true;
    }

    // 移動速度変化イベント
    IEnumerator RandomSpeedEvent()
    {
        float eventTime = 15.0f;
        List<GameObject> playerList = gameManager.GetPlayerList();
        ChangeSpeed(playerList);
        yield return new WaitForSeconds(eventTime);
        ResetSpeed(playerList);

        yield break;
    }

    // ランダムに移動速度を変化
    [PunRPC]
    void ChangeSpeed(List<GameObject> playerList)
    {
        int minSpeed = 10;
        int maxSpeed = 30;

        foreach (GameObject player in playerList)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            int rndSpeed = Random.Range(minSpeed, maxSpeed);
            PhotonView photon = player.GetComponent<PhotonView>();
            photon.RPC("PlayChangeSpeedSE", photon.Owner);
            photon.RPC("SetMoveSpeed", RpcTarget.All, (float)rndSpeed);
        }
        photonView.RPC("CallEffectProcces", RpcTarget.All, (int)EventEffect.EventEffectProcess.MOVE_SPD_AURA);    //  テレポートエフェクト生成
    }

    // 移動速度を元に戻す
    [PunRPC]
    void ResetSpeed(List<GameObject> playerList)
    {
        foreach (GameObject player in playerList)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            PhotonView photon = player.GetComponent<PhotonView>();
            photon.RPC("SetMoveSpeed", RpcTarget.All, playerMovement.GetDefaultMoveSpeed());
        }
        photonView.RPC("CallEffectProcces", RpcTarget.All, (int)EventEffect.EventEffectProcess.STOP_SPD_AURA);    //  テレポートエフェクト停止
    }
}
