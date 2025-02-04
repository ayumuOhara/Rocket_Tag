using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private EventData eventData;  // EventData�̎Q��

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // �C�ӂ̃^�C�~���O�ŃC�x���g�������_���Ŕ���������ꍇ�͈ȉ����g�p
        if (Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftControl))  // Space�L�[���������Ƃ��ɃC�x���g�𔭐�
        {
            TriggerRandomEvent();
        }
    }

    // �����_���ɃC�x���g��I�����郁�\�b�h
    void TriggerRandomEvent()
    {
        int totalPercent = 0;

        // �C�x���g�̊m���̍��v���v�Z
        foreach (var eventSetting in eventData.EventSettings)
        {
            totalPercent += eventSetting.eventPercent;
        }
        
        // �����_���Ȑ��𐶐��i0����totalPercent�̊ԁj
        int randomValue = Random.Range(0, totalPercent);

        int eventPer = 0;

        // �m���Ɋ�Â��ă����_���ɃC�x���g��I��
        foreach (var eventSetting in eventData.EventSettings)
        {
            eventPer += eventSetting.eventPercent;

            // �����_���Ȓl�����݂̃C�x���g�͈͓̔��Ɏ��܂����ꍇ�A���̃C�x���g��I��
            if (randomValue < eventPer)
            {
                HandleEvent(eventSetting.EVENT_TYPE);
                break;
            }
        }
    }

    // �C�x���g���������郁�\�b�h
    void HandleEvent(EventData.EventType EVENT_TYPE)
    {
        // �C�x���g���Ƃ̏������L�q
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
