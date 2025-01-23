using Photon.Pun;
using UnityEngine;

public class SetPlayerBool : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerMovement playerMovement;
    Alpha_Rocket rocket;

    [SerializeField] GameObject rocketObj;            // ���P�b�g

    [SerializeField] public bool hasRocket;           // ���P�b�g���������Ă��邩
    [SerializeField] public bool isDead;              // ���S����
    [SerializeField] public bool isStun;              // �X�^������

    private void Start()
    {
        rocket = rocketObj.GetComponent<Alpha_Rocket>();
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

        playerMovement.ChangeMoveSpeed(newHasRocket,12.0f);

        //if (rocket != null) rocket.photonView.RPC("ResetPossesing", RpcTarget.All);
    }
}
