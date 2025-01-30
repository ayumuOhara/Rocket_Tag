using Photon.Pun;
using UnityEngine;

public class SetPlayerBool : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerMovement playerMovement;
    public TimeManager timeManager;

    [SerializeField] GameObject rocketObj;            // ���P�b�g

    [SerializeField] public bool hasRocket;           // ���P�b�g���������Ă��邩
    [SerializeField] public bool isDead;              // ���S����
    [SerializeField] public bool isStun;              // �X�^������

    private void Start()
    {
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
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
        Debug.Log($"TimeManager�F{timeManager}");
        timeManager.ResetAcceleration();

        playerMovement.ChangeMoveSpeed(newHasRocket,12.0f);
    }
}
