using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using Photon.Pun;
using System.Collections.Generic;
using System;

public class EventEffect : MonoBehaviourPunCallbacks                    ////  イベントのエフェクトを扱うスクリプト(視界妨害を除く)  ////
{
    internal enum EventEffectNo                             ////  以下宣言区  ////
    {
        TELEPORT_SMOKE,
    }

    static GameObject teleportSmokePrefab;
    GameObject teleportSmokeEntity;
    Transform[] Players;
    ParticleSystem[] teleportSmokeSystem;
    GameManager gameMgr;
    Dictionary<EventEffectNo, Action> effectmap;

    const int numOfPlayers = 4;
    bool isGeneratedSmoke;
    
    internal bool _IsGeneratedSmoke
    { set { isGeneratedSmoke = value; } }                   ////  宣言区終了  ////
    internal Transform[] _Players
    { set { Players = value; } }                   ////  宣言区終了  ////

    void Start()                                            ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }
    async void Initialize()    //  初期化関数
    {
        if (teleportSmokePrefab == null)
        {
            await LoadEffect();
        }
        EffectMapSet();
        teleportSmokeSystem = new ParticleSystem[numOfPlayers];
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
        //Players = gameMgr.GetPlayerList().ConvertAll(x => x.transform).ToArray();    fordebug-------------------------

        isGeneratedSmoke = false;
    }
    void EffectMapSet()    //  エフェクトマップをセッティング
    {
        effectmap = new Dictionary<EventEffectNo, Action> {
            { EventEffectNo.TELEPORT_SMOKE, HandleTeleportSmoke}
        };
    }
    async Task LoadEffect()    //  エフェクトロード
    {
        Task[] loadTask;

        AsyncOperationHandle<GameObject>[] loadHandle;

        const int numOfEffect = 1;
        int loadHandleArrayNo;
        string[] smokeEffectNames = { "TeleportSmoke" };

        loadTask = new Task[numOfEffect];

        loadHandle = new AsyncOperationHandle<GameObject>[numOfEffect];

        loadHandleArrayNo = 0;

        for (; loadHandleArrayNo < numOfEffect; loadHandleArrayNo++)
        {
            loadHandle[loadHandleArrayNo] = Addressables.LoadAssetAsync<GameObject>(smokeEffectNames[loadHandleArrayNo]);
            loadTask[loadHandleArrayNo] = loadHandle[loadHandleArrayNo].Task;
        }
        await Task.WhenAll(loadTask);
        for (loadHandleArrayNo = 0; loadHandleArrayNo < numOfEffect; loadHandleArrayNo++)
        {
            teleportSmokePrefab = loadHandle[loadHandleArrayNo].Result;
        }
    }
    [PunRPC]
    void CallEffectProcces(EventEffectNo eventEffectNo)    //  エフェクト処理呼び出し
    {
        effectmap[eventEffectNo]();
    }
    [PunRPC]
    internal void GenerateEffect(int EffectNo, Vector3 players, int playerIndex)    //  エフェクト生成(ラッパー関数)
    {
        Debug.Log("TPエフェクト生成突入");
        Debug.Log("TPエフェクト" + teleportSmokePrefab.name);

        switch (EffectNo)
        {
            case 0:
                {
                    if (isGeneratedSmoke)
                    {
                        Debug.Log(teleportSmokeSystem);
                        teleportSmokeSystem[playerIndex].transform.position = players;
                        teleportSmokeSystem[playerIndex].Clear();
                        teleportSmokeSystem[playerIndex].Play();
                    }
                    else
                    {
                        teleportSmokeEntity = Instantiate(teleportSmokePrefab);
                        teleportSmokeEntity.transform.position = players;
                        teleportSmokeSystem[playerIndex] = teleportSmokeEntity.GetComponent<ParticleSystem>();
                        Debug.Log(teleportSmokeSystem);
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    [PunRPC]
    internal void PlayEventEffect(int EffectNo)    //  イベントエフェクトのラッパー関数
    {
        switch (EffectNo)
        {
            case 0:
                {
                    if (isGeneratedSmoke)
                    {
                        Debug.Log(teleportSmokeSystem);

                    }
                    else
                    {

                        Debug.Log(teleportSmokeSystem);
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    void HandleTeleportSmoke()    //  テレポート時の煙の制御
    {
        if(isGeneratedSmoke)
        {
            for (int i = 0; i < numOfPlayers; i++)
            {
                if (Players[i].gameObject.activeSelf)
                {
                    ReplayEffect(teleportSmokeSystem[i], Players[i].position);
                }
            }
        }
        else
        {
            GenerateTeleportSmoke();
            isGeneratedSmoke = true;
        }
    }
    void ReplayEffect(ParticleSystem effect, Vector3 playPos)    //  エフェクト再再生                       ////  コード保存場所  ////
    {
        effect.transform.position = playPos;
        effect.Clear();
        effect.Play();
    }
    void GenerateTeleportSmoke()    //  テレポートスモーク生成
    {
        for (int i = 0; i < numOfPlayers; i++)
        {
            if (Players[i].gameObject.activeSelf)
            {
                teleportSmokeEntity = Instantiate(teleportSmokePrefab);
                teleportSmokeEntity.transform.position = Players[i].position;
                teleportSmokeSystem[i] = teleportSmokeEntity.GetComponent<ParticleSystem>();
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
