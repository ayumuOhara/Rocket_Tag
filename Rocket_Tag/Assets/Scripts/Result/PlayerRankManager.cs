using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerRankManager : MonoBehaviour
{
    [SerializeField] InstantiatePlayer instantiatePlayer;
    public int playerRank;

    private void Start()
    {
        instantiatePlayer = GameObject.Find("InstantiatePlayer").GetComponent<InstantiatePlayer>();
    }

    public void SetPlayerRank()
    {
        if (instantiatePlayer == null)
        {
            Debug.LogError("instantiatePlayer is null!");
            return;
        }

        playerRank = instantiatePlayer.GetCurrentPlayerCount();
        Debug.Log("Player Rank: " + playerRank);

        if (PhotonNetwork.LocalPlayer == null)
        {
            Debug.LogError("PhotonNetwork.LocalPlayer is null!");
            return;
        }

        // 順位をカスタムプロパティに保存
        Hashtable playerProperties = new Hashtable();
        playerProperties.Add("PlayerRank", playerRank);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

}
