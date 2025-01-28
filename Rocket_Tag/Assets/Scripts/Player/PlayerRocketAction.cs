using Photon.Pun;
using UnityEngine;

public class PlayerRocketAction : MonoBehaviourPunCallbacks
{
    SetPlayerBool setPlayerBool;
    ObserveDistance observeDistance;
    SkillManager skillManager;

    private void Start()
    {
        setPlayerBool = GetComponent<SetPlayerBool>();
        observeDistance = GetComponent<ObserveDistance>();
        skillManager = GetComponent<SkillManager>();
    }

    // タッチ/投擲アクション
    public void RocketAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("ロケットを投擲した");
        }

        // 近くのプレイヤーにロケットを渡す
        GameObject target = observeDistance.GetTargetDistance();
        if (target != null)
        {
            SetPlayerBool otherPlayer = target.GetComponent<SetPlayerBool>();

            // 自分の hasRocket を切り替え
            photonView.RPC("SetHasRocket", RpcTarget.All, !setPlayerBool.hasRocket);

            // ターゲットの hasRocket を切り替え
            PhotonView targetPhotonView = target.GetComponent<PhotonView>();
            if (targetPhotonView != null)
            {
                if(skillManager.skillData.skillCode == 104)
                {
                    skillManager.HeatUpCnt();
                }                
                targetPhotonView.RPC("SetHasRocket", RpcTarget.All, !otherPlayer.hasRocket);
                targetPhotonView.RPC("SetIsStun", RpcTarget.All, true);
            }
        }
    }
}
