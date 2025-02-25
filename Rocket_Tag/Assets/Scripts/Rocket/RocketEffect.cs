using System.Collections;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
                                                                                                   ////  ���P�b�g�G�t�F�N�g�����E�؂�ւ�  ////
internal interface EffectState                                                                     ////  �ȉ�State��  ////
{
    void Enter(RocketEffect arg);
    void Update(RocketEffect arg);
    void Exit(RocketEffect arg);
}
internal class FirstStage : EffectState   //  ���P�b�g1�i�K��
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
internal class SecondStage : EffectState    //  ���P�b�g2�i�K��
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
internal class ThirdStage : EffectState    //  ���P�b�g3�i�K��
{
    public void Enter(RocketEffect rocketEffect)
    {
        Debug.Log(2);
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_FRAMES);    //  �ŏI�i�K�̃G�t�F�N�g����
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
internal class LastStage : EffectState    //  ���P�b�g�ŏI�i�K
{
    public void Enter(RocketEffect rocketEffect)
    {
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_FRAMES);    //  �ŏI�i�K�̃G�t�F�N�g����
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.GENERATE_SMOKE);    //  �����擾
    }
    public void Update(RocketEffect rocketEffect)
    {
        rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.SMOKE_DIFFUSION);    //  �����擾
    }
    public void Exit(RocketEffect rocketEffect)
    {

    }
}
internal class PrepareRocket : EffectState    //  ���̃��P�b�g��p�ӂ��Ă�����
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
}                                                                                                  ////  State��I���@�@////
internal class RocketEffect : MonoBehaviour
{
    internal enum RocketEffectProcces    //  ���P�b�g�G�t�F�N�g�̏����ꗗ                          ////  �ȉ��錾��  ////
    {
        GENERATE_FRAMES,
        GENERATE_SMOKE,
        SEARCH_FRAME_SMOKE,
        SMOKE_DIFFUSION,
        SEARCH_ROCKET,
    }
    internal enum EffectNo    //  �G�t�F�N�g�̎��
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
    bool didFalsed;    //  ���P�b�g�����Ƀ^�C�~���O�����킹�邽�߂̃t���O

    internal TimeManager _TimeMgr
    {  get { return timeMgr; } }
    internal int _RocketStage
    {  get { return rocketStage; } }                                                               ////  �錾��I��  ////
    internal bool _DidFalsed
    { get {  return didFalsed; } }

    void OnEnable()                                                                                ////  �ȉ�������  ////
    {
        SetSetActive(didFalsed, this.gameObject);
        if(didFalsed)
        {
            Initialize();    //  ������
        }
    }
    void OnDisable()
    {
        didFalsed = true;
    }
    void Start()                                                                                  
    {

    }
    void Update()
    {
        currentState.Update(this);
    }                                                                                              ////  ������I��  ////
    void SetSetActive(bool flag, GameObject obj)    //  SetActive��ݒ肷��
    {
        switch (flag != obj.activeSelf)
        {
            case true:
                {
                    obj.SetActive(flag);
                    break;
                }
            case false:
                {
                    obj.SetActive(false);
                    break;
                }
        }
        obj.SetActive(flag);
    }
    void Initialize()    //  ������                                                                ////  �ȉ��֐���  ////
    {
        frameEffectPrefab = new GameObject[4];
        ResourceLord();
        rocket = GameObject.Find("Rocket").GetComponent<Transform>();
        smokeGradient = new Gradient();
        smokeGradient.alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0f), new GradientAlphaKey(0.0f, 0.4f) };
        timeMgr = GameObject.Find("TimeManager").GetComponent<TimeManager>();

        frameEffectOffset = new Vector3(0, -0.6f, 0.5f);
        smokeDiffusion = new Vector3(1.02f, 1.02f, 1.02f);

        rocketStage = 0;
        smokeDelTime = 12;

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
    internal void RocketEffectWrapper(RocketEffectProcces rocketEffectProcces)   // ���P�b�g�G�t�F�N�g�̃��b�p�[�֐�
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
                    rocket = GameObject.Find("Rocket").GetComponent<Transform>();
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
    void GenerateEffect(int effectNo ,GameObject effect, Transform parent, Vector3 offset)    //  �G�t�F�N�g����
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
            case 1:
                {
                    smokeEntity = Instantiate(smokeEffectPrefab);
                    smokeEntity.transform.position = rocket.position;
                    break;
                }
            default: break;
        }
    }
    void SmokeDiffusion()    //  �����g�U�A�������f�X�g���C��������PrepareRocketState�Ɉړ�
    {
        if ((smokeDelTime -= Time.deltaTime) > 0)
        {
            smokeColorOverLifeTime.color = smokeGradient;
            smokeEntity.transform.localScale = Vector3.Scale(smokeEntity.transform.localScale, smokeDiffusion);
        }
        else
        {
            Debug.Log("TimeOut");
            Destroy(smokeEntity.gameObject);
            ChangeState(new PrepareRocket());
        }
    }
    void ResourceLord()    //  Resource�t�H���_���̃t�@�C����ǂݍ���
    {
        frameEffectPrefab[0] = Resources.Load<GameObject>("FirstRocketFrame");
        frameEffectPrefab[1] = Resources.Load<GameObject>("SecondRocketFrame");
        frameEffectPrefab[2] = Resources.Load<GameObject>("ThirdRocketFrame");
        frameEffectPrefab[3] = Resources.Load<GameObject>("LastRocketFrame");
        smokeEffectPrefab = Resources.Load<GameObject>("FrameSmoke");
    }
}                                                                                                   ////  �֐���I��  ////