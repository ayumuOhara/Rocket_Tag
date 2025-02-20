using UnityEngine;
using UnityEngine.UI;

public class SkillDescription : MonoBehaviour
{
    public string skillName;    // スキルの名前
    public string skillEffect;  // スキルの効果
    private SkillTextManager skillTextManager;

    void Start()
    {
        // シーン内のSkillTextManagerを取得
        skillTextManager = FindFirstObjectByType<SkillTextManager>();

        // ボタンが押されたときにスキルテキストを表示
        GetComponent<Button>().onClick.AddListener(() => skillTextManager.ShowSkillEffect(skillName,skillEffect));
    }
}
