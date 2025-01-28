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
    Vector3 playerPos;
    Vector3 throwOffset;
    Vector3 judgeDistance;

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
            //if (!rocketCS.isExplode)
            //{
            //    SetParent(rocket, null);
            //}
            StartCoroutine(GetFristHit());
            ApproachPos(rocket, rocket, throwOffset);
        }
        if (isThrowed && !isReturn)
        {
            StraightMoveToPos(rocket.transform, rocket.transform.position, GetLineDir(rocket.transform.position, GetScreenCenterPos()), throwSpeed);
            throwedTime += Time.deltaTime;
        }
        if (throwedTime > retrieveTime)
        {
            isReturn = true;
            throwedTime = 0;
        }
        if (isReturn && hitName == "Player")
        {
            Debug.Log("player�ɓ�������");
        }
        if (isReturn && !isHoldRocket && hitName != "Player")
        {
            Debug.Log(5);
            StraightMoveToPos(rocket.transform, rocket.transform.position, player.transform.position, retrieveForce);
        }
        if (isReturn && IsNear(player, rocket, judgeDistance))
        {
            isHoldRocket = true;
        //    SetParent(rocket, player.transform);
            ApproachPos(player, rocket, startPos);
        }
    }
    void Initialize()    //  ������
    {
        hitName = null;
        throwSpeed = 40f;
        throwedTime = 10f;
        retrieveTime = 2f;
        retrieveForce = 70f;
        isThrowed = false;
        isReturn = false;
        isHoldRocket = true;

        rocket = GameObject.Find("Rocket");
        player = GameObject.Find("Player");

        startPos = rocket.transform.localPosition;
        playerPos = player.transform.position;
        throwOffset = new Vector3 (0, 2, 0);
        judgeDistance = new Vector3(2, 2, 2);

        rocketCS = GetComponent<Rocket>();
    }
    void ThrowFlagChange()    //  �����ɂ�锻��ύX
    {
        isThrowed = true;
        isReturn = false;
        isHoldRocket = false;
    }
    public Vector3 GetLineDir(Vector3 current, Vector3 target)    //  �^�[�Q�b�g�ɑ΂��Ẵx�N�g�����擾����
    {
        return target - current;
    }
    Vector3 GetScreenCenterPos()    //  �J�����̃��[���h�ł̒��S���W�����߂�
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1000);
        Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);
        return Camera.main.ScreenToWorldPoint(screenCenter);
    }
    void ApproachPos(GameObject axis, GameObject approcher, Vector3 offset)    //  axis�𒆐S��Approcher��Pos��offset�������ĕύX����
    {
        approcher.transform.position = axis.transform.position + offset;
    }
    public void StraightMoveToPos(Transform moved, Vector3 current, Vector3 target, float moveSpeed)    //  ���W�Ɍ������Ē����ړ�
    {
        moved.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);
    }
    void SetParent(GameObject child, Transform parent)    //  �e�I�u�W�F�N�g���Z�b�g����
    {
        child.transform.parent = parent;
    }
    IEnumerator GetFristHit()    //  �ŏ��ɍ������I�u�W�F�N�g��tag��ϐ��ɓ����
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
    bool IsNear(GameObject axis, GameObject judged, Vector3 judgeDistance)    //  axis�����Ƃ�judged���߂����𔻒�  
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
//    public void StraightMoveToPos(Transform moved, Vector3 current, Vector3 target, float moveSpeed)    //  ���W�Ɍ������Ē����ړ�
//    {
//        moved.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);
//    }
//    public Vector3 RocketPosition
//    {
//        get { return rocketPosition; } // �l��Ԃ��i�ǂݎ��j
//        set { rocketPosition = value; } // �l��ݒ肷��i�������݁j
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