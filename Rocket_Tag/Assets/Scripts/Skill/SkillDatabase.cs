using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillDataBase", menuName = "Scriptable Objects/SkillDataBase")]
public class SkillDataBase : ScriptableObject
{
    public List<SkillData> SkillData;
}

[System.Serializable]
public class SkillData
{
    public int SkillId;
    public SkillName skillName;

    public enum SkillName
    {
        PullHook      ,
        StickyZone    ,
        DangerousGift ,
        SmashPunch    ,
        CrashingDash  ,
    }
}