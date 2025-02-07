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
        RETRIEVE_HOOK,
    }

    GameObject hookPrefab;
    GameObject hookEntity;
    GameObject chainPrefab;
    GameObject chainEntity;
    GameObject player;
    GameObject hitObj;
    GameObject[] chains;
    Camera playerCamera;
    PlayerMovement playerMovement;

    Vector3 hookGeneratePos;
    Vector3 hookUnlockPos;
    Vector3 chainDeleateDis;

    float throwSpeed;
    float attractSpeed;
    float attractAcceleration;
    float attractingPlayerMoveSpd;
    float hookOffset_Z;
    float hitStopTime;

    int chainsNo;

    internal GameObject HitObj
    {
        get { return hitObj; }
    }

    HookState currentState;
    void Start()
    {
        Initialize();
        Time.timeScale = 0.5f;
    }
    void Update()
    {
        currentState.Update(this);
    }
    void Initialize()    //  ������
    {
        hookPrefab = Resources.Load<GameObject>("Hook");
        chainPrefab = Resources.Load<GameObject>("HookChain");
        player = this.gameObject;    //  �d�l�ɂ���Ē����K
        playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        hitObj = null;

        hookUnlockPos = new Vector3(2.5f, 2.5f, 2.5f);
        chainDeleateDis = new Vector3(2f, 2f, 2f);

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
                    ThrowHook();
                    break;
                }
            case HookProcess.ATTRACT_PLAYER:
                {
                    AttractPlayer();
                    break;
                }
            case HookProcess.RETRIEVE_HOOK:
                {
                    RetrieveHook();
                        break;
                }
            default: break;
        }
    }
    async void ThrowHook()    //  �t�b�N����
    {
        GenerateObj(ref hookEntity, hookPrefab, hookGeneratePos);

        Transform flontChain = hookEntity.transform;
        Collider[] tempCollider = new Collider[1];

        Vector3 chainGenerateDis = new Vector3(1.4f,1.4f,0.8f);
        Vector3 tempScreenCenter = GetVecForScreenCenter(player.transform.position);
        
        float throwAcceleration = 7f;
        float retrieveTime = 1f;
        float topPoint = 1.5f;
        float belowPoint = 1.5f;
        float radius = 1.5f;
        float chainHorizonSize = 0.03f;

        int accelerarionLenPlus = 3;
        chainsNo = 0;
        chains = new GameObject[(int)((retrieveTime * throwSpeed * throwAcceleration * accelerarionLenPlus) / (chainHorizonSize))];    //  �w�͏C��
        Debug.Log(chains.Length);

        hookEntity.transform.LookAt(tempScreenCenter);
        chains[0] = hookEntity;

        for (chainsNo = 0; (tempCollider = GenerateHitDetection(hookEntity.transform, belowPoint, topPoint, radius)) == null || tempCollider.Length == 0 && (retrieveTime -= Time.deltaTime) > 0; )
        {
            StraightMoveToPos(hookEntity.transform, tempScreenCenter, (throwSpeed + (throwAcceleration *= 1.1f)) * Time.deltaTime);
            while(!IsInRange(GetPosDif(chains[chainsNo].transform.position, player.transform.position), chainGenerateDis))
            {
                GenerateObj(ref chainEntity, chainPrefab, chains[chainsNo].transform.position - chains[chainsNo].transform.forward * chainGenerateDis.z);
                chainEntity.transform.parent = hookEntity.transform;
                changeRotation(chainEntity, hookEntity.transform.position);
                chains[++chainsNo] = chainEntity;
            }
            await Task.Yield();
        }
        if (retrieveTime < 0)
        {
            hitObj = hookEntity;
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
    Collider[] GenerateHitDetection(Transform AppendObj, float belowPoint, float topPoint, float radius)    //  �����蔻�萶��  
    {
        return Physics.OverlapCapsule(AppendObj.transform.position - Vector3.down * belowPoint, AppendObj.transform.position + Vector3.up * topPoint, radius);
    }
    async void AttractPlayer ()    //  ������  
    {
        //  playerMovement = hitObj.GetComponent<PlayerMovement>();

        //  float tempPlayerSpeed = playerMovement.GetMoveSpeed();

        //  playerMovement.SetMoveSpeed(0.15f);

        Debug.Log("�q�b�g�X�g�b�v�Ȃ�");
        while ((hitStopTime -= Time.deltaTime) > 0)
        {
            await Task.Yield();
        }
        //    playerMovement.SetMoveSpeed(tempPlayerSpeed);
        while (!IsInRange(GetPosDif(player.transform.position, hookEntity.transform.position), hookUnlockPos))
        {
            while (IsInRange(GetPosDif(chains[chainsNo].transform.position, player.transform.position), chainDeleateDis))
            {
                Debug.Log("����V���O");
                DestroyObj(chains[chainsNo--]);
            }
            SetPos(hookEntity.transform.position, hitObj.transform, Vector3.zero);
            StraightMoveToPos(hookEntity.transform, player.transform.position, attractSpeed);
            attractSpeed *= attractAcceleration;
            await Task.Yield();
        }
        Destroy(hookEntity);
        ChangeState(new NoAct());
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
    bool IsInRange(Vector3 pos, Vector3 judgePos)    //  �I�u�W�F�N�g�����苗�������ǂ���
    {
        return pos.x < judgePos.x && pos.y < judgePos.y && pos.z < judgePos.z;
        Debug.Log(pos);
    }
    void SetPos(Vector3 point, Transform moved, Vector3 offset)    //  �I�u�W�F�N�g���w����W�ɏu�Ԉړ�������
    {
        moved.position = point + offset;
    }
    void changeRotation(GameObject obj, Vector3 pos)    //  �w�肵�����W�Ɍ�������
    {
        obj.transform.LookAt(pos);
    }
    void DestroyObj(GameObject obj)    //  �I�u�W�F�N�g�j��
    {
        Destroy(obj);
    }
    void RetrieveHook()    //  �t�b�N���
    {
        while (!IsInRange(GetPosDif(player.transform.position, hookEntity.transform.position), hookUnlockPos))
        {
            while (IsInRange(GetPosDif(chains[chainsNo].transform.position, player.transform.position), chainDeleateDis))
            {
                Debug.Log("����V���O");
                DestroyObj(chains[chainsNo--]);
            }
            //StraightMoveToPos(hookEntity.transform, player.transform.position, attractSpeed);
            StraightMoveToPos(hookEntity.transform, player.transform.position, 0.1f);
        }
        Destroy(hookEntity);
        ChangeState(new NoAct());
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
                        Debug.Log("playerstate");
                        hook.ChangeState(new HitPlayer());
                        break;
                    }
                default:
                    {
                        Debug.Log("NohitState");

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
        hook.HookWrapper(Hook.HookProcess.RETRIEVE_HOOK);
    }
    public void Exit(Hook hook)
    {

    }
}