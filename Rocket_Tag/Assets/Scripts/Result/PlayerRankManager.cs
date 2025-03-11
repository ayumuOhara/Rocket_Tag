using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerRankManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI rankText;
    public int playerRank;
    
    Hashtable playerProperties = new Hashtable();

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void SetPlayerRank()
    {
        if (gameManager == null)
        {
            Debug.LogError("instantiatePlayer is null!");
            return;
        }

        playerRank = gameManager.GetPlayerList().Count +1;
        Debug.Log("Player Rank: " + playerRank);

        if (PhotonNetwork.LocalPlayer == null)
        {
            Debug.LogError("PhotonNetwork.LocalPlayer is null!");
            return;
        }

        rankText.text = playerRank.ToString() + "位!";

        // 順位をカスタムプロパティに保存
        //Hashtable playerProperties = new Hashtable();
        //playerProperties["PlayerRank"] = playerRank;
        //PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }
    //public void ShowMyResult()
    //{
    //    int myRank = 1;

    //    //自分の順位をカスタムプロパティから取得
    //    if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerRank"))
    //    {
    //        Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerRank"]);
    //        myRank = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerRank"];
    //    }

    //    rankText.text = myRank.ToString() + "位!";
    //}

    // ロビー画面への遷移
    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }

}
