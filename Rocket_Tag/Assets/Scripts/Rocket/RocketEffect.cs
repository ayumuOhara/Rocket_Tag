using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using static CamAim;
                                                                                                   ////  ロケットエフェクト生成・切り替え  ////
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
    public void Exit(RocketEffect rocketEffeet)
    {

    }
}
internal class LastStage : EffectState    //  ロケット最終段階
{
    public void Enter(RocketEffect rocketEffect)
    {
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_FRAMES);    //  最終段階のエフェクト生成
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.SEARCH_FRAME_SMOKE);    //  煙を取得
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
        SEARCH_FRAME_SMOKE,
        SMOKE_DIFFUSION,
        SEARCH_ROCKET,
    }
    internal enum EffectNo    //  エフェクトの種類
    {
        Frame,
    }

    EffectState currentState;

    GameObject[] frameEffectPrefab;
    GameObject frameEffectEntity;
    Transform rocket;
    Transform smokeEntity;
    TimeManager timeMgr;

    Vector3 frameEffectOffset;
    Vector3 smokeDiffusion;
    
    float smokeDelTime;
    int rocketStage;

    internal TimeManager _TimeMgr
    {  get { return timeMgr; } }
    internal int _RocketStage
    {  get { return rocketStage; } }                                                               ////  宣言区終了  ////

    void Start()                                                                                   ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }
    void Update()
    {
        currentState.Update(this);
    }                                                                                              ////  処理区終了  ////
    void Initialize()    //  初期化                                                                ////  以下関数区  ////
    {
        frameEffectPrefab = new GameObject[4];
        ResourceLord();
        rocket = GameObject.Find("Rocket").GetComponent<Transform>();
        timeMgr = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        frameEffectOffset = new Vector3(0, -0.8f, 0);
        smokeDiffusion = new Vector3(1.02f, 1.02f, 1.02f);

        rocketStage = 0;
        smokeDelTime = 7;

        ChangeState(new FirstStage());

    }
    internal void ChangeState(EffectState newState)    //  状態遷移
    {
        if(currentState != null)
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
                    GenerateEffect((int)EffectNo.Frame, frameEffectPrefab[rocketStage++], rocket, frameEffectOffset);//    TEST------------------------
                    if (rocketStage == 4)
                    {
                        rocketStage = 0;
                    }
                    break;
                }
            case RocketEffectProcces.SEARCH_FRAME_SMOKE:
                {
                    smokeEntity = GameObject.Find("FrameSmoke").GetComponent<Transform>();
                    break;
                }
            case RocketEffectProcces.SEARCH_ROCKET:
                {
                    rocket = GameObject.Find("Rocket").GetComponent<Transform>();
                    if (rocket != null && currentState == new PrepareRocket())
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
    void GenerateEffect(int effectNo ,GameObject effect, Transform parent, Vector3 offset)    //  エフェクト生成
    {
        switch (effectNo)
        {
            case 0:
                {
                    if(frameEffectEntity != null)
                    {
                        Destroy(frameEffectEntity);
                    }
                    frameEffectEntity = Instantiate(effect, parent);
                    frameEffectEntity.transform.localPosition += offset;
                    break;
                }
            default: break;
        }
    }
    void SmokeDiffusion()    //  煙幕拡散、煙幕をデストロイしたたらPrepareRocketStateに移動
    {
        if ((smokeDelTime -= Time.deltaTime) > 0)
        {
            Debug.Log(333);
            smokeEntity.transform.localScale = Vector3.Scale(smokeEntity.transform.localScale, smokeDiffusion);
        }
        else
        {
            Destroy(smokeEntity.gameObject);
            ChangeState(new PrepareRocket());
        }
    }
    void ResourceLord()    //  Resourceフォルダ内のファイルを読み込む
    {
        frameEffectPrefab[0] = Resources.Load<GameObject>("FirstRocketFrame");
        frameEffectPrefab[1] = Resources.Load<GameObject>("SecondRocketFrame");
        frameEffectPrefab[2] = Resources.Load<GameObject>("ThirdRocketFrame");
        frameEffectPrefab[3] = Resources.Load<GameObject>("LastRocketFrame");
    }
}                                                                                                   ////  関数区終了  ////