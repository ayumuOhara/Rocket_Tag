using Photon.Pun;
using UnityEngine;

public class TimeManager : MonoBehaviour
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
    float[] decreaseValue  = { 1.0f, 5.0f, 10.0f };
    float[] decreaseUpTime = { 10, 20, 30 };
    public bool isTimeStart = false;
    public bool isTimeStop = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isTimeStart = false;
        isTimeStop  = false;
        ResetRocketCount();
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
    void SyncRocketCount(float count)
    {
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
        if(PhotonNetwork.IsMasterClient)
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
    }

    public bool IsLimitOver()
    {
        return rocketLimit > rocketCount;
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
