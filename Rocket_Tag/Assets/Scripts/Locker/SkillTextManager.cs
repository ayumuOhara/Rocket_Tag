using UnityEngine;
using TMPro;

public class SkillTextManager : MonoBehaviour
{
    public TextMeshProUGUI skillNameText;        // スキル名を表示するテキスト
    public TextMeshProUGUI skillDescriptionText; // スキル効果を表示するテキスト
    private string currentSkillName = "";

    void Start()
    {
        // 初期状態でスキルテキストを非表示
        skillNameText.gameObject.SetActive(false);
        skillDescriptionText.gameObject.SetActive(false);
    }

    public void ShowSkillEffect(string skillName, string skillEffect)
    {
        // 別のスキルが選択されたらテキストを更新
        if(currentSkillName != skillEffect)
        {
            skillNameText.gameObject.SetActive(true);
            skillNameText.text = skillName;
            skillDescriptionText.gameObject.SetActive(true);
            skillDescriptionText.text = skillEffect;
            currentSkillName = skillName;
        }
    }
}
