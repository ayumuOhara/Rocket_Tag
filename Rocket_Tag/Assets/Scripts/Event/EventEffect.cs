using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Photon.Pun;

public class EventEffect : MonoBehaviourPunCallbacks                    ////  イベントのエフェクトを扱うスクリプト(視界妨害を除く)  ////
{
    internal enum EventEffectProcces    //  イベントエフェクトを                             ////  以下宣言区  ////
    {
        TELEPORT_SMOKE,
        MOVE_SPD_AURA,
        STOP_SPD_AURA
    }
    internal enum EffectName
    {
        TELEPORT_SMOKE,
        SPD_UP_AURA,
        SPD_DOWN_AURA,
    }

    Dictionary<EventEffectProcces, Action> effectProccesMap;
    Dictionary<EffectName, String> effectNameMap;
    Dictionary<EffectName, GameObject> loadedEffects;
    GameObject teleportSmokeEntity;
    GameObject spdChagneAuraEntity;
    Transform[] players;
    ParticleSystem[] teleportSmokeSystem;
    ParticleSystem[] spdChagneAuraSystem;
    GameManager gameMgr;
    PlayerMovement[] playerMovement;

    const int numOfPlayers = 4;
    const int SpdChangeAuraValue = 2;
    float defaultPlayerMoveSpd;
    bool isGeneratedSmoke;
    bool isGeneratedSpdAura;
    
    internal bool _IsGeneratedSmoke
    { set { isGeneratedSmoke = value; } }                   ////  宣言区終了  ////
    internal Transform[] _Players
    { set { players = value; } }                   ////  宣言区終了  ////

    void Start()                                            ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }
    async void Initialize()    //  初期化関数
    {
        SetDictionary();

        await LoadEffect();
        teleportSmokeSystem = new ParticleSystem[numOfPlayers];
        spdChagneAuraSystem = new ParticleSystem[numOfPlayers];
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

        isGeneratedSmoke = false;
        isGeneratedSpdAura = false;
    }
    void SetDictionary()    //  辞書型変数をセッティング
    {
        effectProccesMap = new Dictionary<EventEffectProcces, Action>
        {
            { EventEffectProcces.TELEPORT_SMOKE, HandleTeleportSmoke},
            { EventEffectProcces.MOVE_SPD_AURA, ApplySpdBuffEffect},
            { EventEffectProcces.STOP_SPD_AURA, StopSpdChangeAura}
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
        loadedEffects = new Dictionary<EffectName, GameObject>();
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
            KeyValuePair<EffectName, String> kvps = kvp;
            Debug.Log("ロード突入");
            loadHandle[kvp.Key] = Addressables.LoadAssetAsync<GameObject>(kvps.Value);
            loadTask.Add(loadHandle[kvps.Key].Task.ContinueWith(t =>
            {
                Debug.Log("バリュー" + kvps.Value);
                Debug.Log(effectNameMap[kvps.Key]);
                if (loadHandle[kvps.Key].Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log(125);
                    //loadedEffects.Add(kvp.Key,loadHandle.Result);
                    //loadedEffects.Add(kvp.Key, kvp.Value) = loadHandle.Result;
                    loadedEffects[kvps.Key] = loadHandle[kvps.Key].Result;
                }
                else
                {
                    Debug.Log("失敗エフェクト");
                    //loadedEffects[kvps.Key] = loadHandle.Result;
                    //Debug.Log----------------------------------
                }
            }));
        }
        await Task.WhenAll(loadTask);
        Debug.Log("ロード終了");
    }
    [PunRPC]
    void CallEffectProcces(EventEffectProcces eventEffectNo)    //  エフェクト処理呼び出し
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
        else
        {
            GenerateTeleportSmoke();
            isGeneratedSmoke = true;
        }
    }
    void ReplayEffect(ParticleSystem effect, Vector3 replayPos)    //  エフェクト再再生                       ////  コード保存場所  ////
    {
        if (effect.transform.parent == null)
        {
            effect.transform.position = replayPos;
        }
        else
        {
            effect.transform.localPosition = replayPos;
        }
        effect.Clear();
        effect.Play();
    }
    void GenerateTeleportSmoke()    //  テレポートスモーク生成
    {
        AssignPlyaersTF();

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
    void AssignPlyaersTF()    //  プレイヤーのトランスフォームを取得
    {
        // || players.Length > 4
        if (players == null)
        {
            players = gameMgr.GetPlayerList().ConvertAll(x => x.transform).ToArray();
        }
    }
    void GetPlayersMovement()    //  プレイヤームーブメントインスタンス取得
    {
        AssignPlyaersTF();
        // && playerMovement.Length > 4
        if (playerMovement == null)
        {
            playerMovement = new PlayerMovement[numOfPlayers];
            for (int playerIndex = 0; playerIndex < numOfPlayers; playerIndex++)
            {
                playerMovement[playerIndex] = players[playerIndex].GetComponent<PlayerMovement>();
            }
            defaultPlayerMoveSpd = playerMovement[0].GetDefaultMoveSpeed();
        }
    }
    void ApplySpdBuffEffect()    //  運動能力変化エフェクト制御
    {
        Debug.Log("エフェクト関数突入");
        GetPlayersMovement();
        if (!isGeneratedSpdAura)
        {
            for (int playerIndex = 0; playerIndex < numOfPlayers; playerIndex++)
            {
                Debug.Log("エフェクト生成ループ突入");

                if (playerMovement[playerIndex].GetMoveSpeed() > defaultPlayerMoveSpd)
                {
                    spdChagneAuraEntity = Instantiate(loadedEffects[EffectName.SPD_UP_AURA], players[playerIndex]);
                    spdChagneAuraSystem[playerIndex] = spdChagneAuraEntity.GetComponent<ParticleSystem>();
                    Debug.Log(spdChagneAuraSystem[playerIndex]);
                }
                else if (playerMovement[playerIndex].GetMoveSpeed() < defaultPlayerMoveSpd)
                {
                    spdChagneAuraEntity = Instantiate(loadedEffects[EffectName.SPD_DOWN_AURA], players[playerIndex]);
                    spdChagneAuraSystem[playerIndex] = spdChagneAuraEntity.GetComponent<ParticleSystem>();
                    Debug.Log(spdChagneAuraSystem[playerIndex]);
                }
                Debug.Log("loaded" + loadedEffects[EffectName.SPD_UP_AURA].name);

            }
            isGeneratedSpdAura = true;
        }
        else
        {
            for (int playerIndex = 0; playerIndex < numOfPlayers; playerIndex++)
            {
                ReplayEffect(spdChagneAuraSystem[playerIndex], Vector3.zero);
            }
        }
    }
    void StopSpdChangeAura()    //  エフェクトを非表示にする
    {
        foreach (ParticleSystem p in spdChagneAuraSystem)
        {
            if (p != null)
            {
                p.Stop();
            }
        }
    }
}
// void ReplayEffect(ParticleSystem effect, Vector3 playPos)    //  エフェクト再再生                       ////  コード保存場所  ////
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
