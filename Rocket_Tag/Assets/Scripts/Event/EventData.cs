using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData")]
public class EventData : ScriptableObject
{
    [SerializeField] private List<Event> eventSettings = new List<Event>();

    private Dictionary<EventType, int> eventDictionary; // 高速アクセス用

    [Serializable]
    public class Event
    {
        public string eventName;       // イベント名
        public EventType EVENT_TYPE;   // イベントの種類
        public int eventPercent;       // 各イベントごとの数値
    }

    public enum EventType
    {
        BLIND,
        BOMB_AREA,
        CHANGE_POS,
        RANDOM_SPEED,
        RANDOM_SKILL,
    }

    // Dictionary を初期化する
    private void OnEnable()
    {
        eventDictionary = new Dictionary<EventType, int>();
        foreach (var eventData in eventSettings)
        {
            if (!eventDictionary.ContainsKey(eventData.EVENT_TYPE))
            {
                eventDictionary.Add(eventData.EVENT_TYPE, eventData.eventPercent);
            }
        }
    }

    // eventSettings（リスト）を取得するプロパティ
    public List<Event> EventSettings => eventSettings;
}
