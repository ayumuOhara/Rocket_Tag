using UnityEngine;

[System.Serializable]
public class SkillData
{
    public int skillCode;       // スキル番号
    public string skillName;    // スキル名
    public int countLimit;      // スキル使用可能回数
    public string skillEffect;  // スキル効果説明文
}

public class SkillDataBase : ScriptableObject
{
    public SkillData[] skillDatas;
}