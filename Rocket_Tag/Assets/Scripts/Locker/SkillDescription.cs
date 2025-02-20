using UnityEngine;
using UnityEngine.UI;

public class SkillDescription : MonoBehaviour
{
    public string skillName;    // �X�L���̖��O
    public string skillEffect;  // �X�L���̌���
    private SkillTextManager skillTextManager;

    void Start()
    {
        // �V�[������SkillTextManager���擾
        skillTextManager = FindFirstObjectByType<SkillTextManager>();

        // �{�^���������ꂽ�Ƃ��ɃX�L���e�L�X�g��\��
        GetComponent<Button>().onClick.AddListener(() => skillTextManager.ShowSkillEffect(skillName,skillEffect));
    }
}
