using UnityEngine;

[System.Serializable]
public class SkillData
{
    public int skillCode;       // �X�L���ԍ�
    public string skillName;    // �X�L����
    public int countLimit;      // �X�L���g�p�\��
    public string skillEffect;  // �X�L�����ʐ�����
}

public class SkillDataBase : ScriptableObject
{
    public SkillData[] skillDatas;
}