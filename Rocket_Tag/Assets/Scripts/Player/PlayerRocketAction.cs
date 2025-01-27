using Photon.Pun;
using UnityEngine;

public class PlayerRocketAction : MonoBehaviourPunCallbacks
{
    SetPlayerBool setPlayerBool;
    ObserveDistance observeDistance;

    private void Start()
    {
        setPlayerBool = GetComponent<SetPlayerBool>();
        observeDistance = GetComponent<ObserveDistance>();
    }

    // �^�b�`/�����A�N�V����
    public void RocketAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("���P�b�g�𓊝�����");
        }

        GameObject target = observeDistance.GetTargetDistance();
        if (target != null)
        {
            SetPlayerBool otherPlayer = target.GetComponent<SetPlayerBool>();

            // ������ hasRocket ��؂�ւ�
            photonView.RPC("SetHasRocket", RpcTarget.All, !setPlayerBool.hasRocket);

            // �^�[�Q�b�g�� hasRocket ��؂�ւ�
            PhotonView targetPhotonView = target.GetComponent<PhotonView>();
            if (targetPhotonView != null)
            {
                targetPhotonView.RPC("SetHasRocket", RpcTarget.All, !otherPlayer.hasRocket);
                targetPhotonView.RPC("SetIsStun", RpcTarget.All, true);
            }
        }
    }
}
