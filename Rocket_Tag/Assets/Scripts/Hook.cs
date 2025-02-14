using UnityEngine;
using System.Threading.Tasks;
using System;
using Unity.VisualScripting;
using Unity.Properties;
using UnityEngine.InputSystem.DualShock;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine.Rendering;
////  StatePatternを用いてフックの生成、挙動の作成をするスクリプト  ////
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
    GameObject chainEntity;
    GameObject player;
    GameObject hitObj;
    List<GameObject> chains;
    Camera playerCam;
    PlayerMovement playerMovement;  ////----結合の際使用

    Vector3 hookUnlockDis_Small;
    Vector3 hookUnlockDis_Big;
    Vector3 chainUnlockDis_Small;
    Vector3 chainUnlockDis_Big;
    Vector3 chainDeleteDis;

    float attractSpd;
    float attractAcceleration;
    int chainsNo;

    internal GameObject HitObj
    { get { return hitObj; } }

    HookState currentState;                                                               ////  宣言区終了  ////
    void Start()                                                                          ////  以下処理区  ////
    {
        Initialize();
    }
    void Update()
    {
        //currentState.Update(this);

    }                                                                                     ////  処理区終了  ////
    void Initialize()    //  初期化                                                       ////  以下関数区  ////
    {
        hookPrefab = Resources.Load<GameObject>("Hook");
        chainPrefab = Resources.Load<GameObject>("HookChain");
        player = this.gameObject;
        playerCam = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        hitObj = null;
        chains = new List<GameObject>();

        chainDeleteDis = new Vector3(2f, 2f, 2f);

        attractSpd = 2f;
        attractAcceleration = 1.1f;
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
                    Vector3 hookGeneratePos = player.transform.position + playerCam.transform.forward * 1.35f;

                    GenerateObj(ref hookEntity, hookPrefab, hookGeneratePos);
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
    async void ThrowHook()    //  フック投擲
    {
        Collider[] tempCollider = new Collider[1];

        Vector3 chainNotGenerateDis_Small = new Vector3(1.5f, 2f, 100f);
        Vector3 chainNotGenerateDis_Big = new Vector3(1.5f, 2f, 0f);
        Vector3 tempScreenCenter = GetVecForScreenCenter(player.transform.position);

        Debug.Log(tempScreenCenter);
        float throwSpd = 30f;
        float throwAcceleration = 7f;
        float retrieveTime = 1f;
        float topPoint = 1.5f;
        float belowPoint = 1.5f;
        float radius = 0.6f;
        float chainLongSide = 0.35f;

        chains.Add(hookEntity);
        LookAtPos(hookEntity, tempScreenCenter);
        
        for (; ((tempCollider = GenerateHitDetection(hookEntity.transform, belowPoint, topPoint, radius)) == null || tempCollider.Length == 0) && (retrieveTime -= Time.deltaTime) > 0;)
        {
            StraightMoveToPos(hookEntity.transform, tempScreenCenter, (throwSpd + (throwAcceleration *= 1.1f)), 1);
            while (!IsInRange(chains[chainsNo].transform.position, player.transform.position - chainNotGenerateDis_Small, player.transform.position + chainNotGenerateDis_Big))
            {
                GenerateObj(ref chainEntity, chainPrefab, chains[chainsNo].transform.position - chains[chainsNo].transform.forward * chainLongSide);
                LookAtPos(chainEntity, hookEntity.transform.position);
                chainEntity.transform.parent = hookEntity.transform;
                chains.Add(chainEntity);
                chainsNo++;
                await Task.Yield();
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
    async void AttractPlayer ()    //  プレイヤー引き寄せ  
    {
        //  playerMovement = hitObj.GetComponent<PlayerMovement>();

        attractSpd = 1.5f;
        attractAcceleration = 1.25f;
        float attractingPlayerMoveSpd;    ////----結合時使用
        float hitStopTime = 0.9f;
        //  float tempPlayerSpeed = playerMovement.GetMoveSpeed();

        //  playerMovement.SetMoveSpeed(0.15f);
        hookUnlockDis_Small = new Vector3(1.2f, 2f, 0.7f);
        hookUnlockDis_Big = new Vector3(0.5f, 2f, 2.4f);
        chainUnlockDis_Small = new Vector3(3f, 1.7f, 1f);
        chainUnlockDis_Big = new Vector3(5f, 1.7f, 4f);

        Debug.Log("ヒットストップなう");
        while ((hitStopTime -= Time.deltaTime) > 0)
        {
            await Task.Yield();
        }
        while (!IsInRange(hookEntity.transform.position,player.transform.position - hookUnlockDis_Small, player.transform.position + hookUnlockDis_Big))
        {
            while (IsInRange(chains[chainsNo ].transform.position, player.transform.position - chainUnlockDis_Small, player.transform.position + chainUnlockDis_Big))
            {
                DestroyObj(chains[chainsNo]);
                await Task.Yield();
                chainsNo--;
            }
            SetPos(hookEntity.transform.position, hitObj.transform, Vector3.zero);
            StraightMoveToPos(hookEntity.transform, player.transform.position, attractSpd *= attractAcceleration, 1);
            await Task.Yield();
        }
        if(hookEntity != null)
        Destroy(hookEntity);
        //    playerMovement.SetMoveSpeed(tempPlayerSpeed);
        ChangeState(new NoAct());
    }
    void RetrieveHook()    //  フック回収
    {
        attractSpd = 1.5f;
        attractAcceleration = 1.1f;
        hookUnlockDis_Small = new Vector3(1.2f, 1.2f, 1.2f);
        chainUnlockDis_Small = new Vector3(0.5f, 0.5f, 0.5f);

        //while (!IsInRange(GetPosDif(player.transform.position, hookEntity.transform.position), hookUnlockPos))
        while (!IsInRange(hookEntity.transform.position, player.transform.position, player.transform.position + hookUnlockDis_Small))
        {
            while (!IsInRange(chains[--chainsNo].transform.position, player.transform.position, player.transform.position + chainUnlockDis_Small))
            {
                DestroyObj(chains[chainsNo]);
            }
            StraightMoveToPos(hookEntity.transform, player.transform.position, attractSpd *= attractAcceleration, 1);
        }
        Destroy(hookEntity);
        ChangeState(new NoAct());
    }
    Vector3 GetVecForScreenCenter(Vector3 perspective)    //  prespectiveからカメラ中心へのヴェクトルをとる
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 100);
        return playerCam.ScreenToWorldPoint(screenCenter) - perspective;
    }
    Collider[] GenerateHitDetection(Transform AppendObj, float belowPoint, float topPoint, float radius)    //  当たり判定生成  
    {
        return Physics.OverlapCapsule(AppendObj.transform.position - Vector3.down * belowPoint, AppendObj.transform.position + Vector3.up * topPoint, radius);
    }
    Vector3 GetPosDif(Vector3 point1, Vector3 point2)    //  座標間の距離を取得
    {
        Vector3 posDif = point1 - point2;
        if(posDif.x < 0)
        {
           // posDif *= -1;
        }
        if(posDif.y < 0)
        {
           // posDif.y *= -1;
        }
        if(posDif.z < 0)
        {
          //  posDif.z *= -1;
        }
        return posDif;
    }
    bool IsInRange(Vector3 pos, Vector3 smallerPos, Vector3 biggerPos)    //  オブジェクトが判定距離内かどうか(より大きい・未満)
    {
        return smallerPos.x < pos.x && pos.x < biggerPos.x && smallerPos.y < pos.y && pos.y < biggerPos.y && smallerPos.z < pos.z && pos.z < biggerPos.z;
    }
    void SetPos(Vector3 point, Transform moved, Vector3 offset)    //  オブジェクトを指定座標に瞬間移動させる
    {
        moved.position = point + offset;
    }
    void LookAtPos(GameObject obj, Vector3 pos)    //  指定した座標に向かせる
    {
        obj.transform.LookAt(pos);
    }
    void DestroyObj(GameObject obj)    //  オブジェクト破壊
    {
        Destroy(obj);
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
    //void CameraMove_Aim()    //  aim状態にカメラを移動させる
    //{
    //    playerCam.transform.position = Vector3.Lerp(playerCam.transform.position, player.transform.position + camOffset, camMoveSpd_Aim * Time.deltaTime);
    //}
    ////void MoveObj(Vector3 moved, Vector3 target, moveSpeed)    //  aim状態にカメラを移動させる
    //{
    //    playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, player.transform.position + cameraOffset, cameraMoveSpd_Aim * Time.deltaTime);
    //}
}                                                                                         ////  関数区終了  ////
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
    public void Enter(Hook hook)
    {
        Debug.Log("NoHitState突入");
    }
    public async void Update(Hook hook)
    {
        
        hook.HookWrapper(Hook.HookProcess.RETRIEVE_HOOK);
       
    }
    public void Exit(Hook hook)
    {

    }
}                                                                                         ////  State区終了  ////