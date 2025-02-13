using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class ResultScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rankText;

    //void Update()
    //{
    //    ShowMyResult();
    //}

    public void ShowMyResult()
    {
        int myRank = 1;

        //自分の順位をカスタムプロパティから取得
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerRank"))
        {
            myRank = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerRank"];
        }

        rankText.text = myRank.ToString() + "位!";
        this.gameObject.SetActive(true);
    }

    // ロビー画面への遷移
    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }
}
