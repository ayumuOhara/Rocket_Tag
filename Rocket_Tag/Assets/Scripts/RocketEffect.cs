using Unity.VisualScripting;
using UnityEngine;

public class RocketEffect : MonoBehaviour
{
    enum EffectNo    //  エフェクトの種類
    {
        Frame
    }

    int rocketStage;
    float smokeDelTime;

    GameObject frameEffect;
    GameObject smoke;
    GameObject[] frameEffectPrefab;
    internal TimeManager timeMgr;

    Vector3 frameEffectOffset;
    Vector3 smokeDiffusion;

    IState currentState;
    void Start()
    {
        Initialize();
    }
    void Update()
    {
        currentState.Update(this);
    }
    void Initialize()    //  初期化    ////////----以下関数区----/////////
    {
        rocketStage = 0;
        smokeDelTime = 4;

        frameEffectPrefab[0] = Resources.Load<GameObject>("FirstRocketFrame");
        frameEffectPrefab[1] = Resources.Load<GameObject>("SecondRocketFrame");
        frameEffectPrefab[2] = Resources.Load<GameObject>("ThirdRocketFrame");
        frameEffectPrefab[3] = Resources.Load<GameObject>("LastRocketFrame");
        timeMgr = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        frameEffectOffset = new Vector3(0, -0.8f, 0);
        smokeDiffusion = new Vector3(1.005f, 1.005f, 1.005f);

        ChangeState(new FirstStage());
    }
    internal void ChangeState(IState newState)    //  状態変更
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
                    frameEffect = Instantiate(effect, parent);
                    frameEffect.transform.localPosition = offset;
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
            smoke.transform.localScale = Vector3.Scale(smoke.transform.localScale, smokeDiffusion);
        }
        else
        {
            Destroy(smoke);
            ChangeState(new NullStage());
        }
    }
    internal int GetRocketStage()
    {
        return rocketStage;
    }
}
internal interface IState        ////////----以下state区----////////
{
    void Enter(RocketEffect arg);
    void Update(RocketEffect arg);
    void Exit(RocketEffect arg);
}
internal class FirstStage : IState    //  ロケット1段階目
{
    public void Enter(RocketEffect rocketEffeet)
    {
        rocketEffeet.GenerateFrameEffect();
    }
    public void Update(RocketEffect rocketEffeet)
    {
        if (rocketEffeet.timeMgr.IsStageUpTime())
        {
            rocketEffeet.ChangeState(new SecondStage());
        }
    }
    public void Exit(RocketEffect rocketEffeet)
    {

    }
}
internal class SecondStage : IState    //  ロケット2段階目
{
    public void Enter(RocketEffect rocketEffeet)
    {
        rocketEffeet.GenerateFrameEffect();
    }
    public void Update(RocketEffect rocketEffeet)
    {
        if (rocketEffeet.timeMgr.IsStageUpTime())
        {
            rocketEffeet.ChangeState(new ThirdStage());
        }
    }
    public void Exit(RocketEffect rocketEffeet)
    {

    }
}
internal class ThirdStage : IState    //  ロケット3段階目
{
    public void Enter(RocketEffect rocketEffeet)
    {
        rocketEffeet.GenerateFrameEffect();
    }
    public void Update(RocketEffect rocketEffeet)
    {
        if (rocketEffeet.timeMgr.IsStageUpTime())
        {
            rocketEffeet.ChangeState(new LastStage());
        }
    }
    public void Exit(RocketEffect rocketEffeet)
    {

    }
}
internal class LastStage : IState    //  ロケット最終段階
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
internal class NullStage : IState    //  何もしないState
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