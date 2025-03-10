using System;
using System.Collections;
using System.Collections.Generic;                                                          ////  スキン生成スクリプト  ////
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class tttt : MonoBehaviour
{
    GameObject[] playerSkinPrefab;
    void Start()
    {
        PlayerSkinLord();
    }
    void Update()
    {

    }
    async void PlayerSkinLord()     //  プレイヤースキン読み込み
    {
        Task[] task;

        AsyncOperationHandle<GameObject>[] playerSkinLordHandle;

        const int numOfSkin = 6;

        task = new Task[numOfSkin - 1];
        playerSkinPrefab = new GameObject[numOfSkin];

        playerSkinLordHandle = new AsyncOperationHandle<GameObject>[numOfSkin];

        string[] skinNames = new string[] {"NotWearing", "RedCap", "StrawHat", "Eringi", "Freeza", "Bear"};

        /*  スキンは永久的に使うので開放していない  */
        for (int arrayNo = numOfSkin - 1; arrayNo > 0; arrayNo--)
        {
            playerSkinLordHandle[arrayNo] = Addressables.LoadAssetAsync<GameObject>(skinNames[arrayNo]);
            task[arrayNo - 1] = playerSkinLordHandle[arrayNo].Task;
        }
        await Task.WhenAll(task);
        for (int arrayNo = numOfSkin - 1; arrayNo > 0; arrayNo--)
        {
            playerSkinPrefab[arrayNo] = playerSkinLordHandle[arrayNo].Result;
            await Task.Yield();
        }
    }
}
