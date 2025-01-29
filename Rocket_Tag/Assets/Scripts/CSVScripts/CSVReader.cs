#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
public class CSVReader : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            if (str.IndexOf("/SKillData.csv") != -1)
            {
                TextAsset textasset = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                SkillDataBase sd = AssetDatabase.LoadAssetAtPath<SkillDataBase>(assetfile);
                if (sd == null)
                {
                    sd = ScriptableObject.CreateInstance<SkillDataBase>(); // インスタンスを作成
                    AssetDatabase.CreateAsset(sd, assetfile);
                }
                sd.skillDatas = CSVSerializer.Deserialize<SkillData>(textasset.text);
                EditorUtility.SetDirty(sd);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(); // アセットデータベースを更新

            }
        }
    }
}
#endif
