using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerRankManager : MonoBehaviour
{
    public int playerRank;

    void Start()
    {
        //プレイヤーの順位を設定
        playerRank = Random.Range(0, 10);//仮

        //順位をカスタムプロパティに保存
        Hashtable playerProperties = new Hashtable();
        playerProperties.Add("PlayerRank", playerRank);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }
}
