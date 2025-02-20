using UnityEngine;
using TMPro;

public class SkillTextManager : MonoBehaviour
{
    public TextMeshProUGUI skillNameText;        // �X�L������\������e�L�X�g
    public TextMeshProUGUI skillDescriptionText; // �X�L�����ʂ�\������e�L�X�g
    private string currentSkillName = "";

    void Start()
    {
        // ������ԂŃX�L���e�L�X�g���\��
        skillNameText.gameObject.SetActive(false);
        skillDescriptionText.gameObject.SetActive(false);
    }

    public void ShowSkillEffect(string skillName, string skillEffect)
    {
        // �ʂ̃X�L�����I�����ꂽ��e�L�X�g���X�V
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
