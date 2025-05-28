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
    // スキン生成の種類を定義する列挙型
    internal enum SkinGenerateProcces
    {
        IN_GAME_GENERATE,
    }

    static GameObject[] playerSkinPrefab; // ロードされたスキンプレハブを格納
    GameObject skinEntityHead;
    Transform playerTF;
    const string inGameSceneName = "PlayScene";
    const string skinLoadError = "Error:Skin is didn't load";
    const string playerTFLoadError = "Error:Player's hip doesn't exist";
    const string playerTFIsAssignedThis = "PlayerTF is Assigned [This.transform]";
    const string gameMngNotFound = "Error: GameManager didn't find";
    const string couldntGetPlayerList = "Error: PlayerList couldn't get";
    const string playerPrefasUnexpectedValue = "Playerprefas value is strange";
    const string scriptProssesFinish = "SkinGenerater.cs's process is stop";
    static bool isSkinLoaded = false;

    static internal GameObject[] _SkinPrefab { get { return playerSkinPrefab; } }

    void Start()
    {
        // ゲームシーンでなければ初期化
        if (SceneManager.GetActiveScene().name != inGameSceneName)
        {
            Initialize();
        }
    }

    async void Initialize()
    {
        // スキンが未ロードならロード
        if (playerSkinPrefab == null)
        {
            await PlayerSkinLord();
        }

        // プレイヤーのTransform取得
        playerTF = GameObject.Find("Player")?.transform;
        if (IsNull_Variable(playerTF, false, playerTFLoadError))
        {
            Debug.Log(playerTFIsAssignedThis);
            playerTF = this.transform;
        }

        // スキンロード失敗チェック
        if (IsNull_Array(playerSkinPrefab, false, null, false, skinLoadError, null))
        {
            Debug.Log(scriptProssesFinish);
            return;
        }

        // 自分のプレイヤーならスキン生成
        if (photonView.IsMine)
        {
            SkinGenerate(playerTF);
        }
    }

    internal async void SkinGenerateWrapper(SkinGenerateProcces skinGenerateProcces)
    {
        // スキンロード完了まで待機
        while (!isSkinLoaded)
        {
            await Task.Delay(100);
        }

        switch (skinGenerateProcces)
        {
            case SkinGenerateProcces.IN_GAME_GENERATE:
                InGameGenerate();
                break;
        }
    }

    void InGameGenerate()
    {
        // GameManager取得
        GameManager gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
        if (IsNull_Variable(gameManager, false, gameMngNotFound)) return;

        // プレイヤーリスト取得
        GameObject[] tmpPlayerList = gameManager.GetPlayerList().ToArray();
        if (IsNull_Array(tmpPlayerList, false, null, false, null, couldntGetPlayerList)) return;

        // 各プレイヤーにスキン生成
        foreach (var player in tmpPlayerList)
        {
            SkinGenerate(player.transform);
        }
    }

    void SkinGenerate(Transform playerTF_)
    {
        // PlayerPrefsからスキン情報取得
        int tmpSkinNo = PlayerPrefs.GetInt("PlayerSkinNo", -1);
        int tmpSkinLocation = PlayerPrefs.GetInt("PlayerSkinLocation", -1);

        if (IsUnexpectedValue(new int[] { tmpSkinNo, tmpSkinLocation }, new int[] { -1, -1 }))
        {
            tmpSkinNo = 0;
            tmpSkinLocation = 0;
            Debug.Log(playerPrefasUnexpectedValue);
        }

        // カスタムプロパティにスキン情報を設定
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "SkinNo", tmpSkinNo },
                { "SkinLocation", tmpSkinLocation }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        // 自分のスキンを生成
        LoadAndAttachSkin(tmpSkinNo, tmpSkinLocation, playerTF_);
    }

    async void LoadAndAttachSkin(int skinNo, int location, Transform parent)
    {
        string[] skinNames = new string[] { "NotWearing", "RedCap", "StrawHat", "Eringi", "Freeza", "Bear", "Star" };
        var handle = Addressables.LoadAssetAsync<GameObject>(skinNames[skinNo]);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject skin = Instantiate(handle.Result);
            if (location == 0)
            {
                Transform head = parent.Find("root/Hip/Spine/Head");
                if (head != null)
                {
                    skin.transform.SetParent(head, false);
                }
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // 他プレイヤーのスキン情報を受け取って生成
        if (changedProps.ContainsKey("SkinNo") && changedProps.ContainsKey("SkinLocation"))
        {
            int skinNo = (int)changedProps["SkinNo"];
            int location = (int)changedProps["SkinLocation"];
            GameObject playerObj = GetPlayerObject(targetPlayer);
            if (playerObj != null)
            {
                LoadAndAttachSkin(skinNo, location, playerObj.transform);
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

    bool IsUnexpectedValue(int[] value, int[] unExpectedValue)
    {
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == unExpectedValue[i])
                return true;
        }
        return false;
    }

    bool IsNull_Variable<T>(T value, bool haveToClach, string errorMsg)
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

    bool IsNull_Array<T>(T[] value, bool isCheckPoint, int[] checkPoint, bool haveToClach, string errorMsg_PointNull, string errorMsg_AllNull)
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

    async Task PlayerSkinLord()
    {
        const int numOfSkin = 7;
        string[] skinNames = new string[] { "NotWearing", "RedCap", "StrawHat", "Eringi", "Freeza", "Bear", "Star" };

        Task[] task = new Task[numOfSkin];
        playerSkinPrefab = new GameObject[numOfSkin];
        AsyncOperationHandle<GameObject>[] playerSkinLordHandle = new AsyncOperationHandle<GameObject>[numOfSkin];

        for (int i = 0; i < numOfSkin; i++)
        {
            playerSkinLordHandle[i] = Addressables.LoadAssetAsync<GameObject>(skinNames[i]);
            task[i] = playerSkinLordHandle[i].Task;
        }

        await Task.WhenAll(task);

        for (int i = 0; i < numOfSkin; i++)
        {
            playerSkinPrefab[i] = playerSkinLordHandle[i].Result;
            await Task.Yield();
        }

        isSkinLoaded = true;
    }
}
