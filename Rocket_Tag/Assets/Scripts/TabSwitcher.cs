using UnityEngine;
using UnityEngine.UI;

public class TabSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject skillSection;   // スキルのセクション
    [SerializeField] private GameObject costumeSection; // コスチュームのセクション
    [SerializeField] private Button skillTabButton;     // スキルタブのボタン
    [SerializeField] private Button costumeTabButton;   // コスチュームタブのボタン

    private void Start()
    {
        // 初期状態でスキルセクションを表示
        ShowSkillSection();

        // ボタンにイベントを登録
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
