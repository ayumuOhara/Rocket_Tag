using UnityEngine;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private Text Up;
    [SerializeField] private Text Left;
    [SerializeField] private Text Right;
    [SerializeField] private Text Down;
    [SerializeField] private Text One;
    [SerializeField] private Text Two;
    [SerializeField] private Text Three;
    [SerializeField] private Text twoMain;
    [SerializeField] private Text threeMain;
    void Start()
    {
        Up.text = "↑";
        Left.text = "←";
        Right.text = "→";
        Down.text = "↓";
        One.text = "移動方法はWASDと矢印キーの両方に対応。";//ここに入力
        Two.text = "マウスを動かすと視点が変わり、\r\nALTキーを押すとマウスカーソルが出てきて設定を開いたりできるぞ。";//ここに入力
        Three.text = "Eキーを押してスキル発動‼";//ここに入力
        twoMain.text = "マップ内では、定期的にイベントが発生。\r\nプレイヤーにランダムな効果を与える。\r\n何が起こるかは君次第‼";//ここに入力
        threeMain.text = "ルールは時間経過で鬼が死亡していく鬼ごっこ式バトルロワイヤル。\r\n相手に接触することで、鬼の印の\r\nロケットを擦り付けて生き延びよう。";//ここに入力
    }
}