using UnityEngine;

public class Bomb : MonoBehaviour
{
    float bombLimit = 10.0f;
    float bombCounter = 0;

    Rigidbody bombRB;
    GameObject faker;
    Transform fakerTransform;
    void Start()
    {
         bombRB = this.GetComponent<Rigidbody>();
        GameObject.Find("Faker");
        fakerTransform = GetComponent<Transform>();
    }
    void Update()
    {
        bombCounter += Time.deltaTime;
        if (bombLimit < bombCounter)
        {
            Explosion();
        }
    }
    void Explosion()
    {
        Debug.Log(8);
        bombRB.linearVelocity = new Vector3(0, 50f, 0);
        //fakerTransform.transform.position = this.transform.position + 
    }
}
