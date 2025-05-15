using UnityEngine;
using Photon.Pun;

// PUNのコールバックを受け取れるようにする為のMonoBehaviourPunCallbacks
public class PlayerController : MonoBehaviourPunCallbacks
{
    PlayerMovement playerMovement;                // プレイヤーの移動処理クラス
    PlayerRocketAction playerRocketAction;        // プレイヤーのロケットアクションクラス
    SetPlayerBool setPlayerBool;                  // bool値を管理するクラス
    SkillManager skillManager;                    // スキルを管理するクラス
    public ObserveDistance observeDistance;       // 対象との距離を測るクラス
    InputPlayerName inputPlayerName;              // 名前を入力を管理するクラス

    void Start()
    {
        if (photonView.IsMine)
        {
            playerMovement = GetComponent<PlayerMovement>();
            playerRocketAction = GetComponent<PlayerRocketAction>();
            setPlayerBool = GetComponent<SetPlayerBool>();
            skillManager = GetComponent<SkillManager>();
            observeDistance = GetComponent<ObserveDistance>();
            setPlayerBool.SetPlayerCondition();
        }
    }

    void Update()
    {
        if (photonView.IsMine && setPlayerBool.isDead == false)
        {
            if(setPlayerBool.isStun == false)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    skillManager.UseSkill();
                }

                if (setPlayerBool.hasRocket)
                {
                    playerRocketAction.RocketAction();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine && setPlayerBool.isDead == false)
        {
            if (setPlayerBool.isStun == false)
            {
                playerMovement.GetVelocity();
                playerMovement.PlayerMove();
                playerMovement.JumpAction();
            }
        }
    }
}
