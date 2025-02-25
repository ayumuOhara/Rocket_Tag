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

    // ロケットカウントを全プレイヤーで同期
    public void SyncRocketCount(float count)
    {
        if (!PhotonNetwork.InRoom) // ルームに入っているか確認
        {
            Debug.LogWarning("ルームに入る前に SyncRocketCount() が呼ばれました。処理をスキップします。");
            return;
        }

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "RocketCount", count }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    // 同期したロケットカウントを取得
    float GetSyncRocketCount()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RocketCount", out object value))
        {
            return (float)value;
        }
        return initialTime; // デフォルト値を返す
    }

    float GetSecUntilZero(float limit, float minusValuePerSecond, float timeStep)
    {
        if (minusValuePerSecond <= 0)
        {
            Debug.LogWarning("減少量が0以下です。計算できません。");
            return float.MaxValue; // 無限大を返す
        }

        return limit / (minusValuePerSecond * (1 / timeStep));
    }

    void CountDown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // マスタークライアントのみタイマーを更新
            rocketTime -= Time.deltaTime + decreaseValue[(int)decreaseLevel] * Time.deltaTime;

            SyncRocketCount(rocketTime);
        }
        else
        {
            // マスタークライアント以外が更新されたタイマーを取得
            rocketTime = GetSyncRocketCount();
        }
        
        posessingTime += Time.deltaTime;
        rocketCountText.text = $"{rocketTime.ToString("F1")} sec";
    }

    public bool IsFloatTime()    //  上昇開始時間か判断
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

    // 一定時間ごとに減少速度を上げる
    void CheckForLevelUp()
    {
        if (decreaseLevel < DecreaseLevel.THIRD && posessingTime > decreaseUpTime[(int)decreaseLevel])
        {
            LevelUp();
        }
    }

    // 減少速度を次のレベルにアップ
    void LevelUp()
    {
        decreaseLevel++;
        Debug.Log($"タイマーの減少速度がアップしました: {decreaseLevel}");
    }
    internal bool IsStageUpTime()    //  ロケット炎エフェクト変化時間か判定
    {
        return stageUpTime[rocketEffect._RocketStage] > rocketTime;
    }

    // 加速度をリセットし、関連カウントを初期化
    public void ResetAcceleration()
    {
        Debug.Log("加速度をリセットします");
        posessingTime = 0;
        Debug.Log($"所持経過時間：{posessingTime}");
        decreaseLevel = DecreaseLevel.FIRST;
        Debug.Log($"加速度レベル：{decreaseLevel}");
    }
}
