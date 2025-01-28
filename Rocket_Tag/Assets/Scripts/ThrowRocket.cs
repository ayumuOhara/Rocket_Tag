using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ThrowRocket : MonoBehaviour
{
    string hitName;
    public float throwSpeed;
    float throwedTime;
    float retrieveTime;
    float retrieveForce;
    bool isThrowed;
    bool isReturn;
    bool isHoldRocket;

    Vector3 startPos;

    GameObject player;
    GameObject rocket;
    Rocket rocketCS;
    void Start()
    {
        Initialize();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isHoldRocket)
        {
            ThrowFlagChange();
            SetParent(rocket, null);
            StartCoroutine(GetFristHit());
        }
        if (isThrowed && !isReturn)
        {
            StraightMoveToPos(rocket.transform, rocket.transform.position, GetLineDir(rocket.transform.position, GetScreenCenterPos()), throwSpeed);
            throwedTime += Time.deltaTime;
        }
        //if (throwedTime > retrieveTime)
        //{
        //    isReturn = true;
        //}
        if (isReturn && hitName == "Player")
        {
            Debug.Log("playerに当たった");
        }
        if (isReturn && !isHoldRocket && hitName != "Player")
        {
            StraightMoveToPos(rocket.transform, rocket.transform.position, GetLineDir(rocket.transform.position, player.transform.position), retrieveForce);
            Debug.Log(7777);
        }
        if (isReturn && IsNear(player, rocket, new Vector3(2, 2, 2)))
        {
            isHoldRocket = true;
            SetParent(rocket, player.transform);
            ApproachPos(player, rocket, startPos);
        }

        //if (isReturn && )
        //{
        //    ApproachPos(player, rocket, startPos);
        //    isReturn = false;
        //    isHoldRocket = true;
        //    isThrowed = false;
        //    throwedTime = 0;
        //}
        Debug.Log(hitName);
    }
    void Initialize()    //  初期化
    {
        hitName = null;
        throwSpeed = 9f;
        throwedTime = 10f;
        retrieveTime = 1.5f;
        retrieveForce = 20f;
        isThrowed = false;
        isReturn = false;
        isHoldRocket = true;

        rocket = GameObject.Find("Rocket");
        player = GameObject.Find("Player");

        startPos = rocket.transform.position;

        rocketCS = GetComponent<Rocket>();
    }
    void ThrowFlagChange()    //  投げによる判定変更
    {
        isThrowed = true;
        isReturn = false;
        isHoldRocket = false;
    }
    public Vector3 GetLineDir(Vector3 current, Vector3 target)    //  ターゲットに対してのベクトルを取得する
    {
        return target - current;
    }
    Vector3 GetScreenCenterPos()    //  カメラのワールドでの中心座標を求める
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1000);
        Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);
        return Camera.main.ScreenToWorldPoint(screenCenter);
    }
    bool IsNeedRetrieve()    // 　ロケット回収必要か判定
    {
        return throwedTime > retrieveTime;
    }
    public void RetriveByStraightLine()
    {
        isReturn = true;
        // GetLineDir(rocket.transform.position, player.transform.position);
    }
    void ApproachPos(GameObject axis, GameObject approcher, Vector3 offset)    //  axisを中心にApprocherのPosをoffset分加えて変更する
    {
        approcher.transform.position = axis.transform.position + offset;
    }
    string WhatIsHit(List<Collider> hits)
    {
        if (hits.Count > 0)
        {
            return hits[0].tag;  // 最初にヒットしたオブジェクトのタグを返す
        }
        return null;  // ヒットしなかった場合は null 
    }
    public void StraightMoveToPos(Transform moved, Vector3 current, Vector3 target, float moveSpeed)    //  座標に向かって直線移動
    {
        moved.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);
    }
    void SetParent(GameObject child, Transform parent)    //  親オブジェクトをセットする
    {
        child.transform.parent = parent;
    }
    void RetrunRocket()
    {
        SetParent(rocket, player.transform);
        isReturn = true;
        isThrowed = false;
        StraightMoveToPos(rocket.transform, rocket.transform.position, rocket.transform.parent.position, retrieveForce);
    }
    //void SetHitCapsuletCollider(List<Collider> hitCollider)    //  カプセルコライダーに当たったコライダーを格納する
    //{
    //    hitCollider.Clear();
    //    Collider[] tempHitColliders = Physics.OverlapCapsule(rocket.transform.position - Vector3.down * capsuleCollider.height * 0.5f,
    //                                                          rocket.transform.position + Vector3.up * capsuleCollider.height * 0.5f,
    //                                                          capsuleCollider.radius);
    //    hitCollider.AddRange(tempHitColliders);
    //    return
    //        }
    //string SetHitCapsuletCollider(List<Collider> hitCollider)    //  カプセルコライダーに当たったコライダーを格納する
    //{
    //    return tempHitColliders[0].tag;
    //}
    IEnumerator GetFristHit()
    {
        Collider[] tempHits;

        do
        {
            tempHits = Physics.OverlapCapsule(rocket.transform.position - Vector3.down * 2.5f, rocket.transform.position + Vector3.up * 2.5f, 2.5f * 1.1f);
            if (tempHits.Length > 0)
            {
                if (tempHits[0].tag != rocket.tag)
                {
                    isReturn = true;
                    hitName = tempHits[0].tag;
                }
            }
            yield return null;
        } while (!isReturn);
    }
    //}
    //async Task<string> GetFirstHit()    //  最初に当たったオブジェクトのタグを返す
    //{
    //    Collider[] tempHitColliders;
    //    do
    //    {
    //        tempHitColliders = Physics.OverlapCapsule(rocket.transform.position - Vector3.down * capsuleCollider.height * 0.5f,
    //                                          rocket.transform.position + Vector3.up * capsuleCollider.height * 0.5f,
    //                                          capsuleCollider.radius);
    //    } while (tempHitColliders.Length > 1 && tempHitColliders[0] != null);
    //    Debug.Log(hitName);
    //    return tempHitColliders[0].tag;
    //}
    //void ReturnFlagChange()
    //{
    //}
    //  bool IsNear(GameObject axis, GameObject judged, float distance_x, float distance_y, float distance_z)    //  axisをもとにjudgedが近いかを判定  
    bool IsNear(GameObject axis, GameObject judged, Vector3 judgeDistance)    //  axisをもとにjudgedが近いかを判定  
    {
        Vector3 posDif = axis.transform.position - judged.transform.position;
        if (posDif.x < judgeDistance.x && posDif.y < judgeDistance.y && posDif.z < judgeDistance.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
//public class ThorwRocket_ : MonoBehaviour
//{
//    RocketState rocketState;

//    string hitName;
//    float throwSpeed;
//    float throwedTime;
//    float retrieveTime;
//    float returnForce;
//    bool isThrowed;
//    bool isReturn;
//    bool isHoldRocket;

//    Vector3 startPos;

//    GameObject player;
//    GameObject rocket;
//    CapsuleCollider capsuleCollider;
//    Rocket rocketCS;
//    void Start()
//    {
//        rocketState = new HoldState();
//    }
//    void Update()
//    {
//        rocketState.Handle(this);
//    }
//    public void StraightMoveToPos(Transform moved, Vector3 current, Vector3 target, float moveSpeed)    //  座標に向かって直線移動
//    {
//        moved.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);
//    }
//    public Vector3 RocketPosition
//    {
//        get { return rocketPosition; } // 値を返す（読み取り）
//        set { rocketPosition = value; } // 値を設定する（書き込み）
//    }
//}
//public interface RocketState
//{
//    void Handle(ThorwRocket_ rocket);
//}
//public class HoldState : RocketState
//{
//    void Handle(ThorwRocket_ rocket)
//    {
//        if (Input.GetKeyDown(KeyCode.F))
//        {
//            //   rocket.StraightMoveToPos()
//            rocket.
//        }
//    }
//}
//public class ThrowState : RocketState
//{
//    void Handle(ThorwRocket_ rocket)
//    {

//    }
//}
//public class Returntate : RocketState
//{
//    void Handle(ThorwRocket_ rocket)
//    {

//    }