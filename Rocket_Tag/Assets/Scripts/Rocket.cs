using Photon.Pun;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class Rocket : MonoBehaviourPunCallbacks
{
    enum DecreeseLevel    //  ���e�J�E���g�������x��
    {
        slowest,
        veryslow,
        slow,
        normal,
        fast,
        velyfast,
        fastest
    }
    DecreeseLevel decreeseLevel = DecreeseLevel.slowest;

    float rocketLimit = 0;
    public float rocketCount = 100;
    public float initialCount = 100;
    float vibingPower = 0.2f;
    float vibingDuration = 0.2f;
    float[] vibeStartTime = { 2, 3.2f, 6, 12, 18, 24, 35 };
    float floatStartTime = 2;
    float floatSpeed = 2;
    float possesingTime = 0;
    float explodeRiseSpeed = 18;
    float[] decreeseValue = { 0.4f, 1, 1.8f, 5, 12, 30, 100 };
    float[] decreeseUpTime = { 5, 10, 15, 20, 25, 30, 35 };
    float secToExplode = 0;
    float evacuatePos_Y = 40;
    float throwSpeed = 1;
    float returnForce = 10;
    float throwedTime = 0;
    float retrieveTime = 1.5f;
    bool isExplode = false;
    bool isThrowed = false;
    bool isReturning = false;
    bool isHoldRocket = true;
    bool isDropOut = false;

    Vector3 rocketOffset;
    Vector3 thorowRocketOffset = new Vector3(0, 3f, 0);

    [SerializeField] GameObject player;
    [SerializeField] GameObject _camera;
    GameObject rocket;
    Transform playerTransform;
    CapsuleCollider capsuleCollider; 
    Collider[] hitedColliders;
    Rigidbody playerRB;
    GameManager gameManager;
    CameraController cameraController;

    bool ForTest = true;
    Vector3 STARTPOS;
    void Start()
    { Initialize(); }
    void Update()
    {
        CountElaps();
        if (!ForTest)
        {
            if (IsVibeTime())
            { StartCoroutine(cameraController.Shake(vibingDuration, vibingPower)); }
        }
        if (isFloatingTime() && !IsStopePos())
        {
            SetGravity(playerRB, false);
            Floating(playerTransform, floatSpeed);
        }
        if (IsLimitOver())
        {
            StartCoroutine(Explosion());
            ResetRocketCount();
            ResetPossesing();
        }
        if (IsDecreeseUpTime())
        { DecreeseLevelUp(); }
        if(Input.GetKeyDown(KeyCode.E) && isHoldRocket)
        { isThrowed = true; }
        if(isThrowed)
        { ThrowRocket(); }
        else
        { 
            //hitedColliders = Physics.OverlapCapsule(rocket.transform.position - Vector3.down
            //                                        * GetCapsuleColliderLongY(capsuleCollider, true), rocket.transform.position
            //                                        + Vector3.up * GetCapsuleColliderLongY(capsuleCollider, true));
        } // �J�v�Z����������[�̔��a���o
        if(IsNeedRetrieve())
        {
            
        }
        //if (Mathf.Abs(transform.position.x - playerTransform.position.x) < 2 && isReturning)
        //{
        //    // �^���G�l���M�[��~
        //    ApproachPos(player, rocket, rocketOffset);
        //    isReturning = false;
        //    isHoldRocket = true;
        //    isThrowed = false;
        //    throwedTime = 0;
        //}
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.transform.position = STARTPOS;
        }
        if(isReturning)
        {
        }
        //if(Input.GetKey(KeyCode.LeftArrow))
        //{
        //    player.transform.position = new Vector3(playerPosX + 1, player.transform.position.y, player.transform.position.z);
        //}
    }
    void Initialize()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (ForTest)    //  TestScene�p
        {
            player = GameObject.Find("Player");
        }
        Debug.Log(gameManager);
        playerTransform = player.transform;
        STARTPOS = transform.position;
        evacuatePos_Y = GetPos_YFromStart(40);
        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
        _camera = GameObject.Find("PlayerCamera");     // �Q�[���v���C�Ŏg��
        rocket = GameObject.Find("Rocket");
        STARTPOS = rocket.transform.position;
        playerRB = player.GetComponent<Rigidbody>();
        cameraController = _camera.GetComponent<CameraController>();
        UpdateRocketCount(rocketCount);
    }
    // ���P�b�g�̃J�E���g��S�v���C���[�œ���
    public void UpdateRocketCount(float newRocketCount)
    {
        if (!ForTest)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "RocketCount", rocketCount } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
    float GetSecUntilZero(float limit, float minusValue, float runUnit)    //  �O�ɂȂ�܂ł̎��Ԃ��v�Z(minusValue��runUnit�ł̌v�Z��̌�����)
    {
        return limit / (minusValue * (1 / runUnit));
    }
    void CountElaps()    //  �o�ߕb���J�E���g
    {
        rocketCount -= Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime;
        possesingTime += Time.deltaTime;
        UpdateRocketCount(rocketCount);
        if (isThrowed) 
        { throwedTime += Time.deltaTime; }
    }


    //void Explosion()    //  ���e����
    //{
    //    // isExplode = true;
    //    if (!IsVeryHigh())
    //    {
    //        Floating(playerTransform, explodeRiseSpeed);
    //        ResetRocketCount();
    //        ResetPossesing();
    //        if (!isDropOut)
    //        {
    //            // �}�X�^�[�N���C�A���g�̂݃��P�b�g�t�^���������s
    //            if (PhotonNetwork.IsMasterClient)
    //            {
    //                gameManager.ChooseRocketPlayer();
    //            }
    //            // �v���C���[�̎��S����
    //            PhotonView targetPhotonView = player.GetComponent<PhotonView>();
    //            if (targetPhotonView != null)
    //            {
    //                targetPhotonView.RPC("SetPlayerDead", RpcTarget.All, true);
    //                targetPhotonView.RPC("SetHasRocket", RpcTarget.All, false);
    //            }
    //            isDropOut = true;
    //        }
    //    }

    //}
    IEnumerator Explosion()    //  ���P�b�g����
    {
        DropOut();

        while (!IsStopePos())
        {
            Floating(playerTransform, explodeRiseSpeed);
            yield return null;
        }
        yield break;
    }
    void DropOut()      // �E������
    {
        if (!ForTest)
        {
            // �}�X�^�[�N���C�A���g�̂݃��P�b�g�t�^���������s
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("�R���[�`���J�n");
                //StartCoroutine(gameManager.ChooseRocketPlayer());
            }
            // �v���C���[�̎��S����
            PhotonView targetPhotonView = player.GetComponent<PhotonView>();
            if (targetPhotonView != null)
            {
                targetPhotonView.RPC("SetPlayerDead", RpcTarget.All, true);
            }
        }
    }

    // ���P�b�g�̃J�E���g�����Z�b�g
    public void ResetRocketCount()
    {
        rocketCount = 1000; // �f�t�H���g�l
        UpdateRocketCount(rocketCount);
    }
    void DecreeseLevelUp()    //  ���P�b�g�J�E���g����
    {
        decreeseLevel += 1;
        Debug.Log(decreeseLevel);
        secToExplode = GetSecUntilZero(rocketCount, (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime), Time.deltaTime);
        Debug.Log(secToExplode);
    }
    public void ResetPossesing()    //  �����ɂ����鐔�l�̕ϓ����Z�b�g
    {
        possesingTime = 0;
        decreeseLevel = 0;
    }

    void ApproachPos(GameObject axis, GameObject Approcher, Vector3 offset)    //  �I�u�W�F�N�g�̈ʒu���߂Â���
    { Approcher.transform.position = axis.transform.position + offset; }

    void Floating(Transform floated, float floatForce)    //  �I�u�W�F�N�g���V
    {
        floated.position += Vector3.up * floatForce * Time.deltaTime;
    }
    Vector3 GetScreenCenterPos()    //  �J�����̃��[���h�ł̒��S���W�����߂�
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1000);
        Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);
        //Vector3 direction = (worldCenter - transform.position).normalized;
        return Camera.main.ScreenToWorldPoint(screenCenter);
    }
    void ThrowRocket()
    {
        isThrowed = true;
        isReturning = false;
        isHoldRocket = false;
        transform.position = Vector3.MoveTowards(transform.position, GetLineDir(GetScreenCenterPos(), rocket.transform.position), throwSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        string collidedObjectTag = collision.gameObject.tag;
        Debug.Log(collidedObjectTag);
        if (collidedObjectTag != "Player" && collidedObjectTag != "Ground")
        {
            isReturning = true;
        }
        if(collidedObjectTag == "Player")
        {
            //    �v���C���[�ɓ�����������
        }
    }

    Vector3 GetLineDir(Vector3 target, Vector3 current)
    {
        return target - current;
    }
    //void BomCouuntDecreese(int value)    //  ���P�b�g�J�E���g�����炷;
    //{
    //    bombCount -= value * Time.deltaTime;
    //}


    // �㏑�����ꂽ�J�E���g�𔽉f�i�R�[���o�b�N�j
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
{
        if (!ForTest)
        {
            if (changedProps.ContainsKey("RocketCount"))
            {
                rocketCount = (float)changedProps["RocketCount"];
                Debug.Log("RocketCount updated: " + rocketCount);
            }
        }
    }
    bool IsVibeTime()    //  �J�����U�����Ԃ�����
    { return vibeStartTime[(int)decreeseLevel] > rocketCount; }
    bool isFloatingTime()    //  �������Ԃ�����
    { return floatStartTime > rocketCount; }
    float GetPos_YFromStart(float farFromStartPos)    //  �J�n�ʒu������̋����ɂ���Y���W����
    { return playerTransform.position.y + farFromStartPos; }
    bool IsNeedRetrieve()
    { return throwedTime > retrieveTime; }
    public void RetriveByStraightLine()
    {
        ApproachPos(player, rocket, rocketOffset);
        isHoldRocket = true;
        isReturning = false;
        isThrowed = false;
    }
    bool IsDecreeseUpTime()
    { return decreeseLevel != DecreeseLevel.fastest && possesingTime > decreeseUpTime[(int)decreeseLevel]; }
    bool IsLimitOver()�@�@�@�@//  �J�E���g�����~�b�g���������������
    { return rocketLimit > rocketCount; }
    bool IsStopePos()    //  �����~�ʒu������
    { return playerTransform.position.y > evacuatePos_Y; }
    void SetGravity(Rigidbody rB, bool value)    //  RB��useGravity���Z�b�g
    { rB.useGravity = value; }
    float GetCapsuleColliderLongY(CapsuleCollider collider, bool devideFromCenter)
    {
        if (devideFromCenter)
        { return collider.height / 2;}
        else 
        { return collider.height; }
    }
}

//�C���K