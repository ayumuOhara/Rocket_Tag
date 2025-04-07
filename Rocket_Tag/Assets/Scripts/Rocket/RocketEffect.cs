using System;
using System.Collections;                                                                          ////  ロケットエフェクト生成・切り替え  ////
using System.Threading.Tasks;
//using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

internal interface EffectState                                                                     ////  以下State区  ////
{
    void Enter(RocketEffect arg);
    void Update(RocketEffect arg);
    void Exit(RocketEffect arg);
}
internal class FirstStage : EffectState   //  ロケット1段階目
{
    public void Enter(RocketEffect rocketEffect)
    {
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_FRAMES);
    }
    public void Update(RocketEffect rocketEffect)
    {
        if (rocketEffect._TimeMgr.IsStageUpTime())
        {
            rocketEffect.ChangeState(new SecondStage());
        }
    }
    public void Exit(RocketEffect rocketEffect)
    {

    }
}
internal class SecondStage : EffectState    //  ロケット2段階目
{
    public void Enter(RocketEffect rocketEffect)
    {
        Debug.Log(1);
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_FRAMES);
    }
    public void Update(RocketEffect rocketEffect)
    {
        if (rocketEffect._TimeMgr.IsStageUpTime())
        {
            rocketEffect.ChangeState(new ThirdStage());
        }
    }
    public void Exit(RocketEffect rocketEffect)
    {

    }
}
internal class ThirdStage : EffectState    //  ロケット3段階目
{
    public void Enter(RocketEffect rocketEffect)
    {
        Debug.Log(2);
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_FRAMES);    //  最終段階のエフェクト生成
    }
    public void Update(RocketEffect rocketEffect)
    {
        if (rocketEffect._TimeMgr.IsStageUpTime())
        {
            rocketEffect.ChangeState(new LastStage());
        }
    }
    public void Exit(RocketEffect rocketEffect)
    {

    }
}
internal class LastStage : EffectState    //  ロケット最終段階
{
    public void Enter(RocketEffect rocketEffect)
    {
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_FRAMES);    //  最終段階のエフェクト生成
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_SMOKE);    //  煙を取得
    }
    public void Update(RocketEffect rocketEffect)
    {
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.SMOKE_DIFFUSION);    //  煙を取得
    }
    public void Exit(RocketEffect rocketEffect)
    {

    }
}
internal class PrepareRocket : EffectState    //  次のロケットを用意している状態
{
    public void Enter(RocketEffect rocketEffect)
    {
    }
    public void Update(RocketEffect rocketEffect)
    {
        Debug.Log(123456789);
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.SEARCH_ROCKET);
    }
    public void Exit(RocketEffect rocketEffect)
    {

    }
}                                                                                                  ////  State区終了　　////
internal class RocketEffect : MonoBehaviour
{
    internal enum RocketEffectProcces    //  ロケットエフェクトの処理一覧                          ////  以下宣言区  ////
    {
        GENERATE_FRAMES,
        GENERATE_SMOKE,
        SEARCH_FRAME_SMOKE,
        SMOKE_DIFFUSION,
        SEARCH_ROCKET,
    }
    internal enum EffectNo    //  エフェクトの種類
    {
        FRAME,
        SMOKE,
    }

    EffectState currentState;

    GameObject[] frameEffectPrefab;
    GameObject frameEffectEntity;
    GameObject smokeEffectPrefab;
    GameObject smokeEntity;
    Transform rocket;
    ParticleSystem smokePS;
    ParticleSystem.MainModule smokeMainModule;
    ParticleSystem.ColorOverLifetimeModule smokeColorOverLifeTime;
    Gradient smokeGradient;
    TimeManager timeMgr;

    Vector3 frameEffectOffset;
    Vector3 smokeDiffusion;

    float smokeDelTime;
    int rocketStage;
    bool didFalsed;    //  ロケット生成にタイミングを合わせるためのフラグ
    const string rocketNotFound = "Error:Rocket Not Found";    //  msg for debug--------------
    const string rocketIsAssginedThis = "Rocket variable is assigned [this.transform]";
    const string couldntGetTimemgr = "Error:Couldn't Get timeMgr";    //  msg for debug---------------------
    const string scriptProssesFinish = "RocketEffect.cs's process is stop";    //  msg for debug------------------

    internal TimeManager _TimeMgr
    { get { return timeMgr; } }
    internal int _RocketStage
    { get { return rocketStage; } }                                                               ////  宣言区終了  ////
    internal bool _DidFalsed
    { get { return didFalsed; } }

    void OnEnable()                                                                                ////  以下処理区  ////
    {
        /*  処理順を合わせるため最初にSetActive(false)にする  */
        SetSetActive(didFalsed, this.gameObject);
        if (didFalsed)
        {
            Initialize();    //  初期化
        }
    }
    void OnDisable()
    {
        didFalsed = true;
    }
    void Update()
    {
        currentState.Update(this);
    }                                                                                              ////  処理区終了  ////
    void SetSetActive(bool flag, GameObject obj)    //  SetActiveを設定する
    {
        if (flag != obj.activeSelf)
        {
            obj.SetActive(flag);
        }
        else
        {
            obj.SetActive(false);
        }
    }
    async void Initialize()    //  初期化                                                                ////  以下関数区  ////
    {
        ChangeState(new FirstStage());
        
        await RocketEffectLoad();
        rocket = GameObject.Find("Rocket").GetComponent<Transform>();
        smokeGradient = new Gradient();
        smokeGradient.alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0f), new GradientAlphaKey(0.0f, 0.4f) };
        timeMgr = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        frameEffectOffset = new Vector3(0, -0.6f, 0.5f);
        smokeDiffusion = new Vector3(1.02f, 1.02f, 1.02f);

        rocketStage = 0;
        smokeDelTime = 12;

        if(IsNull_Variable(rocket, false, rocketNotFound))    //  msg for debug-----------------------
        {
            Debug.Log(rocketIsAssginedThis);    //  msg for debug-----------------
            rocket = this.transform;
        }
        if (IsNull_Variable(timeMgr, false, couldntGetTimemgr))
        {
            Debug.Log(scriptProssesFinish);    //  msg for debug-------------------
            return;
        }
    }
    internal void ChangeState(EffectState newState)    //  状態遷移
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }
        currentState = newState;
        currentState.Enter(this);
    }
    internal void RocketEffectWrapper(RocketEffectProcces rocketEffectProcces)   // ロケットエフェクトのラッパー関数
    {
        switch (rocketEffectProcces)
        {
            case RocketEffectProcces.GENERATE_FRAMES:
                {
                    GenerateEffect((int)EffectNo.FRAME, frameEffectPrefab[rocketStage], rocket, frameEffectOffset);
                    rocketStage = rocketStage != 3 ? ++rocketStage : 0;
                    break;
                }
            case RocketEffectProcces.GENERATE_SMOKE:
                {
                    GenerateEffect((int)EffectNo.SMOKE, smokeEffectPrefab, rocket, frameEffectOffset);
                    smokePS = smokeEntity.GetComponent<ParticleSystem>();
                    smokeMainModule = smokePS.main;
                    smokeMainModule.startColor = Color.white;
                    smokeColorOverLifeTime = smokePS.colorOverLifetime;
                    break;
                }
            case RocketEffectProcces.SEARCH_ROCKET:
                {
                    rocket = null;
                    rocket = GameObject.Find("Rocket").GetComponent<Transform>();
                    if (IsNull_Variable(rocket, false, rocketNotFound))    //  msg for debug-------------------
                    {
                        Debug.Log(rocketIsAssginedThis);    //  msg for debug--------------------
                        rocket = this.transform;
                    }
                    if (rocket != null && currentState is PrepareRocket)
                    {
                        ChangeState(new FirstStage());
                    }
                    break;
                }
            case RocketEffectProcces.SMOKE_DIFFUSION:
                {
                    SmokeDiffusion();
                    break;
                }
            default: break;
        }
    }
    void GenerateEffect(int effectNo, GameObject effect, Transform parent, Vector3 offset)    //  エフェクト生成
    {
        switch (effectNo)
        {
            case 0:
                {
                    if (frameEffectEntity != null)
                    {
                        Destroy(frameEffectEntity);
                    }
                    frameEffectEntity = Instantiate(effect, parent);
                    frameEffectEntity.transform.localPosition += offset;
                    break;
                }
            case 1:
                {
                    smokeEntity = Instantiate(smokeEffectPrefab);
                    smokeEntity.transform.position = rocket.position;
                    break;
                }
            default: break;
        }
    }
    void SmokeDiffusion()    //  煙幕拡散、煙幕をデストロイしたたらPrepareRocketStateに移動
    {
        if ((smokeDelTime -= Time.deltaTime) > 0)
        {
            smokeColorOverLifeTime.color = smokeGradient;
            smokeEntity.transform.localScale = Vector3.Scale(smokeEntity.transform.localScale, smokeDiffusion);
        }
        else
        {
            Debug.Log("TimeOut");    //  msg for debug----------------
            Destroy(smokeEntity.gameObject);
            ChangeState(new PrepareRocket());
        }
    }
    bool IsNull_Variable<T>(T value, bool haveToClach, string errorMsg)    //  変数のヌルチェック、危険性があった場合強制クラッシュ
    {
        if (value == null)
        {
            if (haveToClach)
            {
                Environment.FailFast(errorMsg);    //  クラッシュ
            }
            Debug.Log(errorMsg);    //  debug--------------------------
            return true;
        }
        return false;
    }
    bool IsNull_Array<T>     //  配列のヌルチェック、危険性があった場合強制クラッシュ
    (T[] value, bool isCheckPoint, int[] checkPoint, bool haveToClach, string errorMsg_PointNull, string errorMsg_AllNull)
    {
        if (value == null || value.Length == 0)
        {
            if (haveToClach)
            {
                Environment.FailFast(errorMsg_AllNull);    //  クラッシュ
            }
            Debug.Log(errorMsg_AllNull);    //  debug------------------
            return true;
        }
        if (isCheckPoint)
        {
            for (int arrayNo = checkPoint.Length - 1; arrayNo >= 0; arrayNo--)
            {
                if (value[checkPoint[arrayNo]] == null)
                {
                    if (haveToClach)
                    {
                        Environment.FailFast(errorMsg_PointNull);    //  クラッシュ
                    }
                    return true;
                }
                Debug.Log(errorMsg_PointNull);    //  debug-------------------
                return false;
            }
            Debug.Log(errorMsg_PointNull);    //  debug--------------------------
        }
        return false;
    }
    async Task RocketEffectLoad()    //  ロケットエフェクトのロード
    {
        Task[] loadTasks;

        AsyncOperationHandle<GameObject>[] loadHandles;

        const int numOfFrameEffect = 4;
        const int numOfSmokeEffect = 1;
        int loadHandleArrayNo;
        string[] frameEffectNames = {"FirstRocketFrame","SecondRocketFrame", "ThirdRocketFrame", "LastRocketFrame"};
        string smokeEffectName;

        loadTasks = new Task[numOfFrameEffect];

        frameEffectPrefab = new GameObject[numOfFrameEffect];

        loadHandles = new AsyncOperationHandle<GameObject>[numOfFrameEffect + numOfSmokeEffect];

        loadHandleArrayNo = 0;    //  同一的な配列の要素数を指定するために使うときもあります
        smokeEffectName = "FrameSmoke";

        for(string loadName; loadHandleArrayNo < numOfFrameEffect + numOfSmokeEffect; loadHandleArrayNo++)
        {
            if(loadHandleArrayNo < numOfFrameEffect)
            {
                loadHandles[loadHandleArrayNo] = Addressables.LoadAssetAsync<GameObject>(frameEffectNames[loadHandleArrayNo]);
            }
            else
            {
                loadHandles[loadHandleArrayNo] = Addressables.LoadAssetAsync<GameObject>(smokeEffectName);
            }
            loadTasks[loadHandleArrayNo] = loadHandles[loadHandleArrayNo].Task;
        }
        await Task.WhenAll(loadTasks);
        for(loadHandleArrayNo = 0; loadHandleArrayNo < numOfFrameEffect + numOfSmokeEffect; loadHandleArrayNo++)
        {
            if (loadHandleArrayNo < numOfFrameEffect)
            {
                frameEffectPrefab[loadHandleArrayNo] = loadHandles[loadHandleArrayNo].Result;
            }
            else
            {
                smokeEffectPrefab = loadHandles[loadHandleArrayNo].Result;
            }
        }
    }                                                                                              ////  以下関数区  ////
}                                                                                                 
                                                                                                   ////  以下コード保存場所  ////
/*    //void RocketEffectLoad()
    //{
    //    Task[] task;

    //    AsyncOperationHandle<GameObject>[] playerSkinLordHandle;

    //    const int numOfSkin = 6;

    //    task = new Task[numOfSkin - 1];
    //    playerSkinPrefab = new GameObject[numOfSkin];

    //    playerSkinLordHandle = new AsyncOperationHandle<GameObject>[numOfSkin];

    //    string[] skinNames = new string[] { "NotWearing", "RedCap", "StrawHat", "Eringi", "Freeza", "Bear" };

    //    /*  スキンは永久的に使うので開放していない  */
//    for (int arrayNo = numOfSkin - 1; arrayNo > 0; arrayNo--)
//    {
//        playerSkinLordHandle[arrayNo] = Addressables.LoadAssetAsync<GameObject>(skinNames[arrayNo]);
//        task[arrayNo - 1] = playerSkinLordHandle[arrayNo].Task;
//    }
//    await Task.WhenAll(task);
//    for (int arrayNo = numOfSkin - 1; arrayNo > 0; arrayNo--)
//    {
//        playerSkinPrefab[arrayNo] = playerSkinLordHandle[arrayNo].Result;
//        await Task.Yield();
//    }
//
//
//void ResourceLord()    //  Resourceフォルダ内のファイルを読み込む
//{
//    frameEffectPrefab[0] = Resources.Load<GameObject>("FirstRocketFrame");
//    frameEffectPrefab[1] = Resources.Load<GameObject>("SecondRocketFrame");
//    frameEffectPrefab[2] = Resources.Load<GameObject>("ThirdRocketFrame");
//    frameEffectPrefab[3] = Resources.Load<GameObject>("LastRocketFrame");
//    smokeEffectPrefab = Resources.Load<GameObject>("FrameSmoke");
//}



