using Photon.Pun;
using System;                                                                              ////  スキン生成スクリプト  ////
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
public class SkinGenerater : MonoBehaviour
{
    /*  スキンの種類等はPlayerSkin.csにenumで宣言してあります  */                          ////  以下宣言区  ////
    internal enum SkinGenerateProcces    //  スキンジェネレート内の処理一覧
    {
        IN_GAME_GENERATE,
    }
    
    static GameObject[] playerSkinPrefab;    //  [0]はなにも着ていない状態を表現するために使っています
    GameObject skinEntityHead;
    Transform playerTF;

    const string inGameSceneName = "PlayScene";
    const string skinLoadError = "Error:Skin is didn't load";    //  msg for debug--------------
    const string playerTFLoadError = "Error:Player's hip doesn't exist";    //  msg for debug---------------------
    const string playerTFIsAssignedThis = "PlayerTF is Assigned [This.transform]";    //  msg for debug---------------------
    const string gameMngNotFound = "Error: GameManager didn't find";    //  msg for debug------------------
    const string couldntGetPlayerList = "Error: PlayerList couldn't get";    //  msg for debug------------------
    const string playerPrefasUnexpectedValue = "Playerprefas value is strange";    //  msg for debug------------------
    const string scriptProssesFinish = "SkinGenerater.cs's process is stop";    //  msg for debug------------------

    static int skinLocation;

    static internal GameObject[] _SkinPrefab
    { get { return playerSkinPrefab; } }                                                   ////  宣言区終了  ////

    void Start()                                                                           ////  以下処理区  ////
    {
        if (SceneManager.GetActiveScene().name != inGameSceneName)
        {
            Initialize();    //  初期化
        }
    }                                                                                      ////  処理区終了  ////
async void Initialize()     //  初期化                                                 ////  以下関数区  ////
    {
        if (playerSkinPrefab == null)
        {
            await PlayerSkinLord();
        }
        playerTF = GameObject.Find("Player").GetComponent<Transform>();
        if(IsNull_Variable(playerTF, false, playerTFLoadError))    //  msg for debug------------------------------
        {
            playerTF = this.transform;   
        }
        if (IsNull_Array(playerSkinPrefab, false, null, false, skinLoadError, null))    //  msg for debug---------------------
        {
            return;
        }
        SkinGenerate(playerTF);
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
    void InGameGenerate()    //  インゲームのスキン生成処理(プレイヤーがインゲームに生成されたタイミングで呼び出される)
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject[] tmpPlayerList = gameManager.GetPlayerList().ToArray();

        IsNull_Variable(gameManager, false, gameMngNotFound);    //  Msg for debug---------------------------
        IsNull_Array(tmpPlayerList, false, null, false, null, couldntGetPlayerList);  //  Msg for debug------------------

        for (int tmpPlayerListLen = 0; tmpPlayerListLen < tmpPlayerList.Length; tmpPlayerListLen++)
        {
            SkinGenerate(tmpPlayerList[tmpPlayerListLen].gameObject.transform);
        }
    }
    void SkinGenerate(Transform playerTF_)    //  プレイヤーのスキンの生成
    {
        int tmpSkinNo = PlayerPrefs.GetInt("PlayerSkinNo", -1);
        int tmpSkinLocation = PlayerPrefs.GetInt("PlayerSkinLocation", -1);

        if (IsUnexpectedValue(new int[] {tmpSkinNo, tmpSkinLocation }, new int[] {-1, -1}))
        {
            tmpSkinNo = 0;
            tmpSkinLocation = 0;
        }
        if (tmpSkinNo != 0)
        {
            switch (tmpSkinLocation)
            {
                case 0:
                    {
                        skinEntityHead = Instantiate(playerSkinPrefab[tmpSkinNo], playerTF_.Find("root/Hip/Spine/Head"));
                        break;
                    }
            }
        }
    }
    bool IsUnexpectedValue(int[] value, int[] unExpectedValue)    //  値チェック
    {
        for (int arrayNo = value.Length - 1; arrayNo >= 0; --arrayNo)
        {
            if (value[arrayNo] == unExpectedValue[arrayNo])
            {
                return true;
            }
        }
        return false;
    }
    bool IsNull_Variable<T>(T value, bool haveToClach, string errorMsg)    //  変数のヌルチェック、危険性があった場合強制クラッシュ
    {
        if (value == null)
        {
            if (haveToClach)
            {
                Environment.FailFast(errorMsg);    //  クラッシュ
            }
            return true;
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
            return true;
        }
        if (isCheckPoint)
        {
            for (int arrayNo = checkPoint.Length - 1; arrayNo >= 0; arrayNo--)
            {
                if (value[checkPoint[arrayNo]] == null)
                {
                    if (haveToClach)
                    {
                        Environment.FailFast(errorMsg_PointNull);    //  クラッシュ
                    }
                    return true;
                }
                return false;
            }
        }
        return false;
    }
    async Task PlayerSkinLord()     //  プレイヤースキン読み込み
    {
        Task[] task;

        AsyncOperationHandle<GameObject>[] playerSkinLordHandle;

        const int numOfSkin = 7;
        string[] skinNames = new string[] { "NotWearing", "RedCap", "StrawHat", "Eringi", "Freeza", "Bear", "Star" };

        task = new Task[numOfSkin - 1];
        playerSkinPrefab = new GameObject[numOfSkin];

        playerSkinLordHandle = new AsyncOperationHandle<GameObject>[numOfSkin];

        /*  スキンは永久的に使うので開放していない  */
        for (int arrayNo = numOfSkin - 1; arrayNo > 0; arrayNo--)
        {
            playerSkinLordHandle[arrayNo] = Addressables.LoadAssetAsync<GameObject>(skinNames[arrayNo]);
            task[arrayNo - 1] = playerSkinLordHandle[arrayNo].Task;
        }
        await Task.WhenAll(task);
        for (int arrayNo = numOfSkin - 1; arrayNo > 0; arrayNo--)
        {
            playerSkinPrefab[arrayNo] = playerSkinLordHandle[arrayNo].Result;
            await Task.Yield();
        }
    }                                                                                      ////  関数区終了  ////
}