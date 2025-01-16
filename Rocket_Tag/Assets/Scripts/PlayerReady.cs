using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerReady : MonoBehaviourPunCallbacks
{
    public void SetReady(bool isReady)
    {
        // CustomPropertiesに「IsReady」フラグを設定
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties["IsReady"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

        Debug.Log($"プレイヤー {PhotonNetwork.LocalPlayer.NickName} の準備完了状態: {isReady}");
    }
}
