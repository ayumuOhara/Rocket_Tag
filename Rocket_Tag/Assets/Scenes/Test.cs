using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Test : MonoBehaviour
{
    GameObject[] frameEffectPrefab;
    GameObject smokeEffectPrefab;
    async void Start()
    {
       await RocketEffectLoad();
    }
    void Update()
    {
        
    }
    async Task RocketEffectLoad()    //  ロケットエフェクトのロード
    {
        Task[] loadTasks;

        AsyncOperationHandle<GameObject>[] loadHandles;

        const int numOfFrameEffect = 4;
        const int numOfSmokeEffect = 1;
        int loadHandleArrayNo;
        string[] frameEffectNames = { "FirstRocketFrame", "SecondRocketFrame", "ThridRocketFrame", "LastRocketFrame" };
        string smokeEffectName;

        loadTasks = new Task[numOfFrameEffect + numOfSmokeEffect];

        frameEffectPrefab = new GameObject[numOfFrameEffect];

        loadHandles = new AsyncOperationHandle<GameObject>[numOfFrameEffect + numOfSmokeEffect];

        loadHandleArrayNo = 0;    //  同一的な配列の要素数を指定するために使うときもあります
        smokeEffectName = "FrameSmoke";
        Debug.Log(loadHandleArrayNo);

        for (; loadHandleArrayNo < numOfFrameEffect + numOfSmokeEffect; loadHandleArrayNo++)
        {
            if (loadHandleArrayNo < numOfFrameEffect)
            {
                loadHandles[loadHandleArrayNo] = Addressables.LoadAssetAsync<GameObject>(frameEffectNames[loadHandleArrayNo]);
            }
            else
            {
                loadHandles[loadHandleArrayNo] = Addressables.LoadAssetAsync<GameObject>(smokeEffectName);
            }
            loadTasks[loadHandleArrayNo] = loadHandles[loadHandleArrayNo].Task;
        }
        await Task.WhenAll(loadTasks);
        for (loadHandleArrayNo = 0; loadHandleArrayNo < numOfFrameEffect + numOfSmokeEffect; loadHandleArrayNo++)
        {
            if (loadHandleArrayNo < numOfFrameEffect)
            {
                frameEffectPrefab[loadHandleArrayNo] = loadHandles[loadHandleArrayNo].Result;
            }
            else
            {
                smokeEffectPrefab = loadHandles[loadHandleArrayNo].Result;
            }
        }
    }
}
