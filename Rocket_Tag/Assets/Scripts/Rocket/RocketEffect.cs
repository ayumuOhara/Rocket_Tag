using System;                                                                                      ////  ロケットエフェクト生成・切り替え  ////
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static RocketEffect;

internal interface IEffectState                                                                     ////  以下State区  ////
{
    void Enter(RocketEffect arg);
    void Update(RocketEffect arg);
    void Exit(RocketEffect arg);
}
internal class FirstStage : IEffectState    //  ロケット1段階目
{
    public void Enter(RocketEffect rocketEffect)
    {
        //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcess.GENERATE_FRAMES);    //  1段階目のエフェクト生成
        rocketEffect.CallRocketEffectProcess(RocketEffect.RocketEffectProcess.GENERATE_PLUNK);
        Debug.Log("FirstStage");
    }
    public void Update(RocketEffect rocketEffect)
    {
        if (rocketEffect.TimeMgr.IsStageUpTime())
        {
            rocketEffect.ChangeState(new SecondStage());
        }
    }
    public void Exit(RocketEffect rocketEffect)
    {

    }
}
internal class SecondStage : IEffectState    //  ロケット2段階目
{
    public void Enter(RocketEffect rocketEffect)
    {
        //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcess.GENERATE_FRAMES);  //  2段階目のエフェクト生成
        rocketEffect.CallRocketEffectProcess(RocketEffect.RocketEffectProcess.GENERATE_PLUNK);
        Debug.Log("SecondStage");
    }
    public void Update(RocketEffect rocketEffect)
    {
        if (rocketEffect.TimeMgr.IsStageUpTime())
        {
            rocketEffect.ChangeState(new ThirdStage());
        }
    }
    public void Exit(RocketEffect rocketEffect)
    {

    }
}
internal class ThirdStage : IEffectState    //  ロケット3段階目
{
    public void Enter(RocketEffect rocketEffect)
    {
        //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcess.GENERATE_FRAMES);    //  3段階目のエフェクト生成
        rocketEffect.CallRocketEffectProcess(RocketEffect.RocketEffectProcess.GENERATE_PLUNK);
        Debug.Log("ThirdStage");
    }
    public void Update(RocketEffect rocketEffect)
    {
        if (rocketEffect.TimeMgr.IsStageUpTime())
        {
            rocketEffect.ChangeState(new LastStage());
        }
    }
    public void Exit(RocketEffect rocketEffect)
    {

    }
}
internal class LastStage : IEffectState    //  ロケット最終段階
{
    public void Enter(RocketEffect rocketEffect)
    {
        //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcess.GENERATE_FRAMES);    //  最終段階のエフェクト生成
        //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcess.GENERATE_SMOKE);    //  煙を取得
        rocketEffect.CallRocketEffectProcess(RocketEffect.RocketEffectProcess.GENERATE_PLUNK);
        Debug.Log("LastStage");
    }
    public void Update(RocketEffect rocketEffect)
    {
        //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcess.SMOKE_DIFFUSION);    //  煙を拡散
        rocketEffect.CallRocketEffectProcess(RocketEffectProcess.SMOKE_DIFFUSION);    //  煙を拡散
    }
    public void Exit(RocketEffect rocketEffect)
    {

    }
}
internal class PrepareRocket : IEffectState    //  次のロケットを用意している状態
{
    public void Enter(RocketEffect rocketEffect)
    {
        Debug.Log("Prepare");
    }
    public void Update(RocketEffect rocketEffect)
    {
        if(rocketEffect.IsFindNextRocket)
        {
            rocketEffect.ChangeState(new FirstStage());
        }
    }
    public void Exit(RocketEffect rocketEffect)
    {
        rocketEffect.IsFindNextRocket = false;
    }
}                                                                                                  ////  State区終了　　////
internal class RocketEffect : MonoBehaviour                                                        ////  ロケットエフェクト制御  ////
{
    internal enum RocketEffectProcess    //  ロケットエフェクトの処理一覧                          ////  以下宣言区  ////
    {
        GENERATE_PLUNK,
        SMOKE_DIFFUSION,
        SEARCH_ROCKET,
    }
    internal enum RocketEffectName    //  ロケットエフェクトの名前一覧
    {
        FIRST_ROCKET_FRAME,
        SECOND_ROCKET_FRAME,
        THIRD_ROCKET_FRAME,
        LAST_ROCKET_FRAME,
        FRAME_SMOKE
    }

    IEffectState currentState;
    IFactory factory;

    Dictionary<RocketEffectProcess, Action> rocketEffectProcess;
    static Dictionary<RocketEffectName, GameObject> loadedEffect;
    Dictionary<RocketEffectName, string> effectNameMap;
    GameObject frameEntity;
    GameObject smokeEntity;
    Transform rocket;
    ParticleSystem smokePS;
    ParticleSystem.MainModule smokeMainModule;
    ParticleSystem.ColorOverLifetimeModule smokeColorOverLifeTime;
    Gradient smokeGradient;
    TimeManager timeMgr;

    Vector3 frameEffectOffset;
    Vector3[] frameEffectScale;
    Vector3 smokeDiffusion;
    Vector3 smokeEffectScale;

    float smokeDelTime;
    int rocketStage;
    //bool didFalsed;    //  ロケット生成にタイミングを合わせるためのフラグ
    bool isInitialized;
    bool isFindNextRocket;
    const string rocketNotFound = "Error:Rocket Not Found";    //  msg for debug--------------
    const string rocketIsAssginedThis = "Rocket variable is assigned [this.transform]";    //  msg for debug--------------
    const string couldntGetTimemgr = "Error:Couldn't Get timeMgr";    //  msg for debug---------------------
    const string scriptProssesFinish = "RocketEffect.cs's process is stop";    //  msg for debug------------------
    
    internal Transform Rocket
    { set { rocket = value; } }
    internal TimeManager TimeMgr
    { get { return timeMgr; } }
    internal int _RocketStage
    { get { return rocketStage; } }
    internal bool IsFindNextRocket
    { get { return isFindNextRocket; } set { isFindNextRocket = value; } }                                                          ////  宣言区終了  ////


    void Start()
    {
        Initialize();    //  初期化
    }
    void Update()
    {
        if (isInitialized)    ////  エラー回避方法改善  ファイルを分ける時間がないためifでエラー回避
        {
            currentState.Update(this);
        }
    }                                                                                              ////  処理区終了  ////
    async void Initialize()
    {
        factory = new RealFactory();

        InitializeDictionary();
        await RocketEffectLoad();
        //rocket = GameObject.Find("Cylinder").GetComponent<Transform>();    //  ファーストステート突入した時のロケットが生成されてないことの無理やりの解消法でバックのため保存
        smokeGradient = new Gradient();
        smokeGradient.alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0f), new GradientAlphaKey(0.0f, 0.4f) };
        timeMgr = factory.CreateTimeMgr();
        //timeMgr = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        frameEffectOffset = new Vector3(-0.39f, -2.85f, 0.74f);
        frameEffectScale = new Vector3[] { new Vector3(1.21f, 1.21f, 1.21f), new Vector3(0.64f, 0.64f, 0.64f), new Vector3(0.56f, 0.61f, 0.5f), new Vector3(0.74f, 0.74f, 0.74f) };
        smokeDiffusion = new Vector3(1.02f, 1.02f, 1.02f);
        smokeEffectScale = new Vector3(1, 1, 1);

        smokeDelTime = 12;
        rocketStage = 0;
        isFindNextRocket = false;

        await WaitTillNullTF(rocket);
        ChangeState(new FirstStage());

        Debug.Log("Rocket is not null");
        isInitialized = true;
    }
    async Task RocketEffectLoad()    //  ロケットエフェクトのロード
    {
        Debug.Log("Rocket effect load start");
        List<Task> loadTasks;
        Dictionary<RocketEffectName, AsyncOperationHandle<GameObject>> loadHandle;

        const string loadFailMsg = "Load is failed";

        loadTasks = new List<Task>();
        loadHandle = new Dictionary<RocketEffectName, AsyncOperationHandle<GameObject>>
        {
            {RocketEffectName.FIRST_ROCKET_FRAME, default},
            {RocketEffectName.SECOND_ROCKET_FRAME, default},
            {RocketEffectName.THIRD_ROCKET_FRAME, default},
            {RocketEffectName.LAST_ROCKET_FRAME, default},
            {RocketEffectName.FRAME_SMOKE, default}
        };

        foreach (KeyValuePair<RocketEffectName, String> kvp in effectNameMap)
        {
            KeyValuePair<RocketEffectName, String> tmpKvp = kvp;
            loadHandle[tmpKvp.Key] = Addressables.LoadAssetAsync<GameObject>(tmpKvp.Value);
            loadTasks.Add(loadHandle[tmpKvp.Key].Task.ContinueWith(t =>
            {
                if (loadHandle[tmpKvp.Key].Status == AsyncOperationStatus.Succeeded)
                {
                    loadedEffect[tmpKvp.Key] = loadHandle[tmpKvp.Key].Result;
                }
                else
                {
                    Debug.LogWarning(loadFailMsg + tmpKvp.Key);    //  デバッグ用--------------------------------------
                }
            }));
        }
        await Task.WhenAll(loadTasks);
    }
    async Task WaitTillNullTF(Transform variable)    //  変数がnullの間ループするタスク
    {
        while (variable == null)
        {
            await Task.Yield();
        }
    }
    void InitializeDictionary()    //  辞書が多変数を初期化
    {
        rocketEffectProcess = new Dictionary<RocketEffectProcess, Action>
        {
            {RocketEffectProcess.GENERATE_PLUNK, GeneratePlume },
            {RocketEffectProcess.SMOKE_DIFFUSION, SmokeDiffusion},
        };
        loadedEffect = new Dictionary<RocketEffectName, GameObject>
        {
            {RocketEffectName.FIRST_ROCKET_FRAME, null},
            {RocketEffectName.SECOND_ROCKET_FRAME, null},
            {RocketEffectName.THIRD_ROCKET_FRAME, null},
            {RocketEffectName.LAST_ROCKET_FRAME, null},
            {RocketEffectName.FRAME_SMOKE, null},
        };
        effectNameMap = new Dictionary<RocketEffectName, string>
        {
            {RocketEffectName.FIRST_ROCKET_FRAME, "FirstRocketFrame" },
            {RocketEffectName.SECOND_ROCKET_FRAME, "SecondRocketFrame" },
            {RocketEffectName.THIRD_ROCKET_FRAME, "ThirdRocketFrame" },
            {RocketEffectName.LAST_ROCKET_FRAME, "LastRocketFrame" },
            {RocketEffectName.FRAME_SMOKE, "FrameSmoke" },
        };

    }
    internal void ChangeState(IEffectState newState)    //  状態遷移
    {
        if (rocket != null)
        {
            if (currentState != null)
            {
                currentState.Exit(this);
            }
            currentState = newState;
            currentState.Enter(this);
        }
    }
    internal void CallRocketEffectProcess(RocketEffectProcess process)    //  関数呼び出し
    {
        rocketEffectProcess[process]();
    }
    void GeneratePlume()    //  ロケットエフェクトを生成
    {
        frameEntity = Instantiate(loadedEffect[(RocketEffectName)rocketStage], rocket);
        if(rocketStage != 3)
        {
            rocketStage++;
            Debug.Log("rocekt stage increese");
        }
        else
        {
            rocketStage = 0;
            Debug.Log("rocekt stage is 0");
        }
        //rocketStage = rocketStage < 3 ? rocketStage++ : rocketStage = 0;      //  ロケットステージが毎回0状態になっているここまで----------------------------------エフェクトが重複生成されてもいた。
        frameEntity.transform.localPosition += frameEffectOffset;
        frameEntity.transform.localScale = Vector3.Scale(frameEntity.transform.localScale, frameEffectScale[rocketStage]);
        Debug.Log("rocketStage" + rocketStage);
        if (rocketStage == 3)
        {
            smokeEntity = Instantiate(loadedEffect[RocketEffectName.FRAME_SMOKE]);
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
//GameObject[] frameEffectPrefab;
//GameObject frameEffectEntity;
//GameObject smokeEffectPrefab;
//GameObject smokeEntity;
// public transform rocket;
//internal enum EffectNo    //  エフェクトの種類
//{
//    FRAME,
//    SMOKE,
//}
//void OnEnable()                                                                                ////  以下処理区  ////
//{
//    /*  for debug---------------------------------------  */
//    ///*  処理順を合わせるため最初にSetActive(false)にする  */
//    //SetSetActive(didFalsed, this.gameObject);
//    //if (didFalsed)
//    //{
//    //    Initialize();    //  初期化
//    //}

//    Initialize();
//}
//void OnDisable()
//{
//    didFalsed = true;
//}
//void Start()
//{
/*  for debug--------------------------------  */
//Debug.Log("Start entire");
//while (!effectLoadTask.IsCompleted)
//{

//}
//}
/*  for debug------------------------------------------*/
//Debug.Log(isloaded);
//Debug.Log(frameEffectPrefab[3].name);
//Debug.Log(rocketStage);    //  for debug-----------------------------------
//void SetSetActive(bool flag, GameObject obj)    //  SetActiveを設定する                        ////  以下関数区  ////
//{
//    if (flag != obj.activeSelf)
//    {
//        obj.SetActive(flag);
//    }
//    else
//    {
//        obj.SetActive(false);
//    }
//}
/*  for debug-----------------------  */
//effectLoadTask = RocketEffectLoad();
//await effectLoadTask;
//void RocketEffectWrapper(RocketEffectProcess RocketEffectProcesss)   // ロケットエフェクトのラッパー関数
//{
//    switch (RocketEffectProcesss)
//    {
//        case RocketEffectProcess.GENERATE_FRAMES:
//            {
//                GenerateEffect((int)EffectNo.FRAME, frameEffectPrefab[rocketStage], rocket, frameEffectOffset, frameEffectScale[rocketStage]);
//                rocketStage = rocketStage != 3 ? ++rocketStage : 0;
//                break;
//            }
//        case RocketEffectProcess.GENERATE_SMOKE:
//            {
//                GenerateEffect((int)EffectNo.SMOKE, smokeEffectPrefab, rocket, frameEffectOffset, smokeEffectScale);    //  offsetにframeEffectOffsetを使用
//                smokePS = smokeEntity.GetComponent<ParticleSystem>();
//                smokeMainModule = smokePS.main;
//                smokeMainModule.startColor = Color.white;
//                smokeColorOverLifeTime = smokePS.colorOverLifetime;
//                break;
//            }
//        case RocketEffectProcess.SEARCH_ROCKET:
//            {
//                Debug.Log("SERACH_ROCKET entered");
//                rocket = null;
//                rocket = GameObject.Find("Rocket").GetComponent<Transform>();
//                if (IsNull_Variable(rocket, false, rocketNotFound))    //  msg for debug-------------------
//                {
//                    Debug.Log(rocketIsAssginedThis);    //  msg for debug--------------------
//                    rocket = this.transform;
//                }
//                if (currentState is PrepareRocket)
//                {
//                    ChangeState(new FirstStage());
//                }
//                Debug.Log("SERACH_ROCKET exsited" + rocket);
//                break;
//            }
//        case RocketEffectProcess.SMOKE_DIFFUSION:
//            {
//                SmokeDiffusion();
//                break;
//            }
//        default: break;
//    }
////}
///// void GenerateEffecct(int effectNo, GameObject effect, Transform parent, Vector3 offset, Vector3 scale)    //  エフェクト生成
//{
//    switch (effectNo)
//    {
//        case 0:
//            {
//                Vector3 fixScale;

//                fixScale = new Vector3(1 / parent.localScale.x, 1 / parent.localScale.y, 1 / parent.localScale.z);

//                if (!IsNull_Variable(frameEffectEntity, false, ""))
//                {
//                    Destroy(frameEffectEntity);
//                }
//                frameEffectEntity = Instantiate(effect, parent);
//                frameEffectEntity.transform.localPosition += offset;
//                frameEffectEntity.transform.localScale = Vector3.Scale(frameEffectEntity.transform.localScale, fixScale);
//                break;
//            }
//        case 1:
//            {
//                smokeEntity = Instantiate(smokeEffectPrefab);
//                smokeEntity.transform.position = rocket.position;
//                break;
//            }
//        default: break;
//    }
//}
//bool IsNull_Array<T>     //  配列のヌルチェック、危険性があった場合強制クラッシュ
//   (T[] value, bool isCheckPoint, int[] checkPoint, bool haveToClach, string errorMsg_PointNull, string errorMsg_AllNull)
//{
//    if (value == null || value.Length == 0)
//    {
//        if (haveToClach)
//        {
//            Environment.FailFast(errorMsg_AllNull);    //  クラッシュ
//        }
//        Debug.Log(errorMsg_AllNull);    //  debug------------------
//        return true;
//    }
//    if (isCheckPoint)
//    {
//        for (int arrayNo = checkPoint.Length - 1; arrayNo >= 0; arrayNo--)
//        {
//            if (value[checkPoint[arrayNo]] == null)
//            {
//                if (haveToClach)
//                {
//                    Environment.FailFast(errorMsg_PointNull);    //  クラッシュ
//                }
//                return true;
//            }
//            Debug.Log(errorMsg_PointNull);    //  debug-------------------
//            return false;
//        }
//        Debug.Log(errorMsg_PointNull);    //  debug--------------------------
//    }
//    return false;
//}