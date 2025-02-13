using UnityEngine;
using UnityEngine.UI;

public class TabSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject skillSection;   // �X�L���̃Z�N�V����
    [SerializeField] private GameObject costumeSection; // �R�X�`���[���̃Z�N�V����
    [SerializeField] private Button skillTabButton;     // �X�L���^�u�̃{�^��
    [SerializeField] private Button costumeTabButton;   // �R�X�`���[���^�u�̃{�^��

    private void Start()
    {
        // ������ԂŃX�L���Z�N�V������\��
        ShowSkillSection();

        // �{�^���ɃC�x���g��o�^
        skillTabButton.onClick.AddListener(ShowSkillSection);
        costumeTabButton.onClick.AddListener(ShowCostumeSection);
    }

    private void ShowSkillSection()
    {
        skillSection.SetActive(true);
        costumeSection.SetActive(false);
    }

    private void ShowCostumeSection()
    {
        skillSection.SetActive(false);
        costumeSection.SetActive(true);
    }
}
