using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using Photon.Pun;

public class EventEffect : MonoBehaviourPunCallbacks                    ////  イベントのエフェクトを扱うスクリプト(視界妨害を除く)  ////
{
    internal enum EventEffectNo                             ////  以下宣言区  ////
    {
        TELEPORT_SMOKE,
    }

    static GameObject teleportSmokePrefab;
    GameObject teleportSmokeEntity;
    ParticleSystem[] teleportSmokeSystem;

    const int numOfPlayers = 4;
    bool isGeneratedSmoke;

    internal bool _IsGeneratedSmoke
    { set { isGeneratedSmoke = value; } }                   ////  宣言区終了  ////

    void Start()                                            ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }
    async void Initialize()    //  初期化関数
    {
        if (teleportSmokePrefab == null)
        {
            LoadEffect();
        }
        teleportSmokeSystem = new ParticleSystem[numOfPlayers];
        isGeneratedSmoke = false;
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
}