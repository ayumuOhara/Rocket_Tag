using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CompasGenerater : MonoBehaviour
{

    GameObject compasPrefab;
    GameObject compasEntity;
    public Transform player;
    void Start()
    {
        Initialize();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateCompas();
        }
    }
    async void Initialize()    //  初期化
    {
        await LoadCompas();
    }
    void GenerateCompas()    //  コンパス生成
    {
        Destroy(compasEntity);
        compasEntity = Instantiate(compasPrefab, player);
    }
    async Task LoadCompas()    //  コンパスロード
    {
        AsyncOperationHandle<GameObject> LoadHandle;

        string compasName = "Compas";

        LoadHandle = Addressables.LoadAssetAsync<GameObject>(compasName);
        await Task.WhenAll(LoadHandle.Task);
        compasPrefab = LoadHandle.Result;
        Debug.Log(111);

    }
}
