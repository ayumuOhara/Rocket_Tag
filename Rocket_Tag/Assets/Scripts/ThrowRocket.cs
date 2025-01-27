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
    string hitName = null;
    float throwSpeed = 3f;
    float throwedTime = 10f;
    float retrieveTime = 1.5f;
    float returnForce = 10;
    bool isThrowed;
    bool isReturn;
    bool isHoldRocket;

    Vector3 startPos;

    GameObject player;
    GameObject rocket;
    List<Collider> hitCollider;
    CapsuleCollider capsuleCollider;
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
            StraightMoveToPos(rocket.transform, rocket.transform.position, GetLineDir(GetScreenCenterPos(), rocket.transform.position), throwSpeed);
        }
        if (isThrowed && hitName == "Player")
        {
            Debug.Log("playerに当たった");
        }
        else if (isThrowed && hitName != "Player")
        {
            isReturn = true;
        }
        if (isReturn)
        {
            StraightMoveToPos(rocket.transform, rocket.transform.position, GetLineDir(player.transform.position, rocket.transform.position), throwSpeed);
        }
        if (isThrowed)
        {
            throwedTime += Time.deltaTime;
        }
        //if (isReturn && )
        //{
        //    ApproachPos(player, rocket, startPos);
        //    isReturn = false;
        //    isHoldRocket = true;
        //    isThrowed = false;
        //    throwedTime = 0;
        //}
        Debug.Log(name);
    }
    void Initialize()    //  初期化
    {
        isThrowed = false;
        isReturn = false;
        isHoldRocket = true;
        
        startPos = rocket.transform.position;
        Debug.Log(startPos);
        rocket = GameObject.Find("Rocket");
        player = GameObject.Find("Player");
        hitCollider = new List<Collider>();
        capsuleCollider = rocket.GetComponent<CapsuleCollider>();
        rocketCS = GetComponent<Rocket>();
    }
    void ThrowFlagChange()    //  投げによる判定変更
    {
        isThrowed = true;
        isReturn = false;
        isHoldRocket = false;
    }
    public Vector3 GetLineDir(Vector3 target, Vector3 current)    //  ターゲットに対してのベクトルを取得する
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
    void StraightMoveToPos(Transform moved, Vector3 current, Vector3 target, float moveSpeed)    //  座標に向かって直線移動
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
        StraightMoveToPos(rocket.transform, rocket.transform.position, rocket.transform.parent.position, returnForce);
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
        Collider[] tempHitColliders;
        do
        {
            tempHitColliders = Physics.OverlapCapsule(rocket.transform.position - Vector3.down * capsuleCollider.height * 0.5f,
                                              rocket.transform.position + Vector3.up * capsuleCollider.height * 0.5f,
                                              capsuleCollider.radius);
            yield return null;
        } while (tempHitColliders.Length > 1 && tempHitColliders[0] != null);
        Debug.Log(tempHitColliders[0].tag);
        hitName = tempHitColliders[0].tag;
    }
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
    void ReturnFlagChange()
    {
        
    }
   // bool IsNear(GameObject axis, GameObject judged, float distance_x, float distance_y)    //  
   // {
   //     Vector3 posDifference = axis.transform.position - judged.transform.position;
   ////     return Mathf.Abs(posDifference.x) < distance && posDifference.y < distance;
   // }
}