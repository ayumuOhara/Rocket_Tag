using Unity.VisualScripting;
using UnityEngine;
////  ロケットエフェクト生成・切り替え  ////
internal interface EffectState        ////////----以下state区----////////
{
    void Enter(RocketEffect arg);
    void Update(RocketEffect arg);
    void Exit(RocketEffect arg);
}
internal class FirstStage : EffectState   //  ロケット1段階目
{
    public void Enter(RocketEffect rocketEffect)
    {
        rocketEffect.GenerateFrameEffect();
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
        rocketEffect.GenerateFrameEffect();
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
        rocketEffect.GenerateFrameEffect();
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
    public void Enter(RocketEffect rocketEffeet)
    {
        rocketEffeet.GenerateFrameEffect();
    }
    public void Update(RocketEffect rocketEffeet)
    {
        rocketEffeet.SmokeDiffusion();
    }
    public void Exit(RocketEffect rocketEffeet)
    {

    }
}
internal class NullStage : EffectState    //  何もしないState
{
    public void Enter(RocketEffect rocketEffeet)
    {

    }
    public void Update(RocketEffect rocketEffeet)
    {

    }
    public void Exit(RocketEffect rocketEffeet)
    {

    }
}
public class RocketEffect : MonoBehaviour
{
    enum EffectNo    //  エフェクトの種類                                                 ////　以下宣言区  ////
    {
        Frame,
    }

    EffectState currentState;

    GameObject[] frameEffectPrefab;
    GameObject frameEffectEntity;
    GameObject smokeEntity;
    TimeManager timeMgr;

    Vector3 frameEffectOffset;
    Vector3 smokeDiffusion;
    
    float smokeDelTime;
    int rocketStage;

    internal TimeManager _TimeMgr
    {  get { return _TimeMgr; } }
    internal int _RocketStage
    {  get { return rocketStage; } }

    void Start()
    {
        Initialize();    //  初期化
    }
    void Update()
    {
        currentState.Update(this);
    }
    void Initialize()    //  初期化    ////////----以下関数区----/////////     ここまで---------------------------------------------------------------------
    {
        frameEffectPrefab = new GameObject[4];
        ResourceLord();
        timeMgr = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        frameEffectOffset = new Vector3(0, -0.8f, 0);
        smokeDiffusion = new Vector3(1.005f, 1.005f, 1.005f);

        rocketStage = 0;
        smokeDelTime = 4;

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
    void GenerateEffect(int effectNo ,GameObject effect, Transform parent, Vector3 offset)    //  エフェクト生成
    {
        switch (effectNo)
        {
            case 0:
                {
                    frameEffectEntity = Instantiate(effect, parent);
                    frameEffectEntity.transform.localPosition = offset;
                    break;
                }
            default: break;
        }
    }
    internal void GenerateFrameEffect()    //  ロケットの炎エフェクト生成
    {
        GenerateEffect((int)EffectNo.Frame, frameEffectPrefab[rocketStage++], this.transform, frameEffectOffset);
    }
    internal void SmokeDiffusion()    //  煙幕拡散、煙幕をデストロイしたたらNullStateに移動
    {
        if ((smokeDelTime -= Time.deltaTime) > 0)
        {
            smokeEntity.transform.localScale = Vector3.Scale(smokeEntity.transform.localScale, smokeDiffusion);
        }
        else
        {
            Destroy(smokeEntity);
            ChangeState(new NullStage());
        }
    }
    void ResourceLord()    //  Resourceフォルダ内のファイルを読み込む
    {
        frameEffectPrefab[0] = Resources.Load<GameObject>("FirstRocketFrame");
        frameEffectPrefab[1] = Resources.Load<GameObject>("SecondRocketFrame");
        frameEffectPrefab[2] = Resources.Load<GameObject>("ThirdRocketFrame");
        frameEffectPrefab[3] = Resources.Load<GameObject>("LastRocketFrame");
    }
}