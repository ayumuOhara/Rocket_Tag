using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerRankManager : MonoBehaviour
{
    [SerializeField] InstantiatePlayer instantiatePlayer;
    public int playerRank;

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
