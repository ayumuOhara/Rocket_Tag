using Photon.Pun;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviourPunCallbacks
{
    enum DecreaseLevel
    {
        FIRST,
        SECOND,
        THIRD,
    }

    DecreaseLevel decreaseLevel = DecreaseLevel.FIRST;
    float rocketLimit = 0;
    public float rocketCount  = 100;
    public float initialCount = 100;
    float posessingTime = 0;
    float secToExplode  = 0;
    float[] decreaseValue  = { 1.0f, 3.0f, 6.0f };
    float[] decreaseUpTime = { 10, 20, 30 };
    public bool isTimeStart = false;
    bool isTimeStop = false;

    public PhotonView timerView;
    [SerializeField] TextMeshProUGUI rocketCountText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isTimeStart = false;
        isTimeStop = false;
        timerView = GetComponent<PhotonView>();

        Initialize();
    }


    // Update is called once per frame
    void Update()
    {
        if(isTimeStart == true && isTimeStop == false)
        {
            CountDown();
            CheckForLevelUp();
        }        
    }

    void Initialize()
    {
        secToExplode = GetSecUntilZero(rocketCount, decreaseValue[(int)decreaseLevel], Time.deltaTime);
    }

    // ���P�b�g�J�E���g��S�v���C���[�œ���
    public void SyncRocketCount(float count)
    {
        if (!PhotonNetwork.InRoom) // ���[���ɓ����Ă��邩�m�F
        {
            Debug.LogWarning("���[���ɓ���O�� SyncRocketCount() ���Ă΂�܂����B�������X�L�b�v���܂��B");
            return;
        }

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "RocketCount", count }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    // �����������P�b�g�J�E���g���擾
    float GetSyncRocketCount()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RocketCount", out object value))
        {
            return (float)value;
        }
        return initialCount; // �f�t�H���g�l��Ԃ�
    }

    float GetSecUntilZero(float limit, float minusValuePerSecond, float timeStep)
    {
        if (minusValuePerSecond <= 0)
        {
            Debug.LogWarning("�����ʂ�0�ȉ��ł��B�v�Z�ł��܂���B");
            return float.MaxValue; // �������Ԃ�
        }

        return limit / (minusValuePerSecond * (1 / timeStep));
    }

    void CountDown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // �}�X�^�[�N���C�A���g�̂݃^�C�}�[���X�V
            rocketCount -= Time.deltaTime + decreaseValue[(int)decreaseLevel] * Time.deltaTime;
            SyncRocketCount(rocketCount);
        }
        else
        {
            // �}�X�^�[�N���C�A���g�ȊO���X�V���ꂽ�^�C�}�[���擾
            rocketCount = GetSyncRocketCount();
        }

        posessingTime += Time.deltaTime;
        rocketCountText.text = $"{rocketCount.ToString("F1")} sec";
    }

    public bool IsLimitOver()
    {
        return rocketLimit > rocketCount;
    }

    [PunRPC]
    public void IsTimeStop(bool newIsTimeStop)
    {
        isTimeStop = newIsTimeStop;
    }

    public void ResetRocketCount()
    {
        rocketCount = initialCount;
        SyncRocketCount(rocketCount);
    }

    // ��莞�Ԃ��ƂɌ������x���グ��
    void CheckForLevelUp()
    {
        if (decreaseLevel < DecreaseLevel.THIRD && posessingTime > decreaseUpTime[(int)decreaseLevel])
        {
            LevelUp();
        }
    }

    // �������x�����̃��x���ɃA�b�v
    void LevelUp()
    {
        decreaseLevel++;
        Debug.Log($"�^�C�}�[�̌������x���A�b�v���܂���: {decreaseLevel}");
    }

    // �����x�����Z�b�g���A�֘A�J�E���g��������
    public void ResetAcceleration()
    {
        Debug.Log("�����x�����Z�b�g���܂�");
        decreaseLevel = DecreaseLevel.FIRST; // ������Ԃɖ߂�
    }
}
