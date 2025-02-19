using Unity.VisualScripting;
using UnityEngine;
////  ���P�b�g�G�t�F�N�g�����E�؂�ւ�  ////
internal interface EffectState        ////////----�ȉ�state��----////////
{
    void Enter(RocketEffect arg);
    void Update(RocketEffect arg);
    void Exit(RocketEffect arg);
}
internal class FirstStage : EffectState   //  ���P�b�g1�i�K��
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
internal class SecondStage : EffectState    //  ���P�b�g2�i�K��
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
internal class ThirdStage : EffectState    //  ���P�b�g3�i�K��
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
internal class LastStage : EffectState    //  ���P�b�g�ŏI�i�K
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
internal class NullStage : EffectState    //  �������Ȃ�State
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
    enum EffectNo    //  �G�t�F�N�g�̎��                                                 ////�@�ȉ��錾��  ////
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
        Initialize();    //  ������
    }
    void Update()
    {
        currentState.Update(this);
    }
    void Initialize()    //  ������    ////////----�ȉ��֐���----/////////     �����܂�---------------------------------------------------------------------
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
    internal void ChangeState(EffectState newState)    //  ��ԑJ��
    {
        if(currentState != null)
        {
            currentState.Exit(this);
        }
        currentState = newState;
        currentState.Enter(this);
    }
    void GenerateEffect(int effectNo ,GameObject effect, Transform parent, Vector3 offset)    //  �G�t�F�N�g����
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
    internal void GenerateFrameEffect()    //  ���P�b�g�̉��G�t�F�N�g����
    {
        GenerateEffect((int)EffectNo.Frame, frameEffectPrefab[rocketStage++], this.transform, frameEffectOffset);
    }
    internal void SmokeDiffusion()    //  �����g�U�A�������f�X�g���C��������NullState�Ɉړ�
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
    void ResourceLord()    //  Resource�t�H���_���̃t�@�C����ǂݍ���
    {
        frameEffectPrefab[0] = Resources.Load<GameObject>("FirstRocketFrame");
        frameEffectPrefab[1] = Resources.Load<GameObject>("SecondRocketFrame");
        frameEffectPrefab[2] = Resources.Load<GameObject>("ThirdRocketFrame");
        frameEffectPrefab[3] = Resources.Load<GameObject>("LastRocketFrame");
    }
}