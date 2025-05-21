//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AddressableAssets;

//[CreateAssetMenu(fileName = "NewEffectDate", menuName = "Game/EffectDate")]
//public class EffectCatalog : ScriptableObject
//{
//    [System.Serializable]
//    internal class Entry
//    {
//        [SerializeField] GameObject effectPrefab;
//        [SerializeField] GameObject fallbackPrefab;
//        [SerializeField] AssetReferenceGameObject addressRef;

//        internal GameObject EffectPrefab => effectPrefab;
//        internal GameObject FallbackPrefab => effectPrefab;
//        internal AssetReferenceGameObject AddressRef => addressRef;
//    }
//    [SerializeField] List<Entry> entries = new();
//    Dictionary<AssetReferenceGameObject, Entry> entryMap;
//    public void Initialize()    //  ‰Šú‰»
//    {
//        entryMap = new();
//        foreach (Entry e in entries)
//        {
//            if (!entryMap.ContainsKey(e.AddressRef))
//            {
//                entryMap[e.AddressRef] = e;
//            }
//            else
//            {

//            }
//        }
//    }
//}
