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
        playerMovement = GetComponent<PlayerMovement>();
        playerRocketAction = GetComponent<PlayerRocketAction>();
        setPlayerBool = GetComponent<SetPlayerBool>();
        Debug.Log("setPlayerBool : " + setPlayerBool);
        //skillManager = GetComponent<SkillManager>();
        observeDistance = GetComponent<ObserveDistance>();
        //inputPlayerName = GameObject.Find("InputPlayerName").GetComponent<InputPlayerName>();

        setPlayerBool.SetPlayerCondition();
    }

    void Update()
    {
        if (photonView.IsMine && setPlayerBool.isDead == false)
        {
            if(setPlayerBool.isStun == false)
            {
                //if(Input.GetKey(KeyCode.E))
                //{
                //    skillManager.UseSkill();
                //}

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
            if ( setPlayerBool.isStun == false /*&& 
                 inputPlayerName.isEnd == true*/    )
            {
                playerMovement.GetVelocity();
                playerMovement.PlayerMove();
                playerMovement.JumpAction();
            }
        }
    }
}
