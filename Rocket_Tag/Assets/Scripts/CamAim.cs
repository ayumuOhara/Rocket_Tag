using Photon.Realtime;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

internal interface CamState    //  �t�b�N��ԃC���^�[�t�F�[�X                            ////  �ȉ�State��  ////
{
    void Enter(CamAim arg);
    void Update(CamAim arg);
    void Exit(CamAim arg);
}
internal class NotAiming : CamState    //  �_�����i���Ă��Ȃ����
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
internal class Aiming : CamState    //  �_�����i���Ă�����
{
    public void Enter(CamAim camAim)
    {

    }
    public void Update(CamAim camAim)
    {
        camAim.CamWrapper(CamAim.CamProcess.AIMING);
        camAim.ChangeState(new NotAiming());
    }
    public void Exit(CamAim camAim)
    {

    }
}                                                                                        ////  State��I��  ////
public class CamAim : MonoBehaviour
{
    internal enum CamProcess    //  �J�����̋����ꗗ                                     ////  �ȉ��錾��  ////
    {
        AIMING,
        TRACK_PLAYER,
    }

    CamState currentState;

    GameObject player;
    Camera playerCam;
    Transform playerTF;
    Transform playerCamTF;
    Quaternion playerStandRot;

    Vector3 tmpCamOffset;
    Vector3 camOffset = new Vector3(-2.24f, -2.85f, 4.25f);
    Vector3 camAddSpdSlowedDis_Small = new Vector3(0.8f, 0.5f, 0.3f);
    Vector3 camAddSpdSlowedDis_Big = new Vector3(0.8f, 0.5f, 0.3f);
    Vector3 currentAngle;

    float camMoveSpd_Aim = 8f;
    float camMoveAddSpd_Aim = 4.5f;
    bool isAim = false;                                                                  ////  �錾��I��  ////
    void Start()
    {
        Initialize();
    }

    void Update()
    {
        currentState.Update(this);
        //{
        //    StraightMoveToPos(playerCam.transform, playerTF.position + camOffset, camMoveSpd_Aim, camMoveAddSpd_Aim);
        //    if (playerTF.rotation.y != playerCamTF.rotation.y)
        //    {
        //        //playerCamTF.rotation.y = 40 - Time.deltaTime;
        //        //playerTF.rotation = Quaternion.Euler(playerTF.rotation.x, playerCamTF.rotation.y , playerTF.rotation.z);
        //        playerTF.rotation = Quaternion.Euler(currentAngle.x, currentAngle.y + 20f, currentAngle.z);
        //    }
        //}
        //else
        //{
        //    StraightMoveToPos(playerCam.transform, playerTF.position - tmpCamOffset, camMoveSpd_Aim, camMoveAddSpd_Aim);
        //}

        //if(Input.GetMouseButton(0))
        //{
        //    Debug.Log(tmpCamOffset);
        //    StraightMoveToPos(playerCam.transform, playerTF.position - tmpCamOffset, camMoveSpd_Aim,camMoveAddSpd_Aim);
        //}
    }
    void Initialize()    //  ������                                                       ////  �ȉ��֐���  ////
    {
        player = this.gameObject;
        playerCam = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        playerTF = player.transform;
        playerCamTF = playerCam.transform;
        tmpCamOffset = player.transform.position - playerCamTF.position;
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
        currentState.Enter(this);
    }
    internal void CamWrapper(CamProcess camProcess)   // �J�����̋����̃��b�p�[�֐�
    {
        switch (camProcess)
        {
            case CamProcess.AIMING:
                {
                    Vector3 test = new Vector3(1, 1, 1);
                    LookAtPos(playerTF, test);
                    //Debug.Log("anglechange");
                    //Debug.Log(GetVecForScreenCenter(playerTF.position));
                    //LookAtPos(playerTF, GetVecForScreenCenter(playerTF.position));
                    ////   playerTF.rotation = Quaternion.Euler(playerTF.rotation.x + playerStandRot.x,playerTF.rotation.y + playerStandRot.y  ,playerTF.rotation.z);
                    //playerTF.rotation = Quaternion.Euler(playerStandRot.x, playerTF.rotation.y, playerStandRot.z);
                    break;
                }
            default: break;
        }
    }
    void ObjRotChange()
    {

    }
    void LookAtPos(Transform obj, Vector3 pos)    //  �w�肵�����W�Ɍ�������
    {
        obj.LookAt(pos);
    }
    Vector3 GetVecForScreenCenter(Vector3 perspective)    //  prespective����J�������S�ւ̃��F�N�g�����Ƃ�
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 100);
        Debug.Log(screenCenter);
        return playerCam.ScreenToWorldPoint(screenCenter);
    }
}