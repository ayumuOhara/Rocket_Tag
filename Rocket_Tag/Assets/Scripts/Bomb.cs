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

    //float bombLimit = 1000;
    //float bombCount = 0;
    float bombLimit = 0;
    float bombCount = 1000;
    float vibeTime = 10;
    float riseSpeed = 60;
    float floatingTime = 2;
    float possesingTime = 0;
    float[] decreeseValue = { 0.4f, 0.8f, 100f };
    float[] decreeseUpTime = {5f, 10f, 15f };


    bool isExplode = false;

    Vector3 playerOffset = new Vector3(0, 5, 5);
    Vector3 explodeInpact;

    Rigidbody bombRB;
    [SerializeField] GameObject player;
    GameObject Camera;
    Transform playerTransform;
    Transform cameraTransform;
    void Start()
    {
        bombRB = this.GetComponent<Rigidbody>();
        player = GameObject.Find("Player");    //  âºplayer
        Camera = GameObject.Find("Main Camera");
        playerTransform = player.transform;
        cameraTransform = Camera.transform;
        explodeInpact = new Vector3(0.2f, cameraTransform.position.y, cameraTransform.position.z);
    }
    void Update()
    {
      //  Debug.Log(decreeseLevel);
        CountElaps();
        if((bombLimit > bombCount - 0.5f || isExplode) && vibeTime > 0)
        {
            ExplodeVibing();
        }
        if (bombLimit > bombCount)
        {
            Explosion();
        }
        if(isExplode)
        {
            playerTransform.transform.position = bombRB.transform.position + playerOffset;
        }
        //if(Input.GetKey(KeyCode.Space))
        //{

        //}
        DecreeseLevelUp();
    }
    void Explosion()    //  îöíeîöî≠
    {
        Debug.Log(8);
        bombRB.linearVelocity = new Vector3(0, riseSpeed, 0);
        isExplode = true;
        bombCount = 1000;
        ResetPossesing();
    }
    void CountElaps()    //  åoâﬂïbêîÉJÉEÉìÉg
    {
        bombCount -= (Time.deltaTime + decreeseValue[(int)decreeseLevel] * Time.deltaTime);
        possesingTime += Time.deltaTime;
        Debug.Log(bombCount);
    }
    void ExplodeVibing()    //  îöî≠èuä‘ÇÃÉJÉÅÉâêUìÆ
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
}

//èCê≥ïK