using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillCoolTime : MonoBehaviour
{

    [SerializeField] Image cooldownMask;  // クール用
    public bool SkillCool = true;

    public IEnumerator CoolTime(float SkillCT)//クールタイム
    {
        Debug.Log("呼ばれた");
        float elapsed = 0f;
        SkillCool = false;
        cooldownMask.fillAmount = 1f;

        while (elapsed < SkillCT)
        {
            elapsed += Time.deltaTime;
            cooldownMask.fillAmount = 1f - (elapsed / SkillCT);
            yield return null;
        }

        cooldownMask.fillAmount = 0f;
        SkillCool = true;
    }
}
