using Unity.VisualScripting;
using UnityEngine;                                                                          ////  プレイヤーを指し示すコンパスの管理script  ////

public class FollowUpCompas : MonoBehaviour
{
    
    public Transform[] playersTF;
    public Transform bombPlayerTF;

    const int numOfPlayer = 4;

    void Start()
    {
        //bombPlayerTF = this.gameObject.GetComponent<Transform>();
    }
    void Update()
    {
        //this.gameObject.transform.LookAt(GetCloserPlayer());
        Quaternion tmpAngle = Quaternion.LookRotation(GetCloserPlayer() - bombPlayerTF.position);
        tmpAngle.x /= 36;
        this.gameObject.transform.rotation = Quaternion.Lerp(bombPlayerTF.rotation, tmpAngle, 3f * Time.deltaTime);
        //this.gameObject.transform.LookAt(GetCloserObj());
    }
    Vector3 GetCloserPlayer()    //  最も近いプレイヤーのトランスフォームを取得する
    {
        float tmpLineDis;
        float minLineDis;
        int closestPlayerNo = numOfPlayer - 1;
        
        tmpLineDis = Vector3.Distance(playersTF[closestPlayerNo].position, bombPlayerTF.position);
        minLineDis = tmpLineDis;

        for (int arrayNum = numOfPlayer - 2; arrayNum != -1; arrayNum--)
        {

            tmpLineDis = Vector3.Distance(playersTF[arrayNum].position, bombPlayerTF.position);
            if (minLineDis > tmpLineDis)
            {
                minLineDis = tmpLineDis;
                closestPlayerNo = arrayNum;
            }
        }
        return playersTF[closestPlayerNo].position;
    }
    Vector3 GetCloserObj(Vector3 axis, Vector3[] objArray)    //  最も近いオブジェクトのポジションを取得する
    {
        float tmpLineDis;
        float minLineDis;
        int closestObjNo = 3;

        tmpLineDis = Vector3.Distance(objArray[closestObjNo], axis);
        minLineDis = tmpLineDis;

        for (int arrayNum = numOfPlayer - 2; arrayNum != -1; arrayNum--)
        {
            tmpLineDis = Vector3.Distance(objArray[arrayNum], axis);
            if (minLineDis > tmpLineDis)
            {
                minLineDis = tmpLineDis;
                closestObjNo = arrayNum;
            }
        }
        return playersTF[closestObjNo].position;
    }
    //Transform GetCloserPlayer()    //  最も近いプレイヤーのトランスフォームを取得する
    //{
    //    float tmpLineDis;

    //    tmpLineDis = Vector3.Distance(playersTF[numOfPlayer]. position,bombPlayerTF.position);

    //    for (int arrayNum = numOfPlayer; arrayNum == 0; arrayNum--)
    //    {
    //        if (playersTF[numOfPlayer] != bombPlayerTF)
    //        {
    //        }
    //    }
    //    // return
    //}
    void GetLineDis()    //  二点間の直線の長さを求める
    {

    }

}
