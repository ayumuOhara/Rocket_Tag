using Photon.Pun;
using UnityEngine;

public class DebugButton : MonoBehaviourPunCallbacks
{
    public void OnClick()
    {
        // "Player" タグを持つすべてのプレイヤーを取得
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("プレイヤーが見つかりません。");
            return;
        }

        // ランダムにプレイヤーを抽選
        int rnd = Random.Range(0, players.Length);
        GameObject selectedPlayer = players[rnd];

        // 抽選したプレイヤーの PhotonView を取得
        PhotonView targetPhotonView = selectedPlayer.GetComponent<PhotonView>();
        if (targetPhotonView != null)
        {
            // hasRocket を true に設定し、同期
            targetPhotonView.RPC("SetHasRocket", RpcTarget.All, true);
        }
        else
        {
            Debug.LogWarning("PhotonView が見つかりません。");
        }
    }
}
