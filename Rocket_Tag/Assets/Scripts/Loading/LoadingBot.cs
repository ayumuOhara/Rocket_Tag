using TMPro;
using UnityEngine;

public class LoadingBot : MonoBehaviour
{

    public float speed = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (transform.position.x <= -8)
        {
            transform.position = new Vector3(7f, -0.6f, -6.5f);
        }
    }
}
