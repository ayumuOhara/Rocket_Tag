//using Photon.Pun;
//using Photon.Realtime;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.UIElements;

//public class ThrowRocket : MonoBehaviour
//{
//    string hitName;
//    public float throwSpeed;
//    float throwedTime;
//    float retrieveTime;
//    float retrieveForce;
//    bool isThrowed;
//    bool isReturn;
//    bool isHoldRocket;

//    Vector3 startPos;
//    Vector3 playerPos;
//    Vector3 cameraCenter;
//    Vector3 judgeDistance;
//    Vector3 throwOffset;
//    Vector3 offsetZero;

//    [SerializeField] GameObject player;
//    GameObject hitPlayer;
//    [SerializeField] GameObject rocket;
//    [SerializeField] GameObject rocketPos;
//    GameObject targetPlayer;
//    Camera playerCamera;
//    void Start()
//    {
//        Initialize();
//    }
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.F) && isHoldRocket)
//        {
//            ThrowFlagChange();
//            SetParent(rocket, null);
//            cameraCenter = GetLineDir(rocket.transform.position, GetScreenCenterPos());
//            StartCoroutine(GetFristHit());
//            ApproachPos(rocket, rocket, throwOffset);
//        }
//        if (isThrowed && !isReturn)
//        {
//            StraightMoveToPos(rocket.transform, rocket.transform.position, cameraCenter, throwSpeed);
//            throwedTime += Time.deltaTime;
//        }
//        if (throwedTime > retrieveTime)
//        {
//            isReturn = true;
//            throwedTime = 0;
//        }
//        if (isReturn && hitName == "Player")
//        {
//            SetPlayerBool mySpb = player.GetComponent<SetPlayerBool>();
//            PhotonView playerView = player.GetComponent<PhotonView>();
//            playerView.RPC("SetHasRocket", RpcTarget.All, !mySpb.hasRocket);

//            SetPlayerBool targetSpb = targetPlayer.GetComponent<SetPlayerBool>();
//            PhotonView targetView = targetPlayer.GetComponent<PhotonView>();
//            targetView.RPC("SetHasPlayer", RpcTarget.All, !targetSpb.hasRocket);
//        }
//        if (isReturn && !isHoldRocket && hitName != "Player")
//        {
//            StraightMoveToPos(rocket.transform, rocket.transform.position, player.transform.position, retrieveForce);
//        }
//        if (isReturn && !isHoldRocket && IsNear(player, rocket, judgeDistance))
//        {
//            isReturn = false;
//            isHoldRocket = true;
//            SetParent(rocket, player.transform);
//            //ApproachPos(player, rocket, startPos);
//            //rocket.transform.rotation = player.transform.rotation;
//            rocket.transform.position = rocketPos.transform.position;
//            //ApproachPos(rocketPos, rocket, offsetZero);
//        }
//    }
//    void Initialize()    //  ������
//    {
//        hitName = null;
//        throwSpeed = 40f;
//        throwedTime = 10f;
//        retrieveTime = 2f;
//        retrieveForce = 70f;
//        isThrowed = false;
//        isReturn = false;
//        isHoldRocket = true;

//        hitPlayer = GameObject.Find("HitPlayer");
//        playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();

//        //rocket = GameObject.Find("Rocket");
//        //player = GameObject.Find("Player");

//        startPos = rocket.transform.localPosition;
//        playerPos = player.transform.position;
//        throwOffset = new Vector3(0, 5, 0);
//        offsetZero = new Vector3(0, 5, 0);
//    }
//    void ThrowFlagChange()    //  �����ɂ�锻��ύX
//    {
//        isThrowed = true;
//        isReturn = false;
//        isHoldRocket = false;
//    }
//    public Vector3 GetLineDir(Vector3 current, Vector3 target)    //  �^�[�Q�b�g�ɑ΂��Ẵx�N�g�����擾����
//    {
//        return target - current;
//    }
//    Vector3 GetScreenCenterPos()    //  �J�����̃��[���h�ł̒��S���W�����߂�
//    {
//        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1000);
//        Vector3 worldCenter = playerCamera.ScreenToWorldPoint(screenCenter);
//        return playerCamera.ScreenToWorldPoint(screenCenter);
//    }
//    void ApproachPos(GameObject axis, GameObject approcher, Vector3 offset)    //  axis�𒆐S��Approcher��Pos��offset�������ĕύX����
//    {
//        approcher.transform.position = axis.transform.position + offset;
//    }
//    public void StraightMoveToPos(Transform moved, Vector3 current, Vector3 target, float moveSpeed)    //  ���W�Ɍ������Ē����ړ�
//    {
//        moved.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);
//    }
//    void SetParent(GameObject child, Transform parent)    //  �e�I�u�W�F�N�g���Z�b�g����
//    {
//        child.transform.parent = parent;
//    }
//    IEnumerator GetFristHit()    //  �ŏ��ɍ������I�u�W�F�N�g��ۑ�����
//    {
//        Collider[] tempHits;

//        do
//        {
//            tempHits = Physics.OverlapCapsule(rocket.transform.position - Vector3.down * 2.5f, rocket.transform.position + Vector3.up * 2.5f, 2.5f * 1.1f);
//            if (tempHits.Length > 0)
//            {
//                if (tempHits[0].tag != rocket.tag)
//                {
//                    isThrowed = false;
//                    isReturn = true;
//                    targetPlayer = tempHits[0].gameObject;
//                    hitName = tempHits[0].tag;
//                }
//            }
//            yield return null;
//        } while (!isReturn);
//    }
//    bool IsNear(GameObject axis, GameObject judged, Vector3 judgeDistance)    //  axis�����Ƃ�judged���߂����𔻒�  
//    {
//        Vector3 posDif = axis.transform.position - judged.transform.position;
//        if (posDif.x < judgeDistance.x && posDif.y < judgeDistance.y && posDif.z < judgeDistance.z)
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }
//}
////using Photon.Realtime;
////using System.Net.Sockets;
////using System.Threading.Tasks;
////using UnityEngine;
//////using UnityEngine.InputSystem.LowLevel;
////using UnityEngine.UIElements;

////public class ThrowRocket : MonoBehaviour
////{
////    internal enum ThrowRocketMethod
////    {
////        GET_FIRST_HIT,
////        RETURN_ROCKET,
////        THROW_AND_INPUT_FIRST_HIT
////    }

////    float throwSpeed;
////    float holdPos_Z;
////    string firstHit;

////    Vector3 judgeDistance;

////    GameObject player;
////    GameObject rocket;
////    Camera playerCamera;

////    RocketPosState currentState;
////    void Start()
////    {
////        Initialize();
////    }
////    void Update()
////    {
////        currentState.Update(this);
////    }
////    void Initialize()    //  ������
////    {
////        throwSpeed = 5f;
////        firstHit = null;
////        holdPos_Z = this.transform.localPosition.z;

////        rocket = GameObject.Find("Rocket");
////        player = GameObject.Find("Player(Clone)");
////        playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
////        judgeDistance = new Vector3(2, 2, 2);
////    }
////    internal string GetFirstHit()    //  firstHit�v���p�e�B
////    {
////        return firstHit;
////    }
////    internal void ChangeState(RocketPosState newState)    //  ��ԑJ��
////    {
////        if (currentState != null)
////        {
////            currentState.Exit(this);
////        }
////        currentState = newState;
////        currentState.Enter(this);
////    }
////    internal Vector3 GetVecForScreenCenter(Vector3 perspective)    //  �J�������S�ւ̃��F�N�g�����Ƃ�
////    {
////        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1000);
////        return playerCamera.WorldToScreenPoint(screenCenter) - perspective;
////    }
////    internal async void ThrowAndInputFirstHit()    //  �ŏ��ɓ��������I�u�W�F�N�g�̃^�O��������
////    {
////        Collider[] tempCollider = new Collider[100];
////        while ((tempCollider = Physics.OverlapCapsule(rocket.transform.position - Vector3.down * 2.5f, rocket.transform.position + Vector3.up * 2.5f, 2.5f * 1.1f)) == null)
////        {
////            StraightMoveToPos(this.transform.position, this.transform.position, GetVecForScreenCenter(this.transform.position), throwSpeed * Time.deltaTime);
////            await Task.Yield();
////        }
////        firstHit = tempCollider[0].tag;
////    }
////    void StraightMoveToPos(Vector3 moved, Vector3 current, Vector3 target, float moveSpeed)    //  ���W�Ɍ������Ē����ړ�
////    {
////        moved = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);
////    }
////    void ApproachPos(Vector3 axis, Transform approcher, Vector3 offset, bool isApproachLocal)    //  axis�𒆐S��Approcher��Pos��offset�������ĕύX����(local/global�I����)
////    {
////        if (isApproachLocal)
////        {
////            approcher.transform.localPosition = transform.InverseTransformPoint(axis + offset);
////        }
////        else
////        {
////            approcher.transform.position = axis + offset;
////        }
////    }
////    internal async void Wrapper_Throw(ThrowRocketMethod throwRocketMethod)   //  ���̃X�N���v�g���̃��b�p�[�֐�
////    {
////        switch (throwRocketMethod)
////        {
////            case ThrowRocketMethod.GET_FIRST_HIT:
////                {
////                    GetFirstHit();
////                    break;
////                }
////            case ThrowRocketMethod.RETURN_ROCKET:
////                {
////                    ReturnRocket();
////                    break;
////                }
////            case ThrowRocketMethod.THROW_AND_INPUT_FIRST_HIT:
////                {
////                    ThrowAndInputFirstHit();
////                    break;
////                }
////        }
////    }
////    async void ReturnRocket()    //  ���P�b�g�A�ҏ���
////    {
////        while (IsNear(player, rocket, judgeDistance))
////        {
////            StraightMoveToPos(rocket.transform.position, rocket.transform.position, player.transform.position, throwSpeed);
////            await Task.Yield();
////        }
////        ApproachPos(Vector3.zero, rocket.transform, (player.transform.position - player.transform.forward * holdPos_Z), true);
////    }
////    bool IsNear(GameObject axis, GameObject judged, Vector3 judgeDistance)    //  axis�����Ƃ�judged���߂����𔻒�  
////    {
////        Vector3 posDif = axis.transform.position - judged.transform.position;
////        return posDif.x < judgeDistance.x && posDif.y < judgeDistance.y && posDif.z < judgeDistance.z;
////    }
////}
////internal interface RocketPosState    //  ���P�b�g�ʒu�C���^�[�t�F�[�X
////{
////    void Enter(ThrowRocket arg);    //  ���̏�ԂɑJ�ڎ����s
////    void Update(ThrowRocket arg);    //    ��Ԓ������Ǝ��s
////    void Exit(ThrowRocket arg);    //  ���̏�ԂɑJ�ڎ����s
////}
////public class HoldRocket : RocketPosState    //  ���P�b�g�������
////{
////    public void Enter(ThrowRocket throwRocket)
////    {

////    }
////    public void Update(ThrowRocket throwRocket)
////    {
////        if (Input.GetKeyDown(KeyCode.F))
////        {
////            throwRocket.ChangeState(new ThrowedRocket());
////        }
////    }
////    public void Exit(ThrowRocket throwRocket)
////    {

////    }
////}
////public class ThrowedRocket : RocketPosState    //  ���P�b�g������ꂽ���
////{
////    public void Enter(ThrowRocket throwRocket)
////    {
////        throwRocket.Wrapper_Throw(ThrowRocket.ThrowRocketMethod.THROW_AND_INPUT_FIRST_HIT);
////    }
////    public async void Update(ThrowRocket throwRocket)
////    {
////        //if (throwRocket.ThrowAndInputFirstHit<!= null && throwRocket.GetFirstHit() == "Player")
////        //{
////        //    T
////        //}
////        else if (throwRocket.GetFirstHit() != null)
////        {
////            throwRocket.ChangeState(new ReturningRocket());
////        }
////    }
////    public void Exit(ThrowRocket throwRocket)
////    {

////    }
////}
////public class ReturningRocket : RocketPosState    //  ���P�b�g�A�ҏ��
////{
////    public void Enter(ThrowRocket throwRocket)
////    {

////    }
////    public void Update(ThrowRocket throwRocket)
////    {

////    }
////    public void Exit(ThrowRocket throwRocket)
////    {

////    }
////}