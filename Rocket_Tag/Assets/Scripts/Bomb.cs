using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    enum DecreeseLevel
    {
        slow,
        normal,
        fast
    }
    
    DecreeseLevel decreeseLevel = DecreeseLevel.slow;

    float bombLimit = 0;
    float bombCount = 1000;
    float vibeTime;
    float vibeStartTime = 100f;
    float riseSpeed = 60;
    float floatingTime = 2;
    float floatForce = 200f;
    float possesingTime = 0;
    float[] decreeseValue = { 0.4f, 0.8f, 100f };
    float[] decreeseUpTime = {5f, 10f, 15f };
    bool isExplode = false;

    Vector3 playerOffset = new Vector3(0, 5, 5);
    Vector3 explodeInpact;

    Rigidbody bombRB;
    [SerializeField] GameObject player;
    GameObject camera;
    GameObject bomb;
    Transform playerTransform;
    Transform cameraTransform;
    void Start()
    {
        vibeTime = 2;
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
        
    }
    void Explosion()    //  îöíeîöî≠
    {
        if((floatingTime -= Time.deltaTime) > 0)
        {
            player.transform.Translate(0, floatForce, 0);
        }
        bombRB.linearVelocity = new Vector3(0, riseSpeed, 0);
        isExplode = true;
        bombCount = 1000;
        ResetPossesing();
    }
    void CountElaps()    //  åoâﬂïbêîÉJÉEÉìÉg
    {
        bombCount -= (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime);
        possesingTime += Time.deltaTime;
    }
    void CameraVibe()    //  îöî≠èuä‘ÇÃÉJÉÅÉâêUìÆ
    {
        cameraTransform.transform.position = explodeInpact;
        explodeInpact.x *= -1;
        vibeTime -= Time.deltaTime;
    }
    void DecreeseLevelUp()
    {
        if (decreeseLevel != DecreeseLevel.fast && possesingTime > decreeseUpTime[(int)decreeseLevel])
        {
            decreeseLevel += 1;
            Debug.Log(decreeseLevel);
        }
    }
    void ResetPossesing()
    {
        possesingTime = 0;
        decreeseLevel = 0;
    }
    void ApproachPos(GameObject axis, GameObject Approcher, Vector3 offset)
    {
        Approcher.transform.position = axis.transform.position + offset;
    }
}

//èCê≥ïK