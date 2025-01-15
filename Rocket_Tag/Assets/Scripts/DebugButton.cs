using Photon.Pun;
using UnityEngine;

public class DebugButton : MonoBehaviourPunCallbacks
{
    public void OnClick()
    {
        // "Player" �^�O�������ׂẴv���C���[���擾
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("�v���C���[��������܂���B");
            return;
        }

        // �����_���Ƀv���C���[�𒊑I
        int rnd = Random.Range(0, players.Length);
        GameObject selectedPlayer = players[rnd];

        // ���I�����v���C���[�� PhotonView ���擾
        PhotonView targetPhotonView = selectedPlayer.GetComponent<PhotonView>();
        if (targetPhotonView != null)
        {
            // hasRocket �� true �ɐݒ肵�A����
            targetPhotonView.RPC("SetHasRocket", RpcTarget.All, true);
        }
        else
        {
            Debug.LogWarning("PhotonView ��������܂���B");
        }
    }
}
