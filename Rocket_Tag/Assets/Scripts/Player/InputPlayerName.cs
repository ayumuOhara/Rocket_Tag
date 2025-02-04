using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class InputPlayerName : MonoBehaviour
{
    [SerializeField] TMP_InputField inputName;
    [SerializeField] OverHeadMsgCreater msgCreater;

    // プレイヤーネームをサーバーに設定
    public void InputName()
    {
        string playerName = inputName.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("プレイヤー名が入力されていません。");
            return;
        }

        // PUNのNickNameに設定
        PhotonNetwork.NickName = playerName;

        // カスタムプロパティを設定
        Hashtable playerProps = new Hashtable();
        playerProps["PlayerName"] = playerName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log($"プレイヤー名を設定: {playerName}");

        msgCreater.MsgCreate();

        // 入力UIを非表示にする
        this.gameObject.SetActive(false);
    }
}
