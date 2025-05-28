
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
    // スキン生成のプロセスを定義する列挙型
    internal enum SkinGenerateProcces
    {
        IN_GAME_GENERATE,
    }

    // プレイヤースキンのプレハブ配列
    static GameObject[] playerSkinPrefab;
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
    static int skinLocation;
    static bool isSkinLoaded = false; // スキンがロードされたかどうかを示すフラグ

    static internal GameObject[] _SkinPrefab { get { return playerSkinPrefab; } }

    void Start()
    {
        // ゲームシーンでない場合は初期化を行う
        if (SceneManager.GetActiveScene().name != inGameSceneName)
        {
            Initialize();
        }
    }

    async void Initialize()
    {
        // スキンプレハブがロードされていない場合はロードを行う
        if (playerSkinPrefab == null)
        {
            await PlayerSkinLord();
        }

        // プレイヤーのTransformを取得
        playerTF = GameObject.Find("Player")?.transform;
        if (IsNull_Variable(playerTF, false, playerTFLoadError))
        {
            Debug.Log(playerTFIsAssignedThis);
            playerTF = this.transform;
        }

        // スキンプレハブが正しくロードされているか確認
        if (IsNull_Array(playerSkinPrefab, false, null, false, skinLoadError, null))
        {
            Debug.Log(scriptProssesFinish);
            return;
        }

        // 自分のプレイヤーの場合はスキンを生成
        if (photonView.IsMine)
        {
            SkinGenerate(playerTF);
        }
    }

    internal void SkinGenerateWrapper(SkinGenerateProcces skinGenerateProcces)
    {
        // スキン生成プロセスに応じて処理を分岐
        switch (skinGenerateProcces)
        {
            case SkinGenerateProcces.IN_GAME_GENERATE:
                InGameGenerate();
                break;
        }
    }

    void InGameGenerate()
    {
        // ゲームマネージャーを取得
        GameManager gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
        if (IsNull_Variable(gameManager, false, gameMngNotFound)) return;

        // プレイヤーリストを取得
        GameObject[] tmpPlayerList = gameManager.GetPlayerList().ToArray();
        if (IsNull_Array(tmpPlayerList, false, null, false, null, couldntGetPlayerList)) return;

        // 各プレイヤーに対してスキンを生成
        foreach (var player in tmpPlayerList)
        {
            SkinGenerate(player.transform);
        }
    }

    void SkinGenerate(Transform playerTF_)
    {
        // プレイヤーのスキン番号と位置を取得
        int tmpSkinNo = PlayerPrefs.GetInt("PlayerSkinNo", -1);
        int tmpSkinLocation = PlayerPrefs.GetInt("PlayerSkinLocation", -1);

        // スキン番号と位置が不正な場合はデフォルト値を設定
        if (IsUnexpectedValue(new int[] { tmpSkinNo, tmpSkinLocation }, new int[] { -1, -1 }))
        {
            tmpSkinNo = 0;
            tmpSkinLocation = 0;
            Debug.Log(playerPrefasUnexpectedValue);
        }

        // スキンがロードされていない場合はスキップ
        if (!isSkinLoaded)
        {
            Debug.LogWarning("Skin not loaded yet. Skipping instantiation.");
            return;
        }

        // スキン番号と位置をPhotonのカスタムプロパティに設定
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "SkinNo", tmpSkinNo },
                { "SkinLocation", tmpSkinLocation }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        // スキンをインスタンス化
        InstantiateSkin(tmpSkinNo, tmpSkinLocation, playerTF_);
    }

    void InstantiateSkin(int skinNo, int location, Transform parent)
    {
        // スキンプレハブが正しくロードされているか確認
        if (playerSkinPrefab == null || skinNo < 0 || skinNo >= playerSkinPrefab.Length || playerSkinPrefab[skinNo] == null)
        {
            Debug.LogError("Invalid skin prefab or index.");
            return;
        }

        // スキンの位置が0の場合は頭にスキンを生成
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
        // プレイヤーのプロパティが更新された場合の処理
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
        // プレイヤーオブジェクトを取得
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
        // 値が予期しないものであるかどうかを確認
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == unExpectedValue[i])
                return true;
        }
        return false;
    }

    bool IsNull_Variable<T>(T value, bool haveToClach, string errorMsg)
    {
        // 変数がnullであるかどうかを確認
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
        // 配列がnullまたは空であるかどうかを確認
        if (value == null || value.Length == 0)
        {
            if (haveToClach)
            {
                Environment.FailFast(errorMsg_AllNull);
            }
            Debug.Log(errorMsg_AllNull);
            return true;
        }

        // 特定のインデックスがnullであるかどうかを確認
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
        // プレイヤースキンをロードする
        const int numOfSkin = 7;
        string[] skinNames = new string[] { "NotWearing", "RedCap", "StrawHat", "Eringi", "Freeza", "Bear", "Star" };

        Task[] task = new Task[numOfSkin];
        playerSkinPrefab = new GameObject[numOfSkin];
        AsyncOperationHandle<GameObject>[] playerSkinLordHandle = new AsyncOperationHandle<GameObject>[numOfSkin];

        // 各スキンを非同期でロード
        for (int i = 0; i < numOfSkin; i++)
        {
            playerSkinLordHandle[i] = Addressables.LoadAssetAsync<GameObject>(skinNames[i]);
            task[i] = playerSkinLordHandle[i].Task;
        }

        // すべてのスキンがロードされるまで待機
        await Task.WhenAll(task);

        // ロードされたスキンを配列に格納
        for (int i = 0; i < numOfSkin; i++)
        {
            playerSkinPrefab[i] = playerSkinLordHandle[i].Result;
            await Task.Yield();
        }

        // スキンがロードされたことを示すフラグを立てる
        isSkinLoaded = true;
    }
}
