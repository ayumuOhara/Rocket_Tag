using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    enum DecreeseLevel    //  爆弾カウント減少レベル
    {
        slow,
        normal,
        fast
    }
    
    DecreeseLevel decreeseLevel = DecreeseLevel.slow;

    float rocketLimit = 0;
    public float rocketCount = 1000;
    float vibeTime;
    float vibeStartTime = 100f;
    float riseSpeed = 60;
    float floatingTime = 2;
    float floatSpeed = 1f;
    float throwForce = 50f;
    float returnForce = 10f;
    float possesingTime = 0;
    float playerPosX;
    float[] decreeseValue = { 0.4f, 0.8f, 100f };
    float[] decreeseUpTime = {5f, 10f, 15f };
    bool isExplode = false;
    bool isThrowing = false;

    Vector3 playerOffset = new Vector3(0, 5, 5);
    Vector3 rocketOffset = new Vector3(0, 5, 5);
    Vector3 explodeInpact;

    Rigidbody rocketRB;
    [SerializeField] GameObject player;
    [SerializeField] GameObject camera;
    GameObject bomb;
    Transform playerTransform;
    Transform cameraTransform;

    Vector3 startpos;
    void Start()
    {
        Time.timeScale = 0.5f;
        vibeTime = 4;
        rocketRB = this.GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        camera = GameObject.Find("Main Camera");
        bomb = GameObject.Find("Bomb");
        playerTransform = player.transform;
        cameraTransform = camera.transform;
        explodeInpact = new Vector3(0.2f, cameraTransform.position.y, cameraTransform.position.z);
        startpos = this.transform.position;
    }
    void Update()
    {
        CountElaps();
        if((vibeStartTime > rocketCount || isExplode) && vibeTime > 0)
        {
            CameraVibe();
        }
        if (rocketLimit > rocketCount)
        {
            Explosion();
        }
        if(isExplode)
        {
            ApproachPos(bomb, player, playerOffset);
        }
        DecreeseLevelUp();
        if(Input.GetKeyDown(KeyCode.E))
        {
            ThrowRocket();
        }
        if (Mathf.Abs(rocketRB.position.x - playerPosX) < 2 && isThrowing)
        {
            rocketRB.transform.position = player.transform.position + rocketOffset;
        }
        else 
        {
            isThrowing = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.transform.position = startpos;
            rocketRB.linearVelocity = new Vector3(0,0,0);
        }

        
    }
    void Explosion()    //  爆弾爆発
    {
        rocketRB.useGravity = false;
        if ((floatingTime -= Time.deltaTime) > 0)
        {
            Floating(transform, floatSpeed);
        }
        else
        {
            rocketRB.linearVelocity = new Vector3(0, riseSpeed, 0);
            isExplode = true;
            rocketCount = 1000;
            ResetPossesing();
        }
    }
    void CountElaps()    //  経過秒数カウント
    {
        rocketCount -= Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime;
        possesingTime += Time.deltaTime;
    }
    void CameraVibe()    //  ロケット爆発瞬間のカメラ振動
    {
        cameraTransform.transform.position = explodeInpact;
        explodeInpact.x *= -1;
        vibeTime -= Time.deltaTime;
    }
    void DecreeseLevelUp()    //  ロケットカウント加速
    {
        if (decreeseLevel != DecreeseLevel.fast && possesingTime > decreeseUpTime[(int)decreeseLevel])
        {
            decreeseLevel += 1;
            Debug.Log(decreeseLevel);
        }
    }
    public void ResetPossesing()    //  所持における数値の変動リセット
    {
        possesingTime = 0;
        decreeseLevel = 0;
    }
    void ApproachPos(GameObject axis, GameObject Approcher, Vector3 offset)    //  オブジェクトの位置を近づける
    {
        Approcher.transform.position = axis.transform.position + offset;
    }
    void Floating(Transform floated, float floatForce)    //  オブジェクト浮遊
    {
        floated.position += Vector3.up * floatForce * Time.deltaTime;
    }
    Vector3 GetScreenCeterPos()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1000);
        Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);
        Vector3 direction = (worldCenter - transform.position).normalized;
        return direction;
    }
    void ThrowRocket()
    {
        rocketRB.AddForce(GetScreenCeterPos() * throwForce, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
        //bool isFirstCollision = true;
        //if (isFirstCollision)
        //{
        //    isFirstCollision = false;
        //    return; // 最初の衝突は処理しない
        //}
        if (!CompareTag("Player") && !CompareTag("Ground"))
        {
            rocketRB.AddForce(GetLineDir() * returnForce, ForceMode.Impulse);
            Debug.Log(1);
           
        }
    }
    Vector3 GetLineDir()
    {
        Vector3 dir = player.transform.position - this.transform.position;
        return dir;
    }
    //void BomCouuntDecreese(int value)    //  ロケットカウントを減らす;
    //{
    //    bombCount -= value * Time.deltaTime;
    //}
}

//修正必