using Photon.Pun;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerRocketAction : MonoBehaviourPunCallbacks
{
    SetPlayerBool setPlayerBool;
    ObserveDistance observeDistance;
    SkillManager skillManager;

    [Header("サウンド設定")]
    [SerializeField] private AudioClip SetSound; // アセットから設定する音
    private AudioSource audioSource;

    private void Start()
    {
        setPlayerBool = GetComponent<SetPlayerBool>();
        observeDistance = GetComponent<ObserveDistance>();
        skillManager = GetComponent<SkillManager>();

        // AudioSource を取得（Inspector に設定がなければ追加）
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
            // 自分の hasRocket を切り替え
            photonView.RPC("SetHasRocket", RpcTarget.All, !setPlayerBool.hasRocket);

            // ターゲットの hasRocket を切り替え
            PhotonView targetPhotonView = target.GetComponent<PhotonView>();
            SetPlayerBool otherPlayer = target.GetComponent<SetPlayerBool>();
            if (targetPhotonView != null)
            {
                targetPhotonView.RPC("SetHasRocket", RpcTarget.All, !otherPlayer.hasRocket);
                targetPhotonView.RPC("SetIsStun", RpcTarget.All, true);
            }
        }
    }

    [PunRPC]
    public void SetHasRocket(bool value)
    {
        if (setPlayerBool.hasRocket != value) // 変更があるときのみ処理
        {
            setPlayerBool.hasRocket = value;

            // 🎵 hasRocket が true になったら音を鳴らす
            if (value && audioSource != null && SetSound != null)
            {
                if (!audioSource.isPlaying) // 連続再生防止
                {
                    audioSource.clip = SetSound;
                    audioSource.Play();
                }
            }
        }
    }
}
