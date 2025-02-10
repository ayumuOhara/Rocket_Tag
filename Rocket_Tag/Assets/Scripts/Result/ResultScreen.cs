using Photon.Pun;
using UnityEngine;

public class ResultScreen : MonoBehaviour
{
    public UnityEngine.UI.Text rankText;

    void Start()
    {
        ShowMyRank();
    }

    void ShowMyRank()
    {
        int myRank = 0;

        //�����̏��ʂ��J�X�^���v���p�e�B����擾
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerRank"))
        {
            myRank = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerRank"];
        }

        rankText.text = "Your Rank: " + myRank.ToString();
    }
}
