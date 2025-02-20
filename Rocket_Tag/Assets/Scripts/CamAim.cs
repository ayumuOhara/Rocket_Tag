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
        camAim._CamController.isAiming = false;
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
        camAim._CamController.isAiming = true;
        camAim._PlayerMovement.SetMoveSpeed(camAim._playerMoveSpd_Aim);
    }
    public void Update(CamAim camAim)
    {
        camAim.CamWrapper(CamAim.CamProcess.AIMING);    //  ADS���̏������Ă�
        if(Input.GetMouseButtonUp(1))
        {
            camAim.ChangeState(new NotAiming());
        }
    }
    public void Exit(CamAim camAim)
    {
        camAim._PlayerMovement.SetMoveSpeed(camAim._tmpPlayerMoveSpd);
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
    //Transform playerCamTF;    TEST------------------------
    CameraController camController;
    PlayerMovement playerMovement;

    Quaternion playerStandRot;
    Vector3 playerCamOffset;
    Vector3 camAddSpdSlowedDis_Small;
    Vector3 camAddSpdSlowedDis_Big;
    Vector3 currentAngle;

    float playerMoveSpd_Aim;
    float tmpPlayrMoveSpd;
    float camMoveSpd_Aim;
    float camMoveAddSpd_Aim;
    float headMoveSpd_Aim;
    float wholeBodyMoveSpd_Aim;
    bool isAim;
   
    internal CameraController _CamController
    { get { return camController; } set { camController = value; } }                     ////  �錾��I��  ////
    internal PlayerMovement _PlayerMovement
    { get { return playerMovement; } }
    internal float _playerMoveSpd_Aim
    { get { return playerMoveSpd_Aim; } }
    internal float _tmpPlayerMoveSpd
    { get { return tmpPlayrMoveSpd; } }
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
        //playerCamTF = playerCam.transform;    TEST--------------------------
        currentAngle = playerTF.eulerAngles;
        camController = playerCam.GetComponent<CameraController>();
        playerMovement = player.GetComponent<PlayerMovement>();

        playerCamOffset = new Vector3(-1.02f, -1.74f, 2.75f);
        camAddSpdSlowedDis_Small = new Vector3(0.8f, 0.5f, 0.3f);
        camAddSpdSlowedDis_Big = new Vector3(0.8f, 0.5f, 0.3f);

        tmpPlayrMoveSpd = playerMovement.GetMoveSpeed();
        playerMoveSpd_Aim = 1.2f;
        camMoveSpd_Aim = 8f;
        camMoveAddSpd_Aim = 4.5f;
        headMoveSpd_Aim = 4.5f;
        wholeBodyMoveSpd_Aim = 4.5f;
        isAim = false;

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
                    //playerCamTF.position = Vector3.Lerp(playerCamTF.position, playerTF.position - playerCamOffset, camMoveSpd_Aim * Time.deltaTime);
                    LookPos(playerTF, GetVecForScreenCenter(playerTF.position), camMoveSpd_Aim, 8);    //  ����L�����̑̂��J�������S���Ɍ�������
                    LookPos(playerHeadTF, GetVecForScreenCenter(playerHeadTF.position), wholeBodyMoveSpd_Aim, 1);    //  //  ����L�����̓����J�������S���Ɍ�������
                    break;
                }
            default: break;
        }
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