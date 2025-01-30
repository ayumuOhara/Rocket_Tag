using UnityEngine;
using UnityEngine.UI;

public class PageSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject firstSection;
    [SerializeField] private GameObject secondSection;
    [SerializeField] private GameObject thirdSection;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;

    private GameObject currentSection;

    private void Start()
    {
        // ������Ԃ�1�y�[�W�ڂ�\��
        ShowFirstSection();

        // �{�^���ɃC�x���g��o�^
        forwardButton.onClick.AddListener(GoForwardSection);
        backButton.onClick.AddListener(GoBackSection);
    }

    public void GoForwardSection()
    {
        if (currentSection == firstSection)
        {
            ShowSecondSection();
        }
        else if (currentSection == secondSection)
        {
            ShowThirdSection();
        }
    }

    public void GoBackSection()
    {
        if (currentSection == secondSection)
        {
            ShowFirstSection();
        }
        else if (currentSection == thirdSection)
        {
            ShowSecondSection();
        }
    }

    private void ShowFirstSection()
    {
        currentSection = firstSection; // ���݂̃Z�N�V�������X�V

        forwardButton.gameObject.SetActive(true); // ���֐i�ރ{�^����\��
        backButton.gameObject.SetActive(false);   // �߂�{�^�����\��

        firstSection.SetActive(true);
        secondSection.SetActive(false);
        thirdSection.SetActive(false);
    }

    private void ShowSecondSection()
    {
        currentSection = secondSection; // ���݂̃Z�N�V�������X�V

        forwardButton.gameObject.SetActive(true); // ���֐i�ރ{�^����\��
        backButton.gameObject.SetActive(true);    // �߂�{�^����\��

        firstSection.SetActive(false);
        secondSection.SetActive(true);
        thirdSection.SetActive(false);
    }

    private void ShowThirdSection()
    {
        currentSection = thirdSection; // ���݂̃Z�N�V�������X�V

        forwardButton.gameObject.SetActive(false); // ���֐i�ރ{�^�����\��
        backButton.gameObject.SetActive(true);     // �߂�{�^����\��

        firstSection.SetActive(false);
        secondSection.SetActive(false);
        thirdSection.SetActive(true);
    }
}
