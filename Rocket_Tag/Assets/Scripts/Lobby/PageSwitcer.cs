using UnityEngine;
using UnityEngine.UI;

public class PageSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject firstSection;
    [SerializeField] private GameObject secondSection;
    [SerializeField] private GameObject thirdSection;
    [SerializeField] private GameObject fourSection;
    [SerializeField] private GameObject fiveSection;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Text statusText;
    [SerializeField] private Text pageText;


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
        else if (currentSection == thirdSection)
        {
            ShowFourSection();
        }
        else if (currentSection == fourSection)
        {
            ShowFiveSection();
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
        else if (currentSection == fourSection)
        {
            ShowThirdSection();
        }
        else if (currentSection == fiveSection)
        {
            ShowFourSection();
        }
    }

    private void ShowFirstSection()
    {
        currentSection = firstSection; // 現在のセクションを更新
        NewText();
        pageText.text = "1/5";

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを表示
        backButton.gameObject.SetActive(false);   // 戻るボタンを非表示

        firstSection.SetActive(true);
        secondSection.SetActive(false);
        thirdSection.SetActive(false);
        fourSection.SetActive(false);
        fiveSection.SetActive(false);
    }

    private void ShowSecondSection()
    {
        currentSection = secondSection; // 現在のセクションを更新
        NewText();
        pageText.text = "2/5";

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを表示
        backButton.gameObject.SetActive(true);    // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(true);
        thirdSection.SetActive(false);
        fourSection.SetActive(false);
        fiveSection.SetActive(false);
    }

    private void ShowThirdSection()
    {
        currentSection = thirdSection; // 現在のセクションを更新
        NewText();
        pageText.text = "3/5";

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを非表示
        backButton.gameObject.SetActive(true);     // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(false);
        thirdSection.SetActive(true);
        fourSection.SetActive(false);
        fiveSection.SetActive(false);
    }
    private void ShowFourSection()
    {
        currentSection = fourSection; // 現在のセクションを更新
        NewText();
        pageText.text = "4/5";

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを非表示
        backButton.gameObject.SetActive(true);     // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(false);
        thirdSection.SetActive(false);
        fourSection.SetActive(true);
        fiveSection.SetActive(false);
    }
    private void ShowFiveSection()
    {
        currentSection = fiveSection; // 現在のセクションを更新
        NewText();
        pageText.text = "5/5";

        forwardButton.gameObject.SetActive(false); // 次へ進むボタンを非表示
        backButton.gameObject.SetActive(true);     // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(false);
        thirdSection.SetActive(false);
        fourSection.SetActive(false);
        fiveSection.SetActive(true);
    }
    public void NewText()
    {
        if(currentSection == firstSection) //チュートリアルテキスト（１ページ目）
        {
            statusText.text =
                "ルールは時間経過で鬼が死亡していく\n鬼ごっこ式バトルロワイヤル。\n相手に接触することで、鬼の印の\nロケットを擦り付けて生き延びよう。\n"//ここに入力
                ;
        }
        else if(currentSection == secondSection)//チュートリアルテキスト（２ページ目）
        {
            statusText.text =
                "移動方法はWASDキー。\nまたは矢印キーの両方に対応。\n"//ここに入力
                ;
        }
        else if(currentSection == thirdSection)//チュートリアルテキスト（３ページ目）
        {
            statusText.text =
                "マウスを動かすと視点が変わり、\nALTキーを押すとマウスカーソルが出てきて設定を開いたりできるぞ。\n"//ここに入力
                ;
        }
        else if (currentSection == fourSection)//チュートリアルテキスト（４ページ目）
        {
            statusText.text =
                "追い詰められてもスキルで逆転。\nEキーを押して、鬼から逃げろ‼\n鬼をなすり付けろ‼\n"//ここに入力
                ;
        }
        else if (currentSection == fiveSection)//チュートリアルテキスト（５ページ目）
        {
            statusText.text =
                "マップ内では、定期的にイベントが発生しプレイヤーにランダムな効果を与える。\n何が起こるかは君次第‼\n"//ここに入力
                ;
        }
        else
        {
            statusText.text = "";
        }
    }
}
