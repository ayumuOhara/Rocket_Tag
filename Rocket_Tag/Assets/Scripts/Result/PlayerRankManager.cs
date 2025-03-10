using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerRankManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public int playerRank;

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

        playerRank = gameManager.GetPlayerList().Count;
        Debug.Log("Player Rank: " + playerRank);

        if (PhotonNetwork.LocalPlayer == null)
        {
            Debug.LogError("PhotonNetwork.LocalPlayer is null!");
            return;
        }

        // 順位をカスタムプロパティに保存
        Hashtable playerProperties = new Hashtable();
        playerProperties["PlayerRank"] = playerRank;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

}
