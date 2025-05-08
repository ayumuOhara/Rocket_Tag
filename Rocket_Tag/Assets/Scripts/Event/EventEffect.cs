using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class EventEffect : MonoBehaviour                    ////  イベントのエフェクトを扱うスクリプト  ////
{
    internal enum EventEffectNo
    {
        TELEPORT_SMOKE,
    }

    GameObject teleportSmokePrefab;
    GameObject teleportSmokeEntity;
    public Transform a;
    ParticleSystem[] teleportSmokeSystem;

    const int numOfPlayers = 4;
    bool isGeneratedSmoke;

    void Start()                                            ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }
    async void Initialize()    //  初期化関数
    {
        LoadEffect();
        teleportSmokeSystem = new ParticleSystem[numOfPlayers];
        isGeneratedSmoke = false;
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GenerateEffect((int)EventEffectNo.TELEPORT_SMOKE, a, 0);
        }
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
    internal void GenerateEffect(int EffectNo, Transform players, int playerIndex)    //  エフェクト生成(ラッパー関数)
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
                        teleportSmokeEntity.transform.position = players.position;
                        teleportSmokeSystem[playerIndex] = teleportSmokeEntity.GetComponent<ParticleSystem>();
                        isGeneratedSmoke = true;
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