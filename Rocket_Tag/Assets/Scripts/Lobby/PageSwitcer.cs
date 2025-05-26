using UnityEngine;
using UnityEngine.UI;

public class PageSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject firstSection;
    [SerializeField] private GameObject secondSection;
    [SerializeField] private GameObject thirdSection;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Text pageText;
    [SerializeField] private Text MainText;


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
        AudioManager.Instance.PlaySE(SEManager.SEType.Page);
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
        AudioManager.Instance.PlaySE(SEManager.SEType.Page);
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
        //NewText();
        pageText.text = "1/3";

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを表示
        backButton.gameObject.SetActive(false);   // 戻るボタンを非表示

        firstSection.SetActive(true);
        secondSection.SetActive(false);
        thirdSection.SetActive(false);
    }

    private void ShowSecondSection()
    {
        currentSection = secondSection; // 現在のセクションを更新
        //NewText();
        pageText.text = "2/3";

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを表示
        backButton.gameObject.SetActive(true);    // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(true);
        thirdSection.SetActive(false);
    }

    private void ShowThirdSection()
    {
        currentSection = thirdSection; // 現在のセクションを更新
        //NewText();
        pageText.text = "3/3";

        forwardButton.gameObject.SetActive(false); // 次へ進むボタンを非表示
        backButton.gameObject.SetActive(true);     // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(false);
        thirdSection.SetActive(true);
    }
}
