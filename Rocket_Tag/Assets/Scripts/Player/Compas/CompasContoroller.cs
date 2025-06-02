using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;                                                                          ////  プレイヤーを指し示すコンパスの管理script  ////

public class FollowUpCompas : MonoBehaviour
{
    public Transform[] playersTF;
    public Transform bombPlayerTF;
    Transform compas;
    GameManager gameManager;

    float trackSpd;
    int xRotCap;
    int tmpPlayerNum;

    void Start()                                                                            ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }
    void Update()
    {
        if (gameManager.playerNum != tmpPlayerNum)
        {
            LetfRoomProcces();
        }
        if (gameManager.playerNum > 1)
        {
            Vector3[] tmpPlayerPos = playersTF.Select(tf => tf.position).ToArray();
            ChangeRot(bombPlayerTF, compas, GetCloserObj(bombPlayerTF.position, tmpPlayerPos), trackSpd, xRotCap);
        }
    }                                                                                    ////  処理区終了  ////
    void Initialize()    //  初期化
    {
        bombPlayerTF = this.transform.root;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playersTF = gameManager.GetPlayerList().ConvertAll(x => x.transform).ToArray();
        compas = this.transform;

        trackSpd = 11f;
        xRotCap = 36;
        tmpPlayerNum = gameManager.playerNum;
    }
    void ChangeRot(Transform watcher, Transform changedObj, Vector3 target, float trackSpd, int xRotCap)    //  オブジェクトをターゲットの方向に向ける
    {
        Quaternion tmpAngle = Quaternion.LookRotation(target - watcher.position);
        tmpAngle.x *= 0;
        tmpAngle.z *= 0;
        changedObj.rotation = Quaternion.Lerp(changedObj.rotation, tmpAngle, trackSpd * Time.deltaTime);
    }
    Vector3 GetCloserObj(Vector3 axis, Vector3[] poss)    //  最も近いプレイヤーのトランスフォームを取得する
    {
        float tmpLineDis;
        float minLineDis;
        int closestPlayerNo = poss.Length - 1;

        minLineDis = 200;

        for (int arrayNum = poss.Length - 1; arrayNum != -1; arrayNum--)
        {
            if (poss[arrayNum] != axis)
            {
                tmpLineDis = Vector3.Distance(axis, poss[arrayNum]);   //  order is wired---------------------
                if (minLineDis > tmpLineDis)
                {
                    minLineDis = tmpLineDis;
                    closestPlayerNo = arrayNum;
                }
            }
        }
        return poss[closestPlayerNo];
    }
    void LetfRoomProcces()
    {
        playersTF = gameManager.GetPlayerList().ConvertAll(x => x.transform).ToArray();
        tmpPlayerNum = gameManager.playerNum;
    }
    public void SetPlayerTF()    //  プレイヤートランスフォーム取得
    {
        playersTF = gameManager.GetPlayerList().ConvertAll(x => x.transform).ToArray();
    }
}