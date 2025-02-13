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
        // 初期状態で1ページ目を表示
        ShowFirstSection();

        // ボタンにイベントを登録
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
        currentSection = firstSection; // 現在のセクションを更新

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを表示
        backButton.gameObject.SetActive(false);   // 戻るボタンを非表示

        firstSection.SetActive(true);
        secondSection.SetActive(false);
        thirdSection.SetActive(false);
    }

    private void ShowSecondSection()
    {
        currentSection = secondSection; // 現在のセクションを更新

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを表示
        backButton.gameObject.SetActive(true);    // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(true);
        thirdSection.SetActive(false);
    }

    private void ShowThirdSection()
    {
        currentSection = thirdSection; // 現在のセクションを更新

        forwardButton.gameObject.SetActive(false); // 次へ進むボタンを非表示
        backButton.gameObject.SetActive(true);     // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(false);
        thirdSection.SetActive(true);
    }
}
