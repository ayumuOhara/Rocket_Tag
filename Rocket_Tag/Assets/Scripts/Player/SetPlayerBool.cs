using Photon.Pun;
using UnityEngine;

public class SetPlayerBool : MonoBehaviourPunCallbacks
{
    PlayerMovement playerMovement;
    alpha_Rocket rocket;

    [SerializeField] GameObject rocketObj;            // ���P�b�g

    [SerializeField] public bool hasRocket;           // ���P�b�g���������Ă��邩
    [SerializeField] public bool isDead;              // ���S����
    [SerializeField] public bool isStun;              // �X�^������

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rocket = rocketObj.GetComponent<alpha_Rocket>();
    }

    // �v���C���[�̏�Ԃ̏�����
    public void SetPlayerCondition()
    {
        // ���P�b�g�̏�Ԃ�������
        photonView.RPC("SetHasRocket", RpcTarget.All, false);

        photonView.RPC("SetPlayerDead", RpcTarget.All, false);
        isStun = false;
    }

    // ���S����
    [PunRPC]
    public void SetPlayerDead(bool newIsDead)
    {
        isDead = newIsDead;
    }

    [PunRPC]
    public void SetIsStun(bool newIsStun)
    {
        isStun = newIsStun;
        if(isStun)
        {
            StartCoroutine(playerMovement.StunPlayer());
        }
    }

    // hasRocket ��ݒ肵�A����
    [PunRPC]
    public void SetHasRocket(bool newHasRocket)
    {
        hasRocket = newHasRocket;
        rocketObj.SetActive(hasRocket);

        rocket.ResetPossesing();
    }
}
