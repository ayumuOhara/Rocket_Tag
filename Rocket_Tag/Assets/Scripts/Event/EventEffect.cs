using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Photon.Pun;

public class EventEffect : MonoBehaviourPunCallbacks                                            ////  イベントのエフェクトを扱うスクリプト(視界妨害を除く)  ////
{
    internal enum EventEffectProcess    //    エフェクト処理一覧                                ////  以下宣言区  ////
    {
        TELEPORT_SMOKE,
        MOVE_SPD_AURA,
        STOP_SPD_AURA
    }
    internal enum EffectName    //  エフェクトの名前一覧  
    {
        TELEPORT_SMOKE,
        SPD_UP_AURA,
        SPD_DOWN_AURA,
    }

    IGameMgrFactory Factory;

    Dictionary<EventEffectProcess, Action> effectProccesMap;
    Dictionary<EffectName, String> effectNameMap;
    Dictionary<EffectName, GameObject> loadedEffects;
    Action assignPlayerTF;
    GameObject teleportSmokeEntity;
    GameObject spdChagneAuraEntity;
    Transform[] players;
    ParticleSystem[] teleportSmokeSystem;
    ParticleSystem[] spdChagneAuraSystem;
    GameManager gameMgr;
    PlayerMovement[] playerMovement;

    const int numOfPlayers = 4;
    float defaultPlayerMoveSpd;
    bool isGeneratedSmoke;
    bool isGeneratedSpdAura;
    bool isGeneratedAllSpdChangeAura;
    const string loadFailMsg = "Load is failed";
    const string teleportSmokeNotGenerate = "Teleport smoke is not loading well";
    const string spdChangingFailMsg = "Spd has not changed";

    internal Action _AssignPlayerTF
    { get { return assignPlayerTF; }}
    internal bool _IsGeneratedSmoke
    { set { isGeneratedSmoke = value; } }                                                       ////  宣言区終了  ////

    void Start()                                                                                ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }                                                                                           ////  処理区終了  ////
    async void Initialize()    //  初期化関数                                                   ////  以下関数区  ////
    {
        IGameMgrFactory factory = new RealGameMgrFactory();
        
        SetDictionary();
        await LoadEffect();
        assignPlayerTF = AssignPlayersTF;
        teleportSmokeSystem = new ParticleSystem[numOfPlayers];
        spdChagneAuraSystem = new ParticleSystem[numOfPlayers];
        gameMgr = factory.CreateGameMgr();

        isGeneratedSmoke = false;
        isGeneratedSpdAura = false;
    }
    void SetDictionary()    //  辞書型変数をセッティング
    {
        effectProccesMap = new Dictionary<EventEffectProcess, Action>
        {
            { EventEffectProcess.TELEPORT_SMOKE, HandleTeleportSmoke},
            { EventEffectProcess.MOVE_SPD_AURA, ApplySpdBuffEffect},
            { EventEffectProcess.STOP_SPD_AURA, StopSpdChangeAura}
        };
        effectNameMap = new Dictionary<EffectName, String>()
        {
            {EffectName.TELEPORT_SMOKE, "TeleportSmoke" },
            {EffectName.SPD_UP_AURA, "SpdUpAura" },
            {EffectName.SPD_DOWN_AURA, "SpdDownAura" }
        };
        loadedEffects = new Dictionary<EffectName, GameObject>()
        {
            {EffectName.TELEPORT_SMOKE,null },
            {EffectName.SPD_UP_AURA,   null },
            {EffectName.SPD_DOWN_AURA, null }
        };
    }
    async Task LoadEffect()    //  エフェクトロード
    {
        List<Task> loadTask;

        Dictionary<EffectName, AsyncOperationHandle<GameObject>> loadHandle = new Dictionary<EffectName, AsyncOperationHandle<GameObject>>
        {
            {EffectName.TELEPORT_SMOKE, default},
            {EffectName.SPD_UP_AURA, default},
            {EffectName.SPD_DOWN_AURA, default}
        };

        loadTask = new List<Task>();

        foreach (KeyValuePair<EffectName, String> kvp in effectNameMap)
        {
            KeyValuePair<EffectName, String> kvps = kvp;    //  必要か精査-----------------------------------
            
            loadHandle[kvp.Key] = Addressables.LoadAssetAsync<GameObject>(kvps.Value);
            loadTask.Add(loadHandle[kvps.Key].Task.ContinueWith(t =>    //  読み込みから代入までのタスクを追加する
            {
                if (loadHandle[kvps.Key].Status == AsyncOperationStatus.Succeeded)
                {
                    loadedEffects[kvps.Key] = loadHandle[kvps.Key].Result;
                }
                else
                {
                    Debug.LogWarning(loadFailMsg + kvps.Key);    //  デバッグ用--------------------------------------
                }
            }));
        }
        await Task.WhenAll(loadTask);
    }
    [PunRPC]
    void CallEffectProcces(EventEffectProcess eventEffectNo)    //  エフェクト処理呼び出し
    {
        effectProccesMap[eventEffectNo]();
    }
    void HandleTeleportSmoke()    //  テレポート時の煙の制御
    {
        if(isGeneratedSmoke)
        {
            for (int i = 0; i < numOfPlayers; i++)
            {
                if (players[i].gameObject.activeSelf)
                {
                    ReplayEffect(teleportSmokeSystem[i], players[i].position);
                }
            }
        }
        else if(loadedEffects.TryGetValue(EffectName.TELEPORT_SMOKE, out GameObject TeleportSmoke) && TeleportSmoke != null)
        {
            GenerateTeleportSmoke();
            isGeneratedSmoke = true;
        }
        else
        {
            Debug.LogWarning(teleportSmokeNotGenerate);
        }
    }
    void ReplayEffect(ParticleSystem effect, Vector3 replayPos)    //  エフェクト再再生
    {
        effect.Clear();
        if (effect.transform.parent == null)
        {
            effect.transform.position = replayPos;
        }
        else
        {
            effect.transform.localPosition = replayPos;
        }
        effect.Play();
    }
    void GenerateTeleportSmoke()    //  テレポートスモーク生成
    {
        AssignPlayersTF();

        for (int playerIndex = 0; playerIndex < numOfPlayers; playerIndex++)
        {
            if (players[playerIndex].gameObject.activeSelf)
            {
                teleportSmokeEntity = Instantiate(loadedEffects[EffectName.TELEPORT_SMOKE]);
                teleportSmokeEntity.transform.position = players[playerIndex].position;
                teleportSmokeSystem[playerIndex] = teleportSmokeEntity.GetComponent<ParticleSystem>();
            }
        }
    }
    void AssignPlayersTF()    //  プレイヤーのトランスフォームを取得
    {
        if (players == null)
        {
            players = gameMgr.GetPlayerList().ConvertAll(x => x.transform).ToArray();
        }
    }
    void ApplySpdBuffEffect()    //  運動能力変化エフェクト制御
    {
        GetPlayersMovement();

        if (isGeneratedSpdAura && isGeneratedAllSpdChangeAura)
        {
            for (int playerIndex = 0; playerIndex < numOfPlayers; playerIndex++)
            {
                ReplayEffect(spdChagneAuraSystem[playerIndex], Vector3.zero);
            }
        }
        else if(loadedEffects.TryGetValue(EffectName.TELEPORT_SMOKE, out GameObject TeleportSmoke) && TeleportSmoke != null)
        {
            GenerateSpdAura();
            isGeneratedSpdAura = true;
        }
    }
    void GetPlayersMovement()    //  プレイヤームーブメントインスタンス取得
    {
        AssignPlayersTF();
        if (playerMovement == null)
        {
            playerMovement = new PlayerMovement[numOfPlayers];
            for (int playerIndex = 0; playerIndex < numOfPlayers; playerIndex++)
            {
                playerMovement[playerIndex] = players[playerIndex].GetComponent<PlayerMovement>();    //  改善の余地あり?
            }
            defaultPlayerMoveSpd = playerMovement[0].GetDefaultMoveSpeed();    //  プレイヤーのデフォルト移動速度取得 
        }
    }
    void GenerateSpdAura()    //  スピードを判定して速度エフェクトを出す
    {
        isGeneratedAllSpdChangeAura = true;
        for (int playerIndex = 0; playerIndex < numOfPlayers; playerIndex++)
        {
            if (playerMovement[playerIndex].GetMoveSpeed() > defaultPlayerMoveSpd)
            {
                spdChagneAuraEntity = Instantiate(loadedEffects[EffectName.SPD_UP_AURA], players[playerIndex]);
                Debug.Log(spdChagneAuraSystem[playerIndex]);    //  デバッグ用--------------------------------------
            }
            else if (playerMovement[playerIndex].GetMoveSpeed() < defaultPlayerMoveSpd)
            {
                spdChagneAuraEntity = Instantiate(loadedEffects[EffectName.SPD_DOWN_AURA], players[playerIndex]);
                Debug.Log(spdChagneAuraSystem[playerIndex]);    //  デバッグ用--------------------------------------
            }
            else
            {
                Debug.Log(spdChangingFailMsg);    //  デバッグ用--------------------------------------
                isGeneratedAllSpdChangeAura = false;    //  エラーハンドリング追加---------------------------------------
            }
            if (isGeneratedAllSpdChangeAura)
            {
                spdChagneAuraSystem[playerIndex] = spdChagneAuraEntity.GetComponent<ParticleSystem>();
            }
        }
    }
    void StopSpdChangeAura()    //  エフェクトを非表示にする
    {
        Debug.Log("Stop Spd Change Method entire");
        StopEffect(spdChagneAuraSystem);    //  改善余地あり
    }
    void StopEffect(ParticleSystem[] effect)    //  エフェクト一時停止
    {
        foreach(ParticleSystem p in effect)
        {
            Debug.Log("Effect Stop loop entire");
            p?.Stop();
        }
    }                                                                                           ////  関数区終了  ////
}
// void ReplayEffect(ParticleSystem effect, Vector3 playPos)    //  エフェクト再再生            ////  コード保存場所  ////
//    {
//    effect.transform.position = playPos;
//    effect.Clear();
//    effect.Play();
//}
//[PunRPC]
//internal void PlayEventEffect(int EffectNo)    //  イベントエフェクトのラッパー関数
//{
//    switch (EffectNo)
//    {
//        case 0:
//            {
//                if (isGeneratedSmoke)
//                {
//                    Debug.Log(teleportSmokeSystem);

//                }
//                else
//                {

//                    Debug.Log(teleportSmokeSystem);
//                }
//                break;
//            }
//        default:
//            {
//                break;
//            }
//    }
//}
//[PunRPC]
//internal void GenerateEffect(int EffectNo, Vector3 players, int playerIndex)    //  エフェクト生成(ラッパー関数)
//{
//    Debug.Log("TPエフェクト生成突入");
//    Debug.Log("TPエフェクト" + teleportSmokePrefab.name);

//    switch (EffectNo)
//    {
//        case 0:
//            {
//                if (isGeneratedSmoke)
//                {
//                    Debug.Log(teleportSmokeSystem);
//                    teleportSmokeSystem[playerIndex].transform.position = players;
//                    teleportSmokeSystem[playerIndex].Clear();
//                    teleportSmokeSystem[playerIndex].Play();
//                }
//                else
//                {
//                    teleportSmokeEntity = Instantiate(teleportSmokePrefab);
//                    teleportSmokeEntity.transform.position = players;
//                    teleportSmokeSystem[playerIndex] = teleportSmokeEntity.GetComponent<ParticleSystem>();
//                    Debug.Log(teleportSmokeSystem);
//                }
//                break;
//            }
//        default:
//            {
//                break;
//            }
//    }
//}
