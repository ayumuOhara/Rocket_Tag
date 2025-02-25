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
    float timeLimit = 0;
    public float rocketTime = 100;
    float initialTime = 100;

    float posessingTime = 0;
    float secToExplode  = 0;

    float[] decreaseValue  = { 1.0f, 3.0f, 6.0f };
    float[] decreaseUpTime = { 10, 20, 30 };
    float[] stageUpTime = {100, 70, 30, 7};

    float floatStartTime = 2.2f;
    public bool isTimeStart = false;
    bool isTimeStop = false;

    public PhotonView timerView;
    [SerializeField] TextMeshProUGUI rocketCountText;
    [SerializeField] RocketEffect rocketEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isTimeStart = false;
        isTimeStop = false;
        timerView = GetComponent<PhotonView>();
        //rocketEffect = GameObject.Find("Debuger").GetComponent<RocketEffect>();
        Initialize();
    }


    // Update is called once per frame
    void Update()
    {
        if(isTimeStart == true)
        {
            CountDown();
            CheckForLevelUp();
        }        
    }

    void Initialize()
    {
        secToExplode = GetSecUntilZero(rocketTime, decreaseValue[(int)decreaseLevel], Time.deltaTime);
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
        return initialTime; // �f�t�H���g�l��Ԃ�
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
            rocketTime -= Time.deltaTime + decreaseValue[(int)decreaseLevel] * Time.deltaTime;

            SyncRocketCount(rocketTime);
        }
        else
        {
            // �}�X�^�[�N���C�A���g�ȊO���X�V���ꂽ�^�C�}�[���擾
            rocketTime = GetSyncRocketCount();
        }
        
        posessingTime += Time.deltaTime;
        rocketCountText.text = $"{rocketTime.ToString("F1")} sec";
    }

    public bool IsFloatTime()    //  �㏸�J�n���Ԃ����f
    {
        return floatStartTime > rocketTime;
    }

    public bool IsLimitOver()
    {
        return rocketTime <= timeLimit;
    }

    [PunRPC]
    public void IsTimeStop(bool newIsTimeStop)
    {
        isTimeStop = newIsTimeStop;
    }

    public void ResetRocketCount()
    {
        rocketTime = initialTime;
        SyncRocketCount(rocketTime);
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
    internal bool IsStageUpTime()    //  ���P�b�g���G�t�F�N�g�ω����Ԃ�����
    {
        return stageUpTime[rocketEffect._RocketStage] > rocketTime;
    }

    // �����x�����Z�b�g���A�֘A�J�E���g��������
    public void ResetAcceleration()
    {
        Debug.Log("�����x�����Z�b�g���܂�");
        posessingTime = 0;
        Debug.Log($"�����o�ߎ��ԁF{posessingTime}");
        decreaseLevel = DecreaseLevel.FIRST;
        Debug.Log($"�����x���x���F{decreaseLevel}");
    }
}
