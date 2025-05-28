
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class SkinGenerater_PUN2 : MonoBehaviourPunCallbacks
{
    /*  スキンの種類等はPlayerSkin.csにenumで宣言してあります  */                          ////  以下宣言区  ////
    internal enum SkinGenerateProcces    //  スキンジェネレート内の処理一覧
    {
        IN_GAME_GENERATE,
    }

    static GameObject[] playerSkinPrefab;   //  [0]はなにも着ていない状態を表現するために使っています
    GameObject skinEntityHead;
    Transform playerTF;
    const string inGameSceneName = "PlayScene";                                     //  msg for debug--------------
    const string skinLoadError = "Error:Skin is didn't load";                       //  msg for debug--------------
    const string playerTFLoadError = "Error:Player's hip doesn't exist";            //  msg for debug--------------
    const string playerTFIsAssignedThis = "PlayerTF is Assigned [This.transform]";  //  msg for debug--------------
    const string gameMngNotFound = "Error: GameManager didn't find";                //  msg for debug--------------
    const string couldntGetPlayerList = "Error: PlayerList couldn't get";           //  msg for debug--------------
    const string playerPrefasUnexpectedValue = "Playerprefas value is strange";     //  msg for debug--------------
    const string scriptProssesFinish = "SkinGenerater.cs's process is stop";        //  msg for debug--------------
    static int skinLocation;

    static internal GameObject[] _SkinPrefab { get { return playerSkinPrefab; } }   ////  宣言区終了  ////

    void Start()                                                                    ////  以下処理区  ////
    {
        if (SceneManager.GetActiveScene().name != inGameSceneName)
        {
            Initialize();
        }
    }                                                                               ////  処理区終了  ////

    async void Initialize()                                                         ////  以下関数区  ////
    {
        if (playerSkinPrefab == null)
        {
            await PlayerSkinLord();
        }

        playerTF = GameObject.Find("Player")?.transform;
        if (IsNull_Variable(playerTF, false, playerTFLoadError))                        //  msg for debug------------------------------
        {
            Debug.Log(playerTFIsAssignedThis);                                          //  msg for debug------------------------------
            playerTF = this.transform;
        }

        if (IsNull_Array(playerSkinPrefab, false, null, false, skinLoadError, null))    //  msg for debug------------------------------
        {
            Debug.Log(scriptProssesFinish);                                             //  msg for debug------------------------------
            return;
        }

        if (photonView.IsMine)
        {
            SkinGenerate(playerTF);
        }
    }

    internal void SkinGenerateWrapper(SkinGenerateProcces skinGenerateProcces)  // ロケットエフェクトのラッパー関数
    {
        switch (skinGenerateProcces)
        {
            case SkinGenerateProcces.IN_GAME_GENERATE:      //  インゲームスキン生成処理群
                InGameGenerate();
                break;
        }
    }

    void InGameGenerate()   //  インゲームのスキン生成処理(プレイヤーがインゲームに生成されたタイミングで呼び出される)
    {
        GameManager gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
        if (IsNull_Variable(gameManager, false, gameMngNotFound)) return;

        GameObject[] tmpPlayerList = gameManager.GetPlayerList().ToArray();
        if (IsNull_Array(tmpPlayerList, false, null, false, null, couldntGetPlayerList)) return;

        foreach (var player in tmpPlayerList)
        {
            SkinGenerate(player.transform);
        }
    }

    void SkinGenerate(Transform playerTF_)      //  プレイヤーのスキンの生成
    {
        int tmpSkinNo = PlayerPrefs.GetInt("PlayerSkinNo", -1);
        int tmpSkinLocation = PlayerPrefs.GetInt("PlayerSkinLocation", -1);

        if (IsUnexpectedValue(new int[] { tmpSkinNo, tmpSkinLocation }, new int[] { -1, -1 }))
        {
            tmpSkinNo = 0;
            tmpSkinLocation = 0;
            Debug.Log(playerPrefasUnexpectedValue);
        }

        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "SkinNo", tmpSkinNo },
                { "SkinLocation", tmpSkinLocation }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        InstantiateSkin(tmpSkinNo, tmpSkinLocation, playerTF_);
    }

    void InstantiateSkin(int skinNo, int location, Transform parent)
    {
        if (playerSkinPrefab == null || skinNo < 0 || skinNo >= playerSkinPrefab.Length || playerSkinPrefab[skinNo] == null)
        {
            Debug.LogError("Invalid skin prefab or index.");
            return;
        }

        if (location == 0)
        {
            Transform head = parent.Find("root/Hip/Spine/Head");
            if (head == null)
            {
                Debug.LogError("Head transform not found in player hierarchy.");
                return;
            }

            GameObject skin = PhotonNetwork.Instantiate(playerSkinPrefab[skinNo].name, Vector3.zero, Quaternion.identity);
            skin.transform.SetParent(head, false);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("SkinNo") && changedProps.ContainsKey("SkinLocation"))
        {
            int skinNo = (int)changedProps["SkinNo"];
            int location = (int)changedProps["SkinLocation"];

            GameObject playerObj = GetPlayerObject(targetPlayer);
            if (playerObj != null && playerSkinPrefab != null && skinNo >= 0 && skinNo < playerSkinPrefab.Length && playerSkinPrefab[skinNo] != null)
            {
                InstantiateSkin(skinNo, location, playerObj.transform);
            }
            else
            {
                Debug.LogWarning("Skin instantiation skipped due to missing data.");
            }
        }
    }

    GameObject GetPlayerObject(Player player)
    {
        foreach (var view in FindObjectsOfType<PhotonView>())
        {
            if (view.Owner == player)
            {
                return view.gameObject;
            }
        }
        return null;
    }

    bool IsUnexpectedValue(int[] value, int[] unExpectedValue)      //  値チェック
    {
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == unExpectedValue[i])
                return true;
        }
        return false;
    }

    bool IsNull_Variable<T>(T value, bool haveToClach, string errorMsg)     //  変数のヌルチェック、危険性があった場合強制クラッシュ
    {
        if (value == null)
        {
            if (haveToClach)
            {
                Environment.FailFast(errorMsg);
            }
            Debug.Log(errorMsg);
            return true;
        }
        return false;
    }

    bool IsNull_Array<T>(T[] value, bool isCheckPoint, int[] checkPoint, bool haveToClach, string errorMsg_PointNull, string errorMsg_AllNull)      //  配列のヌルチェック、危険性があった場合強制クラッシュ
    {
        if (value == null || value.Length == 0)
        {
            if (haveToClach)
            {
                Environment.FailFast(errorMsg_AllNull);
            }
            Debug.Log(errorMsg_AllNull);
            return true;
        }

        if (isCheckPoint)
        {
            foreach (int index in checkPoint)
            {
                if (value[index] == null)
                {
                    if (haveToClach)
                    {
                        Environment.FailFast(errorMsg_PointNull);
                    }
                    Debug.Log(errorMsg_PointNull);
                    return true;
                }
            }
        }

        return false;
    }

    async Task PlayerSkinLord()     //  プレイヤースキン読み込み
    {
        const int numOfSkin = 7;
        string[] skinNames = new string[] { "NotWearing", "RedCap", "StrawHat", "Eringi", "Freeza", "Bear", "Star" };

        Task[] task = new Task[numOfSkin - 1];
        playerSkinPrefab = new GameObject[numOfSkin];
        AsyncOperationHandle<GameObject>[] playerSkinLordHandle = new AsyncOperationHandle<GameObject>[numOfSkin];

        for (int i = 1; i < numOfSkin; i++)
        {
            playerSkinLordHandle[i] = Addressables.LoadAssetAsync<GameObject>(skinNames[i]);
            task[i - 1] = playerSkinLordHandle[i].Task;
        }

        await Task.WhenAll(task);

        for (int i = 1; i < numOfSkin; i++)
        {
            playerSkinPrefab[i] = playerSkinLordHandle[i].Result;
            await Task.Yield();
        }
    }
}                                                                                   ////  関数区終了  ////

////  以下コード保存  ////
//bool IsUnexpectedValue    //  値チェック
//(bool isCompare, bool isCheckRange, bool isCheckBigger, int[] value, int[] unExpectedValue, int[] expectedValue_Bigger)
//{
//    if (isCompare)
//    {
//        if (isCheckRange)
//        {
//            for (int arrayNo = value.Length; arrayNo > 0; --arrayNo)
//            {
//                if (!(unExpectedValue[arrayNo] < value[arrayNo] && value[arrayNo] < expectedValue_Bigger[arrayNo]))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//        if (isCheckBigger)
//        {
//            for (int arrayNo = value.Length; arrayNo > 0; --arrayNo)
//            {
//                if (value[arrayNo] < unExpectedValue[arrayNo])
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//    }
//    if (value[0] == unExpectedValue[0])
//    {
//        return true;
//    }
//    return false;
//}
