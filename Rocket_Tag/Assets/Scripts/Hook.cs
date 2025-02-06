using UnityEngine;
using System.Threading.Tasks;
using System;
using Unity.VisualScripting;
using Unity.Properties;
using UnityEngine.InputSystem.DualShock;

public class Hook : MonoBehaviour    //  �t�b�N�X�N���v�g
{
    internal enum HookProcess    //  �t�b�N�̋����ꗗ
    {
        THROW_HOOK,
        ATTRACT_PLAYER,
    }

    GameObject hookPrefab;
    GameObject hookEntity;
    GameObject chainPrefab;
    GameObject chainEntity;
    GameObject player;
    GameObject hitObj;
    Camera playerCamera;
    PlayerMovement playerMovement;
    
    Vector3 hookGeneratePos;
    Vector3 hookUnlockPos;

    float throwSpeed;
    float attractSpeed;
    float attractAcceleration;
    float attractingPlayerMoveSpd;
    float hookOffset_Z;
    float hitStopTime;

    internal GameObject HitObj
    {
        get { return hitObj; }
    }

    HookState currentState;
    void Start()
    {
        Initialize();
    }
    void Update()
    {
        currentState.Update(this);
    }
    void Initialize()    //  ������
    {
        hookPrefab = Resources.Load<GameObject>("Hook");
        player = this.gameObject;    //  �d�l�ɂ���Ē����K
        playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        hitObj = null;

        hookUnlockPos = new Vector3(2.5f, 2.5f, 2.5f);

        throwSpeed = 30f;
        attractSpeed = 2f;
        attractAcceleration = 1.1f;
        hookOffset_Z = 2f;
        hookGeneratePos = player.transform.position;
        hookGeneratePos.z += hookOffset_Z;
        hitStopTime = 0.5f;

        ChangeState(new NoAct());
    }
    internal void ChangeState(HookState newState)    //  ��ԑJ��
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }
        currentState = newState;
        currentState.Enter(this);
    }
    void GenerateObj(ref GameObject entity, GameObject generatObj, Vector3 generatePos)    //  �I�u�W�F�N�g��position���w�肵�Đ��� 
    {
        entity = GameObject.Instantiate(generatObj);
        entity.transform.position = generatePos;
    }
    void StraightMoveToPos(Transform moved, Vector3 target, float moveSpeed)    //  ���W�Ɍ������Ē����ړ�
    {
        moved.position = Vector3.MoveTowards(moved.transform.position, target, moveSpeed * Time.deltaTime);
    }
    internal void HookWrapper(HookProcess HookProcess)   //  �t�b�N�̋����ɉ����������̃��b�p�[�֐�
    {
        switch (HookProcess)
        {
            case HookProcess.THROW_HOOK:
                {
                    GenerateObj(ref hookEntity, hookPrefab, hookGeneratePos);
                    ThrowHook();
                    break;
                }
            case HookProcess.ATTRACT_PLAYER:
                {
                    AttractPlayer();
                    break;
                }
            default: break;
        }
    }
    async void ThrowHook()    //  �ŏ��ɓ��������I�u�W�F�N�g�̃^�O��������
    {
        Collider[] tempCollider = new Collider[1];

        Vector3 tempScreenCenter = GetVecForScreenCenter(player.transform.position);

        float throwAcceleration = 7f;
        float retrieveTime = 10f;
        float topPoint = 1.5f;
        float belowPoint = 1.5f;
        float radius = 1.5f;

        while ((tempCollider = GenerateHitDetection(hookEntity.transform, belowPoint, topPoint, radius)) == null || tempCollider.Length  == 0 && (retrieveTime -= Time.deltaTime) > 0)
        {
            if (tempCollider.Length > 0 && tempCollider != null && tempCollider[0].tag == "Player")
                Debug.Log(tempCollider[0].tag);
            StraightMoveToPos(hookEntity.transform, tempScreenCenter, (throwSpeed + (throwAcceleration *= 1.1f)) * Time.deltaTime);
            await Task.Yield();
        }
        if (retrieveTime < 0)
        {
            hitObj = null;
        }
        else
        {
            hitObj = tempCollider[0].gameObject;
        }
    }
    Vector3 GetVecForScreenCenter(Vector3 perspective)    //  prespective����J�������S�ւ̃��F�N�g�����Ƃ�
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 100);
        return playerCamera.ScreenToWorldPoint(screenCenter) - perspective;
    }
    Collider[] GenerateHitDetection(Transform AppendObj, float belowPoint, float topPoint, float radius)
    {
        return Physics.OverlapCapsule(AppendObj.transform.position - Vector3.down * belowPoint, AppendObj.transform.position + Vector3.up * topPoint, radius);
    }
    async void AttractPlayer ()    //  ������  
    {
      //  playerMovement = hitObj.GetComponent<PlayerMovement>();

      //  float tempPlayerSpeed = playerMovement.GetMoveSpeed();

      //  playerMovement.SetMoveSpeed(0.15f);

        while((hitStopTime -= Time.deltaTime) > 0)
        {
            Debug.Log("�q�b�g�X�g�b�v�Ȃ�");
            await Task.Yield();
        }
        while(!IsNear(GetPosDif(player.transform.position, hookEntity.transform.position), hookUnlockPos))
        {
            SetPos(hookEntity.transform.position, hitObj.transform, Vector3.zero);
            attractSpeed *= attractAcceleration;
            StraightMoveToPos(hookEntity.transform, player.transform.position, attractSpeed);
            await Task.Yield();
        }
        //    playerMovement.SetMoveSpeed(tempPlayerSpeed);
        Destroy(hookEntity);
    }
    Vector3 GetPosDif(Vector3 point1, Vector3 point2)    //  ���W�Ԃ̋������擾
    {
        Vector3 posDif = point1 - point2;
        if(posDif.x < 0)
        {
            posDif *= -1;
        }
        if(posDif.y < 0)
        {
            posDif.y *= -1;
        }
        if(posDif.z < 0)
        {
            posDif.z *= -1;
        }
        return posDif;
    }
    bool IsNear(Vector3 posDif, Vector3 judgePos)    //  �I�u�W�F�N�g�����苗�������ǂ���
    {
        return posDif.x < judgePos.x && posDif.y < judgePos.y && posDif.z < judgePos.z;
        Debug.Log(posDif);
    }
    void SetPos(Vector3 point, Transform moved, Vector3 offset)    //  �I�u�W�F�N�g���w����W�ɏu�Ԉړ�������
    {
        moved.position = point + offset;
    }
}
internal interface HookState    //  �t�b�N��ԃC���^�[�t�F�[�X
{
    void Enter(Hook arg);
    void Update(Hook arg);
    void Exit(Hook arg);
}

internal class NoAct : HookState    //  �t�b�N�n���̃A�N�V���������Ă��Ȃ����
{
    public void Enter(Hook hook)
    {

    }
    public void Update(Hook hook)
    {
        if (Input.GetKey(KeyCode.H))
        {
            hook.ChangeState(new HookThrow());
        }
    }
    public void Exit(Hook hook)
    {

    }
}
internal class HookThrow : HookState    //  �t�b�N�������
{
    public void Enter(Hook hook)
    {
        hook.HookWrapper(Hook.HookProcess.THROW_HOOK);
    }
    public void Update(Hook hook)
    {
        if(hook.HitObj != null)
        {
            switch (hook.HitObj.tag)
            {
                case "Player":
                    {
                        Debug.Log("plyaerrrr");
                        hook.ChangeState(new HitPlayer());
                        break;
                    }
                default:
                    {
                        hook.ChangeState(new NotHitPlayer());
                        break;
                    }
            }
        }
    }
    public void Exit(Hook hook)
    {

    }
}
internal class HitPlayer : HookState    //  �t�b�N���v���C���[�ɓ������Ă���
{
    public void Enter(Hook hook)
    {
        Debug.Log("HitPlayerState�J��");
        hook.HookWrapper(Hook.HookProcess.ATTRACT_PLAYER);
    }
    public void Update(Hook hook)
    {
        hook.ChangeState(new NoAct());
    }
    public void Exit(Hook hook)
    {

    }
}
internal class NotHitPlayer : HookState    //  �t�b�N�������
{
    public void Enter(Hook hook)
    {

    }
    public void Update(Hook hook)
    {

    }
    public void Exit(Hook hook)
    {

    }
}




