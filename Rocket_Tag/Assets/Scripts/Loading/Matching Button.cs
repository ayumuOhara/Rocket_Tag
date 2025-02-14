using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Text statusText;
    public Button button; // ボタンの参照
    private RectTransform rectTransform; // ボタンの RectTransform
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 現在の位置を取得
        Vector3 currentPosition = rectTransform.position;

        // Y軸に-1000の位置を追加
        Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y - 1000, currentPosition.z);

        // ボタンの位置を変更
        rectTransform.position = newPosition;
    }

    // Update is called once per frame
    void Update()
    {
        statusText.text = "マッチングスタート";
    }
}
