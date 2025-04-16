using Unity.VisualScripting;
using UnityEngine;                                                                          ////  プレイヤーを指し示すコンパスの管理script  ////

public class FollowUpCompas : MonoBehaviour
{
    
    public Transform[] playersTF;
    Transform bombPlayerTF;

    const int numOfPlayer = 4;

    void Start()
    {
        
    }
    void Update()
    {

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
