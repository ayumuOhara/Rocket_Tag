using Photon.Pun;
using UnityEngine;

public class ResultScreen : MonoBehaviour
{
    public UnityEngine.UI.Text rankText;

    void Update()
    {
        ShowMyRank();
    }

    public void ShowMyRank()
    {
        int myRank = 1;

        //自分の順位をカスタムプロパティから取得
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerRank"))
        {
            myRank = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerRank"];
        }

        rankText.text = "Your Rank: " + myRank.ToString();
    }
}
