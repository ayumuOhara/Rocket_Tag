using NUnit.Framework.Internal;
using Photon.Realtime;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
                                                                                         ////  playerCam�̃t�b�N�������̐���@�@////
internal interface CamState    //  �J������Ԃ̃C���^�[�t�F�[�X                            ////  �ȉ�State��  ////
{
    void Enter(CamAim arg);
    void Update(CamAim arg);
    void Exit(CamAim arg);
}
internal class NotAiming : CamState    //  ADS����Ȃ����
{
    public void Enter(CamAim camAim)
    {

    }
    public void Update(CamAim camAim)
    {
        if (Input.GetMouseButtonDown(1))
        {
            camAim.ChangeState(new Aiming());
        }
    }
    public void Exit(CamAim camAim)
    {

    }
}
internal class Aiming : CamState    //  ADS���
{
    public void Enter(CamAim camAim)
    {

    }
    public void Update(CamAim camAim)
    {
        camAim.CamWrapper(CamAim.CamProcess.AIMING);    //  ADS���̏������Ă�
        if(Input.GetMouseButtonUp(1))
        {
            camAim.ChangeState(new BackDefualtPos());
        }
    }
    public void Exit(CamAim camAim)
    {

    }
}
internal class BackDefualtPos : CamState    //  ADS���
{
    public void Enter(CamAim camAim)
    {

    }
    public void Update(CamAim camAim)
    {
        if(Input.GetMouseButtonDown(1))
        {
            camAim.ChangeState(new NotAiming());
        }
    }
    public void Exit(CamAim camAim)
    {

    }
}        ////  State��I��  ////
internal class CamAim : MonoBehaviour
{
    internal enum CamProcess    //  �J�����̋����ꗗ                                     ////  �ȉ��錾��  ////
    {
        AIMING,
        TRACK_PLAYER,
    }

    CamState currentState;

    GameObject player;
    Camera playerCam;
    Transform playerHeadTF;
    Transform playerTF;
    Transform playerCamTF;
   
    Quaternion playerStandRot;
    Vector3 playerCamOffset = new Vector3(-1.02f, -1.74f, 2.75f);
    Vector3 camAddSpdSlowedDis_Small = new Vector3(0.8f, 0.5f, 0.3f);
    Vector3 camAddSpdSlowedDis_Big = new Vector3(0.8f, 0.5f, 0.3f);
    Vector3 currentAngle;

    float camMoveSpd_Aim = 8f;
    float camMoveAddSpd_Aim = 4.5f;
    float headMoveSpd_Aim = 4.5f;
    float wholeBodyMoveSpd_Aim = 4.5f;
    bool isAim = false;                                                                  ////  �錾��I��  ////
    void Start()                                                                         ////  �ȉ�������  ////
    {
        Initialize();    //  ������
    }

    void Update()
    {
        currentState.Update(this);
    }                                                                                     ////  ������I��  ////
    void Initialize()    //  ������                                                       ////  �ȉ��֐���  ////
    {
        player = this.gameObject;
        playerCam = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        playerTF = player.transform;
        playerHeadTF = GameObject.Find("Head").GetComponent<Transform>();
        playerCamTF = playerCam.transform;
        currentAngle = playerTF.eulerAngles;
        ChangeState(new NotAiming());
        playerStandRot = playerTF.rotation;
    }
    internal void ChangeState(CamState newState)    //  ��ԑJ��
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }
        currentState = newState;
        currentState.Enter(this);    //memoo -3.832 1.744 -2.142
    }
    internal void CamWrapper(CamProcess camProcess)   // �J�����̋����̃��b�p�[�֐�
    {
        switch (camProcess)
        {
            case CamProcess.AIMING:
                {
                    playerCamTF.position = Vector3.Lerp(playerCamTF.position, playerTF.position - playerCamOffset, camMoveSpd_Aim * Time.deltaTime);
                    LookPos(playerTF, GetVecForScreenCenter(playerTF.position), camMoveSpd_Aim, 8);    //  ����L�����̑̂��J�������S���Ɍ�������
                    LookPos(playerHeadTF, GetVecForScreenCenter(playerHeadTF.position), wholeBodyMoveSpd_Aim, 1);    //  //  ����L�����̓����J�������S���Ɍ�������
                    break;
                }
            default: break;
        }
    }
    async void BackDefaultPos()
    {

    }
    void LookPos(Transform obj, Vector3 pos, float moveSpd, float correctionX)    //  �w�肵���|�W�V�����������悤�ɉ�]������
    {
        Quaternion rotToPos = Quaternion.LookRotation(GetVecForScreenCenter(pos));
        rotToPos.x /= correctionX;
        obj.rotation = Quaternion.Lerp(obj.rotation, rotToPos, moveSpd * Time.deltaTime);
    }
    Vector3 GetVecForScreenCenter(Vector3 perspective)    //  prespective����J�������S�ւ̃��F�N�g�����Ƃ�
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 100);
        return playerCam.ScreenToWorldPoint(screenCenter);
    }
}                                                                                         ////  �֐���I�� ////