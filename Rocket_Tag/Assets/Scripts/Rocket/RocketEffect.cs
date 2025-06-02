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
        rocketEffect._RocketStage++;
    }
}
internal class SecondStage : IEffectState    //  ロケット2段階目
{
    public void Enter(RocketEffect rocketEffect)
    {
        rocketEffect.CallRocketEffectProcess(RocketEffect.RocketEffectProcess.GENERATE_PLUNK);
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
        rocketEffect._RocketStage++;
    }
}
internal class ThirdStage : IEffectState    //  ロケット3段階目
{
    public void Enter(RocketEffect rocketEffect)
    {
        rocketEffect.CallRocketEffectProcess(RocketEffect.RocketEffectProcess.GENERATE_PLUNK);
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
        rocketEffect._RocketStage++;
    }
}
internal class LastStage : IEffectState    //  ロケット最終段階
{
    public void Enter(RocketEffect rocketEffect)
    {
        rocketEffect.CallRocketEffectProcess(RocketEffect.RocketEffectProcess.GENERATE_PLUNK);
    }
    public void Update(RocketEffect rocketEffect)
    {
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
        rocketEffect.IsFindNextRocket = false;
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
    Vector3 smokeEffectEndScale;

    float smokeDelTime;
    int rocketStage;
    bool isInitialized;
    bool isFindNextRocket;
    
    internal Transform Rocket
    { set { rocket = value; } }    //  死んだあとエフェクト出ない問題
    internal TimeManager TimeMgr
    { get { return timeMgr; } }
    internal int _RocketStage
    { get { return rocketStage; } set { rocketStage = value; } }
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
        smokeGradient = new Gradient();
        smokeGradient.alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0f), new GradientAlphaKey(0.0f, 0.3f) };
        timeMgr = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        frameEffectOffset = new Vector3(0.020708f, -2.74f, -0.36f);
        frameEffectScale = new Vector3[] { new Vector3(1.21f, 1.21f, 1.21f), new Vector3(0.64f, 0.64f, 0.64f), new Vector3(0.56f, 0.61f, 0.5f), new Vector3(0.74f, 0.74f, 0.74f) };
        smokeDiffusion = new Vector3(1.011f, 1.011f, 1.011f);
        smokeEffectScale = new Vector3(1, 1, 1);
        smokeEffectEndScale = new Vector3(27f, 27f, 27f);

        smokeDelTime = 2.5f;
        rocketStage = 0;
        isFindNextRocket = false;

        await WaitTillNullTF(rocket);
        ChangeState(new FirstStage());

        isInitialized = true;
    }
    async Task RocketEffectLoad()    //  ロケットエフェクトのロード
    {
        List<Task> loadTasks;
        Dictionary<RocketEffectName, AsyncOperationHandle<GameObject>> loadHandle;

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

                }
            }));
        }
        await Task.WhenAll(loadTasks);
    }
    async Task WaitTillNullTF(Transform variable)    //  変数がnullの間ループするタスク
    {
        while (rocket == null)
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
        frameEntity.transform.localPosition = frameEffectOffset;
        if (rocketStage == 3)
        {
            if(smokeEntity == null)
            {
                smokeEntity = Instantiate(loadedEffect[RocketEffectName.FRAME_SMOKE]);
                smokeEntity.transform.position = rocket.position;
            }
            else
            {
                smokeEntity.transform.position = rocket.position;
                smokePS.Play();
            }
            smokePS = smokeEntity.GetComponent<ParticleSystem>();
            smokeMainModule = smokePS.main;       ////--------------------------------------二回目以降スモークが見えないない、unity最新使用の性か、生成場所が悪いか
            smokeMainModule.startColor = Color.white;
            smokeColorOverLifeTime = smokePS.colorOverLifetime;
            rocketStage = 0;
        }
    }
    void SmokeDiffusion()    //  煙幕拡散、煙幕をデストロイしたたらPrepareRocketStateに移動
    {
        float smokeDiffuseSpd = 0.5f;
        if ((smokeDelTime -= Time.deltaTime) > 0)
        {
            smokeColorOverLifeTime.color = smokeGradient;
            smokeEntity.transform.localScale = Vector3.Lerp(smokeEntity.transform.localScale, smokeEffectEndScale, smokeDiffuseSpd * Time.deltaTime);
        }
        else
        {
            smokeDelTime = 2.5f;
            smokePS.Stop();
            smokePS.Clear();
            smokeEntity.transform.localScale = Vector3.Lerp(smokeEntity.transform.localScale, smokeEffectEndScale, smokeDiffuseSpd * Time.deltaTime);
            ChangeState(new PrepareRocket());
            smokeEntity.transform.localScale = smokeEffectScale;

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
            return true;
        }
        return false;
    }
}