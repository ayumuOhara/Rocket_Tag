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
        //プレイヤーの順位を設定
        playerRank = instantiatePlayer.GetCurrentPlayerCount();

        //順位をカスタムプロパティに保存
        Hashtable playerProperties = new Hashtable();
        playerProperties.Add("PlayerRank", playerRank);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }
}
