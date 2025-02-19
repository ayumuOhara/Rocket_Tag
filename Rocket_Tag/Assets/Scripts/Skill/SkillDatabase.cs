using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SkillDataBase", menuName = "Scriptable Objects/SkillDataBase")]
public class SkillDataBase : ScriptableObject
{
    public List<SkillData> SkillData;
}

[System.Serializable]
public class SkillData
{
    public int skillId;
    public SkillName skillName;
    public Sprite skillIcon;

    public enum SkillName
    {
        PullHook      ,
        StickyZone    ,
        DangerousGift ,
        SmashPunch    ,
        CrashingDash  ,
    }
}