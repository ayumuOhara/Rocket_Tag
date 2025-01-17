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

    float bombLimit = 0;
    public float bombCount = 1000;
    float vibeTime;
    float vibeStartTime = 100f;
    float riseSpeed = 60;
    float floatingTime = 2;
    float floatSpeed = 1f;
    float throwForce = 100f;
    float possesingTime = 0;
    float[] decreeseValue = { 0.4f, 0.8f, 100f };
    float[] decreeseUpTime = {5f, 10f, 15f };
    bool isExplode = false;

    Vector3 playerOffset = new Vector3(0, 5, 5);
    Vector3 explodeInpact;
    Vector3 thorwDir = Vector3.forward;

    Rigidbody bombRB;
    [SerializeField] GameObject player;
    [SerializeField] GameObject camera;
    GameObject bomb;
    Transform playerTransform;
    Transform cameraTransform;
    void Start()
    {
        vibeTime = 4;
        bombRB = this.GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        camera = GameObject.Find("Main Camera");
        bomb = GameObject.Find("Bomb");
        playerTransform = player.transform;
        cameraTransform = camera.transform;
        explodeInpact = new Vector3(0.2f, cameraTransform.position.y, cameraTransform.position.z);
    }
    void Update()
    {
        CountElaps();
        if((vibeStartTime > bombCount || isExplode) && vibeTime > 0)
        {
            CameraVibe();
        }
        if (bombLimit > bombCount)
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
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Debug.Log(screenCenter);
            Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);
            Debug.Log(worldCenter);
            worldCenter.z = camera.transform.position.z * -1 ;
            Vector3 direction = (worldCenter - transform.position).normalized;
            bombRB.AddForce(direction * throwForce, ForceMode.Impulse);

            //  bombRB.AddForce(worldCenter.normalized * throwForce, ForceMode.Impulse);
            // bombRB.linearVelocity = new Vector3(0,0,100);
        }
    }
    void Explosion()    //  爆弾爆発
    {
        bombRB.useGravity = false;
        if ((floatingTime -= Time.deltaTime) > 0)
        {
            Floating(transform, floatSpeed);
        }
        else
        {
            bombRB.linearVelocity = new Vector3(0, riseSpeed, 0);
            isExplode = true;
            bombCount = 1000;
            ResetPossesing();
        }
    }
    void CountElaps()    //  経過秒数カウント
    {
        bombCount -= Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime;
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
    //void BomCouuntDecreese(int value)    //  ロケットカウントを減らす;
    //{
    //    bombCount -= value * Time.deltaTime;
    //}
}

//修正必