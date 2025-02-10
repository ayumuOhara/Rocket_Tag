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

    // �^�b�`/�����A�N�V����
    public void RocketAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("���P�b�g�𓊝�����");
        }

        // �߂��̃v���C���[�Ƀ��P�b�g��n��
        GameObject target = observeDistance.GetTargetDistance();
        if (target != null)
        {
            // ������ hasRocket ��؂�ւ�
            photonView.RPC("SetHasRocket", RpcTarget.All, !setPlayerBool.hasRocket);

            // �^�[�Q�b�g�� hasRocket ��؂�ւ�
            PhotonView targetPhotonView = target.GetComponent<PhotonView>();
            SetPlayerBool otherPlayer = target.GetComponent<SetPlayerBool>();
            if (targetPhotonView != null)
            {
                targetPhotonView.RPC("SetHasRocket", RpcTarget.All, !otherPlayer.hasRocket);
                targetPhotonView.RPC("SetIsStun", RpcTarget.All, true);
            }
        }
    }
}
