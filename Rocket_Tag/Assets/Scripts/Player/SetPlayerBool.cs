using Photon.Pun;
using TMPro;
using UnityEngine;

public class SetPlayerBool : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerMovement playerMovement;
    UILogManager uiLogManager;
    public TimeManager timeManager;
    public ResultScreen resultScreen;
    public PlayerRankManager playerRankManager;
    

    [SerializeField] GameObject rocketObj;  // ���P�b�g

    [SerializeField] public bool hasRocket; // ���P�b�g���������Ă��邩
    [SerializeField] public bool isDead;    // ���S����
    [SerializeField] public bool isStun;    // �X�^������

    private void Start()
    {
        uiLogManager      = GameObject.Find("UILogManager").GetComponent<UILogManager>();
        timeManager       = GameObject.Find("TimeManager" ).GetComponent<TimeManager>();
        resultScreen      = GameObject.Find("Result"      ).GetComponent<ResultScreen>();
        playerRankManager = GameObject.Find("GameManager" ).GetComponent<PlayerRankManager>();
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
        string playerName = PhotonNetwork.NickName;
        uiLogManager.AddLog(playerName, UILogManager.LogType.Dead);

        isDead = newIsDead;

        playerRankManager.SetPlayerRank();
        resultScreen.ShowMyResult();

        this.gameObject.SetActive(false);
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
        if (newHasRocket)
        {
            string playerName = PhotonNetwork.NickName;
            uiLogManager.AddLog(playerName, UILogManager.LogType.ChangeTagger);
        }        

        hasRocket = newHasRocket;

        rocketObj.SetActive(hasRocket);
        timeManager.ResetAcceleration();
    }
}
