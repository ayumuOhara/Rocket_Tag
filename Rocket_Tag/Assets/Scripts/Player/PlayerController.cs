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
    public ChangeObjColor changeObjColor;         // オブジェクトの色変更をするクラス

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerRocketAction = GetComponent<PlayerRocketAction>();
        setPlayerBool = GetComponent<SetPlayerBool>();
        skillManager = GetComponent<SkillManager>();
        observeDistance = GetComponent<ObserveDistance>();
        changeObjColor = GetComponent<ChangeObjColor>();
    }

    void FixedUpdate()
    {
        if (photonView.IsMine && setPlayerBool.isDead == false)
        {
            if(setPlayerBool.isStun == false)
            {
                playerMovement.GetVelocity();
                playerMovement.PlayerMove();
                playerMovement.JumpAction();
            }

            if(setPlayerBool.hasRocket)
            {
                skillManager.UseSkill();
                playerRocketAction.RocketAction();
            }
        }
    }
}
