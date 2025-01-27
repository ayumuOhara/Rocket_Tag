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
            Debug.Log("player�ɓ�������");
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
    void Initialize()    //  ������
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
    void ThrowFlagChange()    //  �����ɂ�锻��ύX
    {
        isThrowed = true;
        isReturn = false;
        isHoldRocket = false;
    }
    public Vector3 GetLineDir(Vector3 target, Vector3 current)    //  �^�[�Q�b�g�ɑ΂��Ẵx�N�g�����擾����
    {
        return target - current;
    }
    Vector3 GetScreenCenterPos()    //  �J�����̃��[���h�ł̒��S���W�����߂�
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1000);
        Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);
        return Camera.main.ScreenToWorldPoint(screenCenter);
    }
    bool IsNeedRetrieve()    // �@���P�b�g����K�v������
    {
        return throwedTime > retrieveTime;
    }
    public void RetriveByStraightLine()    
    {
        isReturn = true;
        // GetLineDir(rocket.transform.position, player.transform.position);
    }
    void ApproachPos(GameObject axis, GameObject approcher, Vector3 offset)    //  axis�𒆐S��Approcher��Pos��offset�������ĕύX����
    {
        approcher.transform.position = axis.transform.position + offset;
    }
    string WhatIsHit(List<Collider> hits)
    {
        if (hits.Count > 0)
        {
            return hits[0].tag;  // �ŏ��Ƀq�b�g�����I�u�W�F�N�g�̃^�O��Ԃ�
        }
        return null;  // �q�b�g���Ȃ������ꍇ�� null 
    }
    void StraightMoveToPos(Transform moved, Vector3 current, Vector3 target, float moveSpeed)    //  ���W�Ɍ������Ē����ړ�
    {
         moved.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);
    }
    void SetParent(GameObject child, Transform parent)    //  �e�I�u�W�F�N�g���Z�b�g����
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
    //void SetHitCapsuletCollider(List<Collider> hitCollider)    //  �J�v�Z���R���C�_�[�ɓ��������R���C�_�[���i�[����
    //{
    //    hitCollider.Clear();
    //    Collider[] tempHitColliders = Physics.OverlapCapsule(rocket.transform.position - Vector3.down * capsuleCollider.height * 0.5f,
    //                                                          rocket.transform.position + Vector3.up * capsuleCollider.height * 0.5f,
    //                                                          capsuleCollider.radius);
    //    hitCollider.AddRange(tempHitColliders);
    //    return
    //        }
    //string SetHitCapsuletCollider(List<Collider> hitCollider)    //  �J�v�Z���R���C�_�[�ɓ��������R���C�_�[���i�[����
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
    //async Task<string> GetFirstHit()    //  �ŏ��ɓ��������I�u�W�F�N�g�̃^�O��Ԃ�
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