 using Photon.Pun;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerRocketAction : MonoBehaviourPunCallbacks
{
    SetPlayerBool setPlayerBool;
    ObserveDistance observeDistance;
    RocketEffect rocketEffect;
    UILogManager uiLogManager;

    private void Start()
    {
        uiLogManager = GameObject.Find("UILogManager").GetComponent<UILogManager>();
        setPlayerBool = GetComponent<SetPlayerBool>();
        observeDistance = GetComponent<ObserveDistance>();
    }

    // タッチ/投擲アクション
    public void RocketAction()
    {
        // 近くのプレイヤーにロケットを渡す
        GameObject target = observeDistance.GetTargetDistance();
        SetPlayerBool otherPlayer = target.GetComponent<SetPlayerBool>();
        if (target != null && !otherPlayer.hasRocket)
        {
            // 自分の hasRocket を切り替え
            photonView.RPC("SetHasRocket", RpcTarget.All, !setPlayerBool.hasRocket);

            // ターゲットの hasRocket を切り替え
            PhotonView targetPhotonView = target.GetComponent<PhotonView>();
            if (targetPhotonView != null)
            {
                //string playerName = PhotonNetwork.NickName;
                uiLogManager.AddLog("player", UILogManager.LogType.ChangeTagger);

                targetPhotonView.RPC("SetHasRocket", RpcTarget.All, !otherPlayer.hasRocket);
                targetPhotonView.RPC("SetIsStun", RpcTarget.All, true);
                //    ロケットを取得
                //rocketEffect.RocketEffectWrapper(RocketEffect.RocketEffectProcces.SEARCH_ROCKET);
            }
        }
    }
}
