using System;
using System.Collections.Generic;                                                          ////  スキン生成スクリプト  ////
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SkinGenerater : MonoBehaviour
{
    /*  スキンの種類等はPlayerSkin.csにenumで宣言してあります  */                          ////  以下宣言区  ////
    internal enum SkinGenerateProcces    //  スキンジェネレート内の処理一覧
    {
        IN_GAME_GENERATE,
    }
    
    static GameObject[] skinPrefab;    //  [0]はなにも着ていない状態を表現するために使っています
    GameObject skinEntity;
    Transform playerHipTF;

    static int skinLocation;

    static internal GameObject[] _SkinPrefab
    { get { return skinPrefab; } }                                                         ////  宣言区終了  ////

    void Start()                                                                           ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }                                                                                      ////  処理区終了  ////
    void Initialize()     //  初期化                                                       ////  以下関数区  ////
    {
        if (skinPrefab == null)
        {
            ResourceLord();
        }
        playerHipTF = GameObject.Find("Hip").GetComponent<Transform>();

        IsNull_Array(skinPrefab, false, null, false, "Error:Skin is didn't Load", null);    //  Mesaage for Debug---------------------
        IsNull_Variable(playerHipTF, false, "Error:Player's hip doesn't exist");    //  Mesaage for Debug------------------------------

        SkinGenerate(playerHipTF);
    }
    internal void SkinGenerateWrapper(SkinGenerateProcces skinGenerateProcces)   // ロケットエフェクトのラッパー関数
    {
        switch (skinGenerateProcces)
        {
            case SkinGenerateProcces.IN_GAME_GENERATE:    //  インゲームスキン生成処理群
                {
                    InGameGenerate();
                    break;
                }
        }
    }
    void InGameGenerate()    //  インゲームのスキン生成処理
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject[] tmpPlayerList = gameManager.GetPlayerList().ToArray();

        IsNull_Variable(gameManager, false, "Error: GameManager didn't Find");    //  Message for debug---------------------------
        IsNull_Array(tmpPlayerList, false, null, false, null, "Error: PlayerList Couldn't List");  //  Message for debug------------------

        for (int tmpPlayerListLen = 0; tmpPlayerListLen < tmpPlayerList.Length; tmpPlayerListLen++)
        {
            SkinGenerate(tmpPlayerList[tmpPlayerListLen].transform);
        }
    }
    void SkinGenerate(Transform playerHipTF_)    //  プレイヤーのスキンの生成
    {
        int tmpSkinNo = PlayerPrefs.GetInt("PlayerSkinNo", -1);
        int tmpSkinLocation = PlayerPrefs.GetInt("PlayerSkinLocation", -1);

        if (!(IsUnexpectedValue(new int[] {tmpSkinNo, tmpSkinLocation }, new int[] {-1, -1})))
        {
            tmpSkinNo = 0;
            tmpSkinLocation = 0;
            Debug.Log("Error: PlayerPrefs is unable");    //  debug--------------
        }
        if (tmpSkinNo != 0)
        {
            switch (tmpSkinLocation)
            {
                case 0:
                    {
                        skinEntity = Instantiate(skinPrefab[tmpSkinNo], playerHipTF.Find("Spine/Head"));
                        break;
                    }
            }
        }
    }
    bool IsUnexpectedValue(int[] value, int[] unExpectedValue)    //  値チェック
    {
        for (int arrayNo = value.Length; arrayNo > 0; --arrayNo)
        {
            if (value[arrayNo] == unExpectedValue[arrayNo])
            {
                return true;
            }
        }
        return false;
    }
    bool IsNull_Array<T>     //  配列のヌルチェック、危険性があった場合強制クラッシュ
    (T[] value, bool isCheckPoint, int[] checkPoint, bool haveToClach, string errorMsg_PointNull, string errorMsg_AllNull)
    {
        if (value == null || value.Length == 0)
        {
            if (haveToClach)
            {
                Environment.FailFast(errorMsg_AllNull);    //  クラッシュ
            }
            Debug.Log(errorMsg_AllNull);    //  debug------------------
            return true;
        }
        if (isCheckPoint)
        {
            for (int arrayNo = checkPoint.Length; arrayNo > 0; --arrayNo)
            {
                if (value[checkPoint[arrayNo]] == null)
                {
                    if (haveToClach)
                    {
                        Environment.FailFast(errorMsg_PointNull);    //  クラッシュ
                    }
                    return true;
                }
                Debug.Log(errorMsg_PointNull);    //  debug-------------------
                return false;
            }
        }
        Debug.Log(errorMsg_PointNull);    //  debug--------------------------
        return false;
    }
    bool IsNull_Variable<T>(T value, bool haveToClach, string errorMsg)    //  変数のヌルチェック、危険性があった場合強制クラッシュ
    {
        if(value == null)
        {
            if(haveToClach)
            {
                Environment.FailFast(errorMsg);    //  クラッシュ
            }
            return true;
        }
        Debug.Log(errorMsg);    //  debug--------------------------
        return false;
    }
    void ResourceLord()    //  Resourceフォルダ内のファイルを読み込む
    {
        skinPrefab = new GameObject[7];
        skinPrefab[1] = Resources.Load<GameObject>("RedCap");
        skinPrefab[2] = Resources.Load<GameObject>("StrawHat");
        skinPrefab[3] = Resources.Load<GameObject>("Eringi");
        skinPrefab[4] = Resources.Load<GameObject>("Freeza");
        skinPrefab[5] = Resources.Load<GameObject>("Bear");
        skinPrefab[6] = Resources.Load<GameObject>("Star");
    }                                                                                      ////  関数区終了  ////
}
/*                                                                                         ////  以下コード保存  ////
     bool IsUnexpectedValue    //  値チェック
    (bool isCompare, bool isCheckRange, bool isCheckBigger, int[] value, int[] unExpectedValue, int[] expectedValue_Bigger)
    {
        if(isCompare)
        {
            if (isCheckRange)
            {
                for (int arrayNo = value.Length; arrayNo > 0; --arrayNo)
                {
                    if(!(unExpectedValue[arrayNo] < value[arrayNo] && value[arrayNo] < expectedValue_Bigger[arrayNo]))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (isCheckBigger)
            {
                for(int arrayNo = value.Length; arrayNo > 0; --arrayNo)
                {
                    if(value[arrayNo] < unExpectedValue[arrayNo])
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        if (value[0] == unExpectedValue[0])
        {
            return true;
        }
        return false;
    }
 */