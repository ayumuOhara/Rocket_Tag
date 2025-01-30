//using Photon.Pun;
//using Photon.Realtime;
//using System.Collections;
//using System.Runtime.CompilerServices;
//using Unity.VisualScripting;
//using UnityEngine;

//public class Rocket : MonoBehaviour
//{
//    IState currentState;
//    float riseSpeed { get; set; }

//    GameObject Player;
//    Rigidbody PlayerRB;

//    public TimeManager timeMgr;

//    void Start()
//    {
//        Initialize();
//    }
//    public void ChangeState(IState newState)
//    {
//        if (currentState != null)
//        {
//            currentState.Exit(this);
//        }
//        currentState = newState;
//        currentState.Enter(this);
//    }
//    void Update()
//    {
//        currentState.Update(this);
//    }
//    void Initialize()    //  ������
//    {
//        Player = GameObject.Find("Player(Clone)");
//        Rigidbody PlayerRB = Player.GetComponent<Rigidbody>();
//        timeMgr = GameObject.Find("TimeManager").GetComponent<TimeManager>();

//        ChangeState(new NoActionState());
//    }
//    internal void Rising(float floatSpeed)    //  �v���C���[�㏸
//    {
//        Player.transform.position += Vector3.up * floatSpeed * Time.deltaTime;
//    }
//    internal void SetPlayerCravity(bool arg)
//    {
//        PlayerRB.useGravity = arg;
//    }
//    IEnumerator Explosion()
//    {
        
//        Debug.Log("���P�b�g����");
//        while (!IsVeryHigh())
//        {
//            Rising()
//            yield return null;
//        }
//        DropOut();
//    }

//}
//public interface IState
//{
//    void Enter(Rocket arg);
//    void Update(Rocket arg);
//    void Exit(Rocket arg);
//}
//public class NoActionState : IState
//{
//    public void Enter(Rocket rocket)
//    {
//    }
//    public void Update(Rocket rocket)
//    {
//        if (rocket.timeMgr.IsFloatTime())
//        {
//            rocket.ChangeState(new FloatState());
//        }
//    }
//    public void Exit(Rocket rocket)
//    {
//    }
//}
//public class FloatState : IState
//{
//    float floatSpd = 5.7f;
//    public void Enter(Rocket rocket)
//    {
//        rocket.SetPlayerCravity(false);
//    }
//    public void Update(Rocket rocket)
//    {
//        if(rocket.timeMgr.IsLimitOver())
//        {
//            rocket.ChangeState(new ExplosionState());
//        }
//    }
//    public void Exit(Rocket rocket)
//    {
        
//    }
//}
//public class ExplosionState : IState
//{
//    float riseSpd = 18.8f;
//    public void Enter(Rocket rocket)
//    {
        
//    }
//    public void Update(Rocket rocket)
//    {
        
//    }
//    public void Exit(Rocket rocket)
//    {

//    }
//}
////public class Rocket : MonoBehaviourPunCallbacks
////{
////    float vibingPower;
////    float vibingDuration;
////    float explodeRiseSpeed;
////    float evacuatePos_Y;
////    bool isExplode;
////    bool isThrowed;
////    bool isReturning;
////    bool isHoldRocket;
////    bool isDropOut;

////    //[SerializeField] GameObject player;
////    GameObject player;
////    //[SerializeField] GameObject _camera;
////    [SerializeField] GameObject _camera;
////    GameObject rocket;
////    Transform playerTransform;
////    Rigidbody playerRB;
////    GameManager gameManager;
////    TimeManager timeManager;
////    CameraController cameraController;

////    void Start()
////    {
////        Initialize();
////    }
////    void Update()
////    {
////        {
////            if (IsVibeTime())
////            {
////                StartCoroutine(cameraController.Shake(vibingDuration, vibingPower));
////            }
////        }
////        if (isFloatingTime() && !IsStopePos())
////        {
////            SetGravity(playerRB, false);
////            Floating(playerTransform, floatSpeed);
////        }
////        if (IsLimitOver())
////        {
////            StartCoroutine(Explosion());
////            ResetRocketCount();
////            ResetPossesing();
////        }
////        if (IsDecreeseUpTime())
////        {
////            DecreeseLevelUp();
////        }
////        //if (Mathf.Abs(transform.position.x - playerTransform.position.x) < 2 && isReturning)
////        //{
////        //    // �^���G�l���M�[��~
////        //    ApproachPos(player, rocket, rocketOffset);
////        //    isReturning = false;
////        //    isHoldRocket = true;
////        //    isThrowed = false;
////        //    throwedTime = 0;
////        //}
////        if (Input.GetKeyDown(KeyCode.R))
////        {
////            this.transform.position = STARTPOS;
////        }
////    }
////    void Initialize()    //  ������
////    {
////        vibingPower = 0.2f;
////        vibingDuration = 0.2f;
////        explodeRiseSpeed = 18;
////        evacuatePos_Y = 40;
////        isExplode = false;
////        isThrowed = false;
////        isReturning = false;
////        isHoldRocket = true;
////        isDropOut = false;

////        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
////        player = GameObject.Find("Player");
////        playerTransform = player.transform;
////        evacuatePos_Y = GetPos_YFromStart(40);
////        _camera = GameObject.Find("PlayerCamera");     // �Q�[���v���C�Ŏg��
////        rocket = GameObject.Find("Rocket");
////        cameraController = _camera.GetComponent<CameraController>();
////    }
////    // ���P�b�g�̃J�E���g��S�v���C���[�œ���
////    //void Explosion()    //  ���e����
////    //{
////    //    // isExplode = true;
////    //    if (!IsVeryHigh())
////    //    {
////    //        Floating(playerTransform, explodeRiseSpeed);
////    //        ResetRocketCount();
////    //        ResetPossesing();
////    //        if (!isDropOut)
////    //        {
////    //            // �}�X�^�[�N���C�A���g�̂݃��P�b�g�t�^���������s
////    //            if (PhotonNetwork.IsMasterClient)
////    //            {
////    //                gameManager.ChooseRocketPlayer();
////    //            }
////    //            // �v���C���[�̎��S����
////    //            PhotonView targetPhotonView = player.GetComponent<PhotonView>();
////    //            if (targetPhotonView != null)
////    //            {
////    //                targetPhotonView.RPC("SetPlayerDead", RpcTarget.All, true);
////    //                targetPhotonView.RPC("SetHasRocket", RpcTarget.All, false);
////    //            }
////    //            isDropOut = true;
////    //        }
////    //    }

////    //}
////    IEnumerator Explosion()    //  ���P�b�g����
////    {
////        DropOut();

////        while (!IsStopePos())
////        {
////            Floating(playerTransform, explodeRiseSpeed);
////            yield return null;
////        }
////        yield break;
////    }
////    void DropOut()
////    {
////        if (PhotonNetwork.IsMasterClient)
////        {
////            // �}�X�^�[�N���C�A���g�̂݃��P�b�g�t�^���������s
////            if (PhotonNetwork.IsMasterClient)
////            {
////                Debug.Log("�R���[�`���J�n");
////                //StartCoroutine(gameManager.ChooseRocketPlayer());
////            }
////            // �v���C���[�̎��S����
////            PhotonView targetPhotonView = player.GetComponent<PhotonView>();
////            if (targetPhotonView != null)
////            {
////                targetPhotonView.RPC("SetPlayerDead", RpcTarget.All, true);
////            }
////            gameManager.ChooseRocketPlayer();
////        }

////        PhotonView photonView = player.GetComponent<PhotonView>();
////        photonView.RPC("SetPlayerDead", RpcTarget.All, true);
////    }

////    // ���P�b�g�̃J�E���g�����Z�b�g
////    public void ResetRocketCount()
////    {
////        rocketCount = initialCount; // �f�t�H���g�l
////        UpdateRocketCount(rocketCount);
////    }
////    void DecreeseLevelUp()    //  ���P�b�g�J�E���g����
////    {
////        decreeseLevel += 1;
////        Debug.Log(decreeseLevel);
////        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
////        Debug.Log(secToExplode);
////    }
////    public void ResetPossesing()    //  �����ɂ����鐔�l�̕ϓ����Z�b�g
////    {
////        possesingTime = 0;
////        decreeseLevel = 0;
////    }



////    void Floating(Transform floated, float floatForce)    //  �I�u�W�F�N�g���V
////    {
////        floated.position += Vector3.up * floatForce * Time.deltaTime;
////    }



////    private void OnCollisionEnter(Collision collision)
////    {
////        string collidedObjectTag = collision.gameObject.tag;
////        Debug.Log(collidedObjectTag);
////        if (collidedObjectTag != "Player" && collidedObjectTag != "Ground")
////        {
////            isReturning = true;
////        }
////        if (collidedObjectTag == "Player")
////        {
////            //    �v���C���[�ɓ�����������
////        }
////    }


////    //void BomCouuntDecreese(int value)    //  ���P�b�g�J�E���g�����炷;
////    //{
////    //    bombCount -= value * Time.deltaTime;
////    //}


////    // �㏑�����ꂽ�J�E���g�𔽉f�i�R�[���o�b�N�j
////    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
////    {
////        if (!ForTest)
////        {
////            if (changedProps.ContainsKey("RocketCount"))
////            {
////                rocketCount = (float)changedProps["RocketCount"];
////                Debug.Log("RocketCount updated: " + rocketCount);
////            }
////        }
////    }
////    bool IsVibeTime()    //  �J�����U�����Ԃ�����
////    {
////        return vibeStartTime[(int)decreeseLevel] > rocketCount;
////    }
////    bool isFloatingTime()    //  �������Ԃ�����
////    {
////        return floatStartTime > rocketCount;
////    }
////    float GetPos_YFromStart(float farFromStartPos)    //  �J�n�ʒu������̋����ɂ���Y���W����
////    {
////        return playerTransform.position.y + farFromStartPos;
////    }


////    bool IsDecreeseUpTime()
////    {
////        return decreeseLevel != DecreeseLevel.fastest && possesingTime > decreeseUpTime[(int)decreeseLevel];
////    }
////    bool IsLimitOver()�@�@�@�@//  �J�E���g�����~�b�g���������������
////    {
////        return rocketLimit > rocketCount;
////    }
////    bool IsStopePos()    //  �����~�ʒu������
////    {
////        return playerTransform.position.y > evacuatePos_Y;
////    }
////    void SetGravity(Rigidbody rB, bool value)    //  RB��useGravity���Z�b�g
////    {
////        rB.useGravity = value;
////    }
////}

//////�C���K