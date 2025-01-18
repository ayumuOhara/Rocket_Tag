using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public PlayerController playerController;
    [SerializeField] InstantiatePlayer instantiatePlayer;
    [SerializeField] PlayerReady playerReady;
    [SerializeField] TextMeshProUGUI playerCntText;     // Ready�������Ă���v���C���[��
    [SerializeField] GameObject[] ReadyUI;              // �}�b�`�J�n�O��UI
    private const int JOIN_CNT_MIN = 2;                 // �Q���l���̍ŏ��l
    private bool isGameStarted = false;                 // �Q�[�����J�n���ꂽ���ǂ����̃t���O

    void Start()
    {
        StartCoroutine(WaitPlayersReady());
    }

    void Update()
    {

    }

    // �v���C���[�̏���������҂�
    IEnumerator WaitPlayersReady()
    {
        while (true)
        {
            int readyCount = GetReadyPlayerCount();
            playerCntText.text = $"{readyCount}/{instantiatePlayer.GetCurrentPlayerCount()}";

            // �}�X�^�[�N���C�A���g�̂݃Q�[���J�n���������s
            if (PhotonNetwork.IsMasterClient && CheckJoinedPlayer() && CheckAllPlayersReady() && !isGameStarted)
            {
                photonView.RPC(nameof(StartGame), RpcTarget.All); // �S���ɃQ�[���J�n��ʒm
                yield break;
            }

            yield return null;
        }
    }

    // �v���C���[���w��̐l���ȏ�Q�����Ă��邩
    public bool CheckJoinedPlayer()
    {
        var currentCnt = instantiatePlayer.GetCurrentPlayerCount();
        return currentCnt >= JOIN_CNT_MIN;
    }

    // �Q�����Ă���v���C���[�S����Ready������
    bool CheckAllPlayersReady()
    {
        Player[] players = PhotonNetwork.PlayerList;

        foreach (var player in players)
        {
            if (!player.CustomProperties.ContainsKey("IsReady") || !(bool)player.CustomProperties["IsReady"])
            {
                Debug.Log($"�v���C���[ {player.NickName} ���܂������������Ă��܂���");
                return false;
            }
        }

        return true;
    }

    // Ready�����������v���C���[�̐����擾
    int GetReadyPlayerCount()
    {
        Player[] players = PhotonNetwork.PlayerList;
        int readyCount = 0;

        foreach (var player in players)
        {
            if (player.CustomProperties.ContainsKey("IsReady") && (bool)player.CustomProperties["IsReady"])
            {
                readyCount++;
            }
        }

        return readyCount;
    }

    // �Q�[���X�^�[�g�i�S�N���C�A���g�ŌĂяo�����j
    [PunRPC]
    void StartGame()
    {
        if (isGameStarted) return; // ���łɃQ�[�����J�n����Ă���Ή������Ȃ�

        Debug.Log("�v���C���[���������̂ŃQ�[�����J�n���܂�");
        isGameStarted = true; // �Q�[���J�n�t���O�𗧂Ă�
        ReadyUI[0].SetActive(false);
        ReadyUI[1].SetActive(false);

        // �}�X�^�[�N���C�A���g�̂݃��P�b�g�t�^���������s
        if (PhotonNetwork.IsMasterClient)
        {
            ChooseRocketPlayer();
        }

        StartCoroutine(CheckSuvivorCnt());
    }

    // �Q�����Ă���v���C���[����P�l��I�сA���P�b�g��t�^
    void ChooseRocketPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("�v���C���[��������܂���B");
            return;
        }

        int rnd = Random.Range(0, players.Length);
        GameObject selectedPlayer = players[rnd];

        PhotonView targetPhotonView = selectedPlayer.GetComponent<PhotonView>();
        if (targetPhotonView != null)
        {
            targetPhotonView.RPC("SetHasRocket", RpcTarget.All, true);
        }
        else
        {
            Debug.LogWarning("PhotonView ��������܂���B");
        }
    }

    // �c��l�����m�F���A�c��P�l�ɂȂ�����I��
    IEnumerator CheckSuvivorCnt()
    {
        while (true)
        {
            GameObject[] suvivor = GameObject.FindGameObjectsWithTag("Player");
            var suvivorCnt = suvivor.Length;

            foreach (var player in suvivor)
            {                
                PlayerController pc = player.GetComponent<PlayerController>();
                if(pc.isDead)
                {
                    suvivorCnt--;
                }
            }

            if (suvivorCnt <= 1)
            {
                Debug.Log("�����l�����P�l�ɂȂ����̂ŃQ�[�����I�����܂�");
                ReadyUI[0].SetActive(true);
                ReadyUI[1].SetActive(true);
                playerReady.SetReady(false);
                playerController.SetPlayerCondition();
                yield break;
            }
            yield return null;
        }
    }
}
