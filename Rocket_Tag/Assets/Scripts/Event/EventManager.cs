using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private EventData eventData;  // EventDataの参照

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 任意のタイミングでイベントをランダムで発生させる場合は以下を使用
        if (Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftControl))  // Spaceキーを押したときにイベントを発生
        {
            TriggerRandomEvent();
        }
    }

    // ランダムにイベントを選択するメソッド
    void TriggerRandomEvent()
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
    }

    // イベントを処理するメソッド
    void HandleEvent(EventData.EventType EVENT_TYPE)
    {
        // イベントごとの処理を記述
        switch (EVENT_TYPE)
        {
            case EventData.EventType.BLIND:
                Debug.Log("BLIND Event Triggered!");
                break;

            case EventData.EventType.BOMB_AREA:
                Debug.Log("BOMB_AREA Event Triggered!");
                break;

            case EventData.EventType.CHANGE_POS:
                Debug.Log("CHANGE_POS Event Triggered!");
                break;

            case EventData.EventType.JANNKENN:
                Debug.Log("JANNKENN Event Triggered!");
                break;

            case EventData.EventType.RANDOM_SPEED:
                Debug.Log("RANDOM_SPEED Event Triggered!");
                break;

            case EventData.EventType.RANDOM_SKILL:
                Debug.Log("RANDOM_SKILL Event Triggered!");
                break;

            default:
                Debug.LogWarning("Unknown Event Triggered!");
                break;
        }
    }
}
