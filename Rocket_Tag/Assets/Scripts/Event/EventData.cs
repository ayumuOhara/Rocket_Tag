using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData")]
public class EventData : ScriptableObject
{
    [SerializeField] private List<Event> eventSettings = new List<Event>();

    private Dictionary<EventType, int> eventDictionary; // �����A�N�Z�X�p

    [Serializable]
    public class Event
    {
        public string eventName;       // �C�x���g��
        public EventType EVENT_TYPE;   // �C�x���g�̎��
        public int eventPercent;       // �e�C�x���g���Ƃ̐��l
    }

    public enum EventType
    {
        BLIND,
        BOMB_AREA,
        CHANGE_POS,
        RANDOM_SPEED,
        RANDOM_SKILL,
    }

    // Dictionary ������������
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

    // eventSettings�i���X�g�j���擾����v���p�e�B
    public List<Event> EventSettings => eventSettings;
}
