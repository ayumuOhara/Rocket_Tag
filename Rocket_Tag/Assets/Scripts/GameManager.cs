using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public PlayerController playerController;
    [SerializeField] InstantiatePlayer instantiatePlayer;
    [SerializeField] PlayerReady playerReady;
    [SerializeField] TextMeshProUGUI playerCntText;     // Ready�������Ă���v���C���[��
    [SerializeField] GameObject[] ReadyUI;              // �}�b�`�J�n�O��UI
    private const int JOIN_CNT_MIN = 2;       // �Q���l���̍ŏ��l

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitPlayersReady());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �v���C���[�̏���������҂�
    IEnumerator WaitPlayersReady()
    {
        while (true)
        {
            int readyCount = GetReadyPlayerCount();
            playerCntText.text = $"{readyCount}/{JOIN_CNT_MIN}";

            if (CheckJoinedPlayer() && CheckAllPlayersReady())
            {
                StartGame();
                yield break;
            }

            yield return null;
        }
    }


    // �v���C���[���w��̐l���ȏ�Q�����Ă��邩
    public bool CheckJoinedPlayer()
    {
        var currentCnt = instantiatePlayer.GetCurrentPlayerCount();

        if (currentCnt >= JOIN_CNT_MIN)
        {
            return true;
        }
        
        return false;
    }

    // �Q�����Ă���v���C���[�S����Ready������
    bool CheckAllPlayersReady()
    {
        Player[] players = PhotonNetwork.PlayerList;

        // �S�v���C���[�́uIsReady�v�t���O���`�F�b�N
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

    // �Q�[���X�^�[�g
    void StartGame()
    {
        Debug.Log("�v���C���[���������̂ŃQ�[�����J�n���܂�");
        ReadyUI[0].SetActive(false);
        ReadyUI[1].SetActive(false);
        ChooseRocketPlayer();
        CheckSuvivorCnt();
    }

    // �Q�����Ă���v���C���[����P�l��I�сA���P�b�g��t�^
    void ChooseRocketPlayer()
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

    // �c��l�����m�F���A�c��P�l�ɂȂ�����I��
    IEnumerator CheckSuvivorCnt()
    {
        while (true)
        {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            if (player.Length <= 1)
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