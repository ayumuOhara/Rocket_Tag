using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Test : MonoBehaviour
{
    GameObject[] frameEffectPrefab;
    GameObject smokeEffectPrefab;
    public GameObject a;
    GameObject parent;
    public GameObject P1;
    int njj = 0;
    async void Start()
    {
       await RocketEffectLoad();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(parent == P1)
            {
                parent = GameObject.Find("Cube");
            }
            else if(parent != P1)
            {
                parent = P1;
            }
            GameObject b = Instantiate(frameEffectPrefab[njj], parent.transform);
            njj++;
        }
    }
    async Task RocketEffectLoad()    //  ロケットエフェクトのロード
    {
        Task[] loadTasks;

        AsyncOperationHandle<GameObject>[] loadHandles;

        const int numOfFrameEffect = 4;
        const int numOfSmokeEffect = 1;
        int loadHandleArrayNo;
        string[] frameEffectNames = { "FirstRocketFrame", "SecondRocketFrame", "ThirdRocketFrame", "LastRocketFrame" };
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