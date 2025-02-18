using UnityEngine;
using System.Threading.Tasks;
using System;
//using Unity.VisualScripting;
using Unity.Properties;
using UnityEngine.InputSystem.DualShock;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using System.Linq.Expressions;
////  フックの生成、挙動の作成をするスクリプト  ////
internal interface HookState    //  フック状態インターフェース                            ////  以下State区  ////
{
    void Enter(Hook arg);
    void Update(Hook arg);
    void Exit(Hook arg);
}

internal class NoAct : HookState    //  フック系統のアクションをしていない状態
{
    public void Enter(Hook hook)
    {
        Debug.Log("NoActState突入");
    }
    public async void Update(Hook hook)
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            hook.ChangeState(new HookThrow());
        }
    }
    public void Exit(Hook hook)
    {

    }
}
internal class HookThrow : HookState    //  フック投擲状態
{
    public void Enter(Hook hook)
    {
        Debug.Log("HookThrowState突入");

        hook.HookWrapper(Hook.HookProcess.THROW_HOOK);
    }
    public async void Update(Hook hook)
    {
        //if (hook.HitObj != null)
        //{
        //    switch (hook.HitObj.tag)
        //    {
                
        //        case "Player":
        //            {
        //                Debug.Log(hook.HitObj.tag);
        //                Debug.Log("playerstate");
        //                hook.ChangeState(new HitPlayer());
        //                break;
        //            }
        //        default:
        //            {
        //                Debug.Log("NohitState");

        //                hook.ChangeState(new NotHitPlayer());
        //                break;
        //            }
        //    }
        //}
    }
    public void Exit(Hook hook)
    {

    }
}
internal class HitPlayer : HookState    //  フックがプレイヤーに当たってる状態
{
    public void Enter(Hook hook)
    {
        Debug.Log("HitPlayerState遷移");
        hook.HookWrapper(Hook.HookProcess.ATTRACT_PLAYER);
    }
    public void Update(Hook hook)
    {

    }
    public void Exit(Hook hook)
    {

    }
}
internal class NotHitPlayer : HookState    //  フック投擲状態
{
    public async void Enter(Hook hook)
    {
        Debug.Log("NoHitState突入");
        hook.HookWrapper(Hook.HookProcess.RETRIEVE_HOOK);
    }
    public async void Update(Hook hook)
    {

    }
    public void Exit(Hook hook)
    {

    }
}                                                                                         ////  State区終了  ////
public class Hook : MonoBehaviour    //  フックスクリプト
{
    internal enum HookProcess    //  フックの挙動一覧                                     ////  以下宣言区  ////
    {
        THROW_HOOK,
        ATTRACT_PLAYER,
        RETRIEVE_HOOK,
    }
    
    GameObject hookPrefab;
    GameObject hookEntity;
    GameObject chainPrefab;
    List<GameObject> chains;
    Transform hookEntityTF;
    Transform hitObj;
    Transform playerTF;
    Transform playerRightHand;
    Transform playerCamTF;
    Camera playerCam;
    PlayerMovement playerMovement;  ////----結合の際使用

    Vector3 hookUnlockDis_Small;
    Vector3 hookUnlockDis_Big;
    Vector3 chainUnlockDis_Small;
    Vector3 chainUnlockDis_Big;

    float attractSpd;
    float attractAcceleration;
    int chainsNo;

    internal Transform _HitObj
    { get { return hitObj; } }

    HookState currentState;                                                               ////  宣言区終了  ////
    void Start()                                                                          ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }
    void Update()
    {
        currentState.Update(this);
    }                                                                                     ////  処理区終了  ////
    void Initialize()    //  初期化                                                       ////  以下関数区  ////
    {
        hookPrefab = Resources.Load<GameObject>("Hook");
        chainPrefab = Resources.Load<GameObject>("HookChain");
        chains = new List<GameObject>();
        playerTF = this.transform;
        playerRightHand = GameObject.Find("RightHand").GetComponent<Transform>();
        playerCamTF = GameObject.Find("PlayerCamera").GetComponent<Transform>();
        playerCam = playerCamTF.GetComponent<Camera>();
        //playerMovement = 結合時使用

        chainsNo = 0;

        ChangeState(new NoAct());

    }
    internal void ChangeState(HookState newState)    //  状態遷移
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }
        currentState = newState;
        currentState.Enter(this);
    }
    internal void HookWrapper(HookProcess hookProcess)   // フックの挙動のラッパー関数
    {
        switch (hookProcess)
        {
            case HookProcess.THROW_HOOK:
                {
                    ThrowHook();    //  投擲処理
                    break;
                }
            case HookProcess.ATTRACT_PLAYER:
                {
                    AttractPlayer();    //  引き寄せ処理
                    break;
                }
            case HookProcess.RETRIEVE_HOOK:
                {
                    RetrieveHook();    //  フック回収処理
                    break;
                }
            default: break;
        }
    }
    async void ThrowHook()    //  フック投擲
    {
        chains = new List<GameObject>();
        GameObject chainEntity = null;
        Collider[] tempCollider = new Collider[1];

        Vector3 chainNotGenerateDis_Small = new Vector3(1.5f, 2f, 100f);
        Vector3 chainNotGenerateDis_Big = new Vector3(1.5f, 2f, 0f);
        Vector3 tempScreenCenter = GetVecForScreenCenter(playerTF.position);
        Vector3 hookGeneratePos = playerRightHand.position + playerCamTF.forward * 1.35f;

        Debug.Log(tempScreenCenter);
        float throwSpd = 30f;
        float throwAcceleration = 7f;
        float retrieveTime = 0.5f;
        float topPoint = 1.5f;
        float belowPoint = 1.5f;
        float radius = 0.6f;
        float chainLongSide = 0.35f;
        chainsNo = 0; 

        GenerateObj(ref hookEntity, hookPrefab, hookGeneratePos);
        chains.Add(hookEntity);
        hookEntityTF = hookEntity.transform;
        hookEntityTF.LookAt(tempScreenCenter);
        Debug.Log("投擲キング");    //     投擲
        while(chains[0] != null && ((tempCollider = GenerateHitDetection(hookEntityTF, belowPoint, topPoint, radius)) == null || tempCollider.Length == 0) && (retrieveTime -= Time.deltaTime) > 0)
        {
            StraightMoveToPos(hookEntityTF, tempScreenCenter, (throwSpd + (throwAcceleration *= 1.1f)), 1);
            while (chains[0] != null && !IsInRange(chains[chainsNo].transform.position, playerTF.position - chainNotGenerateDis_Small, playerTF.position + chainNotGenerateDis_Big))
            {
                GenerateObj(ref chainEntity, chainPrefab, chains[chainsNo].transform.position - chains[chainsNo].transform.forward * chainLongSide);
                chainEntity.transform.LookAt(hookEntityTF);
                chainEntity.transform.parent = hookEntityTF;
                chains.Add(chainEntity);
                chainsNo++;
                await Task.Yield();
            }
            await Task.Yield();
        }
        if (tempCollider.Length != 0 && tempCollider[0] != null)
        {
            hitObj = tempCollider[0].transform;
        }
        else
        {
            hitObj = hookEntityTF;
        }
        switch (hitObj.gameObject.tag)
        {
            case "Player":
                {
                    ChangeState(new HitPlayer());
                    break;
                }
            default:
                {
                    ChangeState(new NotHitPlayer());
                    break;
                }
        }
        Debug.Log("throw終わり");
    }
    async void AttractPlayer ()    //  プレイヤー引き寄せ  
    {
        //  playerMovement = hitObj.GetComponent<PlayerMovement>();    結合時使用-------------------

        hookUnlockDis_Small = new Vector3(1.2f, 2f, 0.7f);
        hookUnlockDis_Big = new Vector3(0.5f, 2f, 2.4f);
        chainUnlockDis_Small = new Vector3(3f, 1.7f, 1f);
        chainUnlockDis_Big = new Vector3(5f, 1.7f, 4f);
        
        //  float attractingPlayerMoveSpd;    結合時使用--------------------
        float hitStopTime = 0.9f;
        //  float tempPlayerSpeed = playerMovement.GetMoveSpeed();    結合時使用------------------------
        //  playerMovement.SetMoveSpeed(0.15f);    結合時使用----------------------

        attractSpd = 1.5f;
        attractAcceleration = 1.25f;

        while ((hitStopTime -= Time.deltaTime) > 0)
        {
            await Task.Yield();
        }
        while (chains[0] != null && !IsInRange(hookEntityTF.position,　playerTF.position - hookUnlockDis_Small, playerTF.position + hookUnlockDis_Big))
        {
            while (chains[0] != null && IsInRange(chains[chainsNo].transform.position, playerTF.position - chainUnlockDis_Small, playerTF.position + chainUnlockDis_Big))
            {
                Destroy(chains[chainsNo]);
                await Task.Yield();
                chainsNo = chainsNo - 1 == -1 ? chainsNo : --chainsNo;
                //chainsNo--;
            }
            if (chains[0] != null)
            {
                SetPos(hookEntityTF.position, hitObj, Vector3.zero);
                StraightMoveToPos(hookEntityTF, playerTF.position, attractSpd *= attractAcceleration, 1);
            }
            await Task.Yield();
        }
        //if (hookEntity != null)    DEBUG------------------------
            ThrowEndDel();
        //    playerMovement.SetMoveSpeed(tempPlayerSpeed);    結合時使用---------------------
        ChangeState(new NoAct());
    }
    async void RetrieveHook()    //  フック回収
    {
        hookUnlockDis_Big = new Vector3(1.2f, 1.2f, 1.2f);
        chainUnlockDis_Big = new Vector3(0.5f, 0.5f, 0.5f);

        attractSpd = 1.5f;
        attractAcceleration = 1.1f;
        int tmpChainsNo = chainsNo;
        chainsNo = 1;

        Debug.Log(chainsNo);
        while (chains[0] != null && !IsInRange(chains[0].transform.position, playerTF.position - hookUnlockDis_Big, playerTF.position + hookUnlockDis_Big))
        {
            while (chains[chainsNo] != null && !IsInRange(chains[chainsNo].transform.position, playerTF.position, playerTF.position + chainUnlockDis_Big))
            {
                Destroy(chains[chainsNo]);
                chainsNo = chainsNo + 1 == tmpChainsNo ? chainsNo : ++chainsNo;
                await Task.Yield();
            }
            StraightMoveToPos(chains[0].transform, playerTF.position, attractSpd *= attractAcceleration, 1);
            await Task.Yield();
        }
        Destroy(chains[0]);
        //ThrowEndDel();
        ChangeState(new NoAct());
    }
    Vector3 GetVecForScreenCenter(Vector3 perspective)    //  prespectiveからカメラ中心奥へのヴェクトルをとる
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 100);
        return playerCam.ScreenToWorldPoint(screenCenter) - perspective;
    }
    Collider[] GenerateHitDetection(Transform AppendObj, float belowPoint, float topPoint, float radius)    //  当たり判定生成  
    {
        return Physics.OverlapCapsule(AppendObj.transform.position - Vector3.down * belowPoint, AppendObj.transform.position + Vector3.up * topPoint, radius);
    }
    bool IsInRange(Vector3 pos, Vector3 smallerPos, Vector3 biggerPos)    //  オブジェクトが判定距離内かどうか(より大きい・未満)
    {
        return smallerPos.x < pos.x && pos.x < biggerPos.x && smallerPos.y < pos.y && pos.y < biggerPos.y && smallerPos.z < pos.z && pos.z < biggerPos.z;
    }
    void SetPos(Vector3 point, Transform moved, Vector3 offset)    //  オブジェクトを指定座標に瞬間移動させる
    {
        moved.position = point + offset;
    }
    void GenerateObj(ref GameObject entity, GameObject generatObj, Vector3 generatePos)    //  オブジェクトをpositionを指定して生成
    {
        entity = GameObject.Instantiate(generatObj);
        entity.transform.position = generatePos;
    }
    void StraightMoveToPos(Transform moved, Vector3 target, float moveSpeed, float acceleration)    //  座標に向かって直線移動
    {
        moved.position = Vector3.MoveTowards(moved.transform.position, target, moveSpeed * acceleration * Time.deltaTime);
    }
    void ThrowEndDel()    //  投擲終了による削除処理
    {
        chains.Clear();
        Destroy(hookEntity);
    }
}                                                                                         ////  関数区終了  ////