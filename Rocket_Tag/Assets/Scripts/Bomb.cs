using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    float bombLimit = 7.0f;
    float bombCount = 0;
    float vibeTime = 10f;
    float riseSpeed = 60f;
    float floatingTime = 2f;
    bool isExplode = false;

    Vector3 playerOffset = new Vector3(0, 5, 5);
    Vector3 explodeInpact;

    Rigidbody bombRB;
    GameObject faker;
    GameObject Camera;
    Transform fakerTransform;
    Transform cameraTransform;
    void Start()
    {
        bombRB = this.GetComponent<Rigidbody>();
        faker = GameObject.Find("Faker");    //  ‰¼player
        Camera = GameObject.Find("Main Camera");
        fakerTransform = faker.transform;
        cameraTransform = Camera.transform;
        explodeInpact = new Vector3(0.2f, cameraTransform.position.y, cameraTransform.position.z);
    }
    void Update()
    {
        CountElaps();
        if((bombLimit < bombCount + 0.5f || isExplode) && vibeTime > 0)
        {
            ExplodeVibing();
        }
        if (bombLimit < bombCount)
        {
            Explosion();
        }
        if(isExplode)
        {
            fakerTransform.transform.position = bombRB.transform.position + playerOffset;
        }
        
    }
    void Explosion()    //  ”š’e”š”­
    {
        Debug.Log(8);
        bombCount = 0;
        bombRB.linearVelocity = new Vector3(0, riseSpeed, 0);
        isExplode = true;
    }
    void CountElaps()    //  Œo‰ß•b”ƒJƒEƒ“ƒg
    {
        bombCount += Time.deltaTime;
    }
    void ExplodeVibing()    //  ”š”­uŠÔ‚ÌƒJƒƒ‰U“®
    {
        cameraTransform.transform.position = explodeInpact;
        explodeInpact.x *= -1;
        vibeTime -= Time.deltaTime;
    }
}

//C³•K