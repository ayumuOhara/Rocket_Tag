using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerReady : MonoBehaviourPunCallbacks
{
    public void SetReady(bool isReady)
    {
        // CustomProperties�ɁuIsReady�v�t���O��ݒ�
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties["IsReady"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

        Debug.Log($"�v���C���[ {PhotonNetwork.LocalPlayer.NickName} �̏����������: {isReady}");
    }
}
