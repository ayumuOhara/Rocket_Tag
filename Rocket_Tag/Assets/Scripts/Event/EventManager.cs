using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EventManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameManager gameManager;
    [SerializeField] UILogManager uiLogManager;
    [SerializeField] private EventData eventData;          // EventDataの参照
    [SerializeField] private SkillDataBase skillDataBase;  // SkillDataの参照
    [SerializeField] GameObject blindEffect;               // 目つぶしイベント用UI

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(TriggerRandomEvent());
        }
    }

    // ランダムにイベントを選択するメソッド
    public IEnumerator TriggerRandomEvent()
    {
        Debug.Log("イベント抽選開始");

        while(true)
        {
            yield return new WaitForSeconds(30.0f);

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
        }        
    }

    // イベントを処理するメソッド
    void HandleEvent(EventData.EventType EVENT_TYPE)
    {
        // イベントごとの処理を記述
        switch (EVENT_TYPE)
        {
            case EventData.EventType.BLIND:
                Debug.Log("目隠しイベント開始");
                StartCoroutine(BlindEvent());
                uiLogManager.AddLog("メカクシ", UILogManager.LogType.Event);
                break;

            case EventData.EventType.BOMB_AREA:
                Debug.Log("エリアイベント開始");
                StartCoroutine(BombAreaEvent());
                uiLogManager.AddLog("エリア", UILogManager.LogType.Event);
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

            case EventData.EventType.RANDOM_SKILL:
                Debug.Log("スキル変化イベント開始");
                StartCoroutine(RandomSkillEvent());
                uiLogManager.AddLog("スキルチェンジ", UILogManager.LogType.Event);
                break;

            default:
                Debug.Log("存在しません");
                break;
        }
    }

    // 目つぶしイベント
    IEnumerator BlindEvent()
    {
        float eventTime = 10.0f;
        photonView.RPC("BlindEffect", RpcTarget.All, true);
        yield return new WaitForSeconds(eventTime);
        photonView.RPC("BlindEffect", RpcTarget.All, false);

        yield break;
    }

    [PunRPC]
    void BlindEffect(bool isBlind)
    {
        blindEffect.SetActive(isBlind);
    }

    // エリアイベント
    IEnumerator BombAreaEvent()
    {
        yield break;
    }

    // プレイヤーの位置入れ替えイベント    
    [PunRPC]
    void ChangePos()
    {
        List<GameObject> playerList = gameManager.GetPlayerList();
        List<Vector3> playerPos = new List<Vector3>();

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

        // 新しい座標をプレイヤーに適用
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].transform.position = playerPos[i];
        }
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
    void ChangeSpeed(List<GameObject> playerList)
    {
        int minSpeed = 10;
        int maxSpeed = 30;

        foreach (GameObject player in playerList)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            int rndSpeed = Random.Range(minSpeed, maxSpeed);
            Debug.Log($"速度変化：{rndSpeed}");
            playerMovement.SetMoveSpeed(rndSpeed);
        }
    }

    // 移動速度を元に戻す
    void ResetSpeed(List<GameObject> playerList)
    {
        foreach (GameObject player in playerList)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.SetMoveSpeed(playerMovement.GetDefaultMoveSpeed());
        }
    }

    // スキルランダム配布イベント
    IEnumerator RandomSkillEvent()
    {
        List<GameObject> playerList = gameManager.GetPlayerList();

        foreach (GameObject player in playerList)
        {
            SkillManager skillManager = player.gameObject.GetComponent<SkillManager>();
            int rnd = Random.Range(0, skillDataBase.SkillData.Count);

            SkillData giveSkill = skillDataBase.SkillData[rnd];
            skillManager.SetSkill(giveSkill);
        }

        playerList.Clear();
        yield break;
    }
}
