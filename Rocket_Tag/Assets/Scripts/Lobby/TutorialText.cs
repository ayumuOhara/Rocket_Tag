using UnityEngine;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private Text One;
    [SerializeField] private Text Two;
    [SerializeField] private Text Three;
    [SerializeField] private Text Four;
    [SerializeField] private Text twoMain;
    [SerializeField] private Text threeMain;
    void Start()
    {
        One.text = "WASDと方向キーで移動";//ここに入力
        Two.text = "ESCキーでメニュー画面を開く";//ここに入力
        Three.text = "マウスで視点移動";//ここに入力
        Four.text = "Eキーを押してスキル使用";//ここに入力
        twoMain.text = "マップ内では、定期的に起きるイベントはプレイヤーに様々な影響を及ぼす。\r\nイベントを利用し、生き残ろう！\r\n";//ここに入力
        threeMain.text = "体に装着されたロケットは時間経過で宙にぶっ飛んでいく！\r\nほかのプレイヤーにロケットを押しつけて最後まで生き残れ！";//ここに入力
    }

    public void NewText(Text mainText, int N)
    {
        if(N == 1)
        {
            mainText.text = "操作方法";//ここに入力
        }
        else if(N == 2)
        {
            mainText.text = "イベントについて";//ここに入力
        }
        else if(N == 3)
        {
            mainText.text = "このゲームについて";//ここに入力
        }
    }
}