using Unity.VisualScripting;
using UnityEngine;                                                                          ////  プレイヤーを指し示すコンパスの管理script  ////

public class FollowUpCompas : MonoBehaviour
{
    public Transform[] playersTF;
    public Transform bombPlayerTF;
    GameManager gameManager;
    
    const int numOfPlayer = 4;

    void Start()
    {
        Initialize();
    }
    void Update()
    {
        Quaternion tmpAngle = Quaternion.LookRotation(GetCloserPlayer() - bombPlayerTF.position);
        Quaternion tmpAngle1;
        tmpAngle.x /= 36;
        this.gameObject.transform.rotation = Quaternion.Lerp(bombPlayerTF.rotation, tmpAngle, 50f * Time.deltaTime);
        //tmpAngle1 = this.gameObject.transform.rotation;
        //tmpAngle1.x /= 36;
    }
    void Initialize()    //  初期化
    {
        bombPlayerTF = this.transform.root;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playersTF = gameManager.GetPlayerList().ConvertAll(x => x.transform).ToArray();
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
}
