//using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using static EventEffect;
//public class EffectLoad : MonoBehaviour
//{
//    internal enum EffectName    //  エフェクトの名前一覧  
//    {
//        TELEPORT_SMOKE,
//        SPD_UP_AURA,
//        SPD_DOWN_AURA,
//    }

//    Dictionary<EffectName, String> effectNameMap;
//    Dictionary<EffectName, GameObject> loadedEffects;
//    const int numOfPlayers = 4;
//    bool cantLoadEffect;
//    string loadFailMsg = "Load is failed";

//    async void Start()
//    {
//        await LoadEffect();
//    }
//    void Initialize()    //  初期化
//    {
        
//    }
//    void SetDictionary()    //  辞書型変数をセッティング
//    {
//        effectNameMap = new Dictionary<EffectName, String>()
//        {
//            {EffectName.TELEPORT_SMOKE, "TeleportSmoke" },
//            {EffectName.SPD_UP_AURA, "SpdUpAura" },
//            {EffectName.SPD_DOWN_AURA, "SpdDownAura" }
//        };
//        loadedEffects = new Dictionary<EffectName, GameObject>()
//        {
//            {EffectName.TELEPORT_SMOKE,null },
//            {EffectName.SPD_UP_AURA,   null },
//            {EffectName.SPD_DOWN_AURA, null }
//        };
//    }
//    async Task LoadEffect()    //  エフェクトロード
//    {
//        List<Task> loadTask;

//        Dictionary<EffectName, AsyncOperationHandle<GameObject>> loadHandle = new Dictionary<EffectName, AsyncOperationHandle<GameObject>>
//        {
//            {EffectName.TELEPORT_SMOKE, default},
//            {EffectName.SPD_UP_AURA, default},
//            {EffectName.SPD_DOWN_AURA, default}
//        };

//        loadTask = new List<Task>();

//        foreach (KeyValuePair<EffectName, String> kvp in effectNameMap)
//        {
//            KeyValuePair<EffectName, String> kvps = kvp;    //  必要か精査-----------------------------------

//            loadHandle[kvp.Key] = Addressables.LoadAssetAsync<GameObject>(kvps.Value);
//            loadTask.Add(loadHandle[kvps.Key].Task.ContinueWith(t =>    //  読み込みから代入までのタスクを追加する
//            {
//                if (loadHandle[kvps.Key].Status == AsyncOperationStatus.Succeeded)
//                {
//                    loadedEffects[kvps.Key] = loadHandle[kvps.Key].Result;
//                }
//                else
//                {
//                    Debug.Log(loadFailMsg);    //  デバッグ用--------------------------------------
//                    cantLoadEffect = true;
//                }
//            }));
//        }
//        await Task.WhenAll(loadTask);
//    }
//}
