using UnityEngine;
using UnityEngine.UI;

public class PageSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject firstSection;
    [SerializeField] private GameObject secondSection;
    [SerializeField] private GameObject thirdSection;
    [SerializeField] private GameObject fourSection;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Text statusText;
    [SerializeField] private GameObject onePageImage;
    [SerializeField] private GameObject twoPageImageL;
    [SerializeField] private GameObject twoPageImageR;
    [SerializeField] private GameObject threePageImage;
    [SerializeField] private GameObject fourPageImage;


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
        else if (currentSection == thirdSection)
        {
            ShowFourSection();
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
        else if (currentSection == fourSection)
        {
            ShowThirdSection();
        }
    }

    private void ShowFirstSection()
    {
        currentSection = firstSection; // 現在のセクションを更新
        NewText();

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを表示
        backButton.gameObject.SetActive(false);   // 戻るボタンを非表示

        firstSection.SetActive(true);
        secondSection.SetActive(false);
        thirdSection.SetActive(false);
        fourSection.SetActive(false);

        onePageImage.SetActive(true);
        twoPageImageL.SetActive(false);
        twoPageImageR.SetActive(false);
        threePageImage.SetActive(false);
        fourPageImage.SetActive(false);
    }

    private void ShowSecondSection()
    {
        currentSection = secondSection; // 現在のセクションを更新
        NewText();

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを表示
        backButton.gameObject.SetActive(true);    // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(true);
        thirdSection.SetActive(false);
        fourSection.SetActive(false);

        onePageImage.SetActive(false);
        twoPageImageL.SetActive(true);
        twoPageImageR.SetActive(true);
        threePageImage.SetActive(false);
        fourPageImage.SetActive(false);
    }

    private void ShowThirdSection()
    {
        currentSection = thirdSection; // 現在のセクションを更新
        NewText();

        forwardButton.gameObject.SetActive(true); // 次へ進むボタンを非表示
        backButton.gameObject.SetActive(true);     // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(false);
        thirdSection.SetActive(true);
        fourSection.SetActive(false);

        onePageImage.SetActive(false);
        twoPageImageL.SetActive(false);
        twoPageImageR.SetActive(false);
        threePageImage.SetActive(true);
        fourPageImage.SetActive(false);
    }
    private void ShowFourSection()
    {
        currentSection = fourSection; // 現在のセクションを更新
        NewText();

        forwardButton.gameObject.SetActive(false); // 次へ進むボタンを非表示
        backButton.gameObject.SetActive(true);     // 戻るボタンを表示

        firstSection.SetActive(false);
        secondSection.SetActive(false);
        thirdSection.SetActive(false);
        fourSection.SetActive(true);

        onePageImage.SetActive(false);
        twoPageImageL.SetActive(false);
        twoPageImageR.SetActive(false);
        threePageImage.SetActive(false);
        fourPageImage.SetActive(true);
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
                "移動方法はWASDと矢印キーの両方に対応。\nマウスを動かすと視点が変わり、\nALTキーを押すとマウスカーソルが出てきて\n設定を開いたりできるぞ。\n"//ここに入力
                ;
        }
        else if(currentSection == thirdSection)//チュートリアルテキスト（３ページ目）
        {
            statusText.text =
                "追い詰められてもスキルで逆転。\nEキーを押して、鬼から逃げろ‼\n鬼をなすり付けろ‼\n"//ここに入力
                ;
        }
        else if (currentSection == fourSection)//チュートリアルテキスト（４ページ目）
        {
            statusText.text =
                "マップ内では、定期的にイベントが発生。\nプレイヤーにランダムな効果を与える。\n何が起こるかは君次第‼\n"//ここに入力
                ;
        }
        else
        {
            statusText.text = "";
        }
    }
}
