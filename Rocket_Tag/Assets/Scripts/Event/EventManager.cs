using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EventManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameManager gameManager;
    [SerializeField] private EventData eventData;  // EventDataの参照
    [SerializeField] GameObject blindEffect;       // 目つぶしイベント用UI

    // ランダムにイベントを選択するメソッド
    public IEnumerator TriggerRandomEvent()
    {
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
                StartCoroutine(BlindEvent());
                break;

            case EventData.EventType.BOMB_AREA:
                StartCoroutine(BombAreaEvent());
                break;

            case EventData.EventType.CHANGE_POS:
                photonView.RPC("ChangePos", RpcTarget.All);
                break;

            case EventData.EventType.JANNKENN:
                StartCoroutine(JannKennEvent());
                break;

            case EventData.EventType.RANDOM_SPEED:
                StartCoroutine(RandomSpeedEvent());
                break;

            case EventData.EventType.RANDOM_SKILL:
                StartCoroutine(RandomSkillEvent());
                break;

            default:
                Debug.Log("存在しません");
                break;
        }
    }

    int blindTime = 5;
    bool isBlind = false;
    // 目つぶしイベント
    IEnumerator BlindEvent()
    {
        photonView.RPC("BlindEffect", RpcTarget.All);
        yield return new WaitForSeconds(blindTime);
        photonView.RPC("BlindEffect", RpcTarget.All);

        yield break;
    }

    [PunRPC]
    void BlindEffect()
    {
        blindEffect.SetActive(!isBlind);
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

    // じゃんけんイベント
    IEnumerator JannKennEvent()
    {
        yield break;
    }


    // 移動速度変化イベント
    IEnumerator RandomSpeedEvent()
    {
        yield break;
    }

    // スキルランダム配布イベント
    IEnumerator RandomSkillEvent()
    {
        yield break;
    }
}
