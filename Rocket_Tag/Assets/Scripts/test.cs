using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject smoke;
    Vector3 smokeDiffusion = new Vector3(1.005f, 1.005f, 1.005f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        smoke.transform.localScale = Vector3.Scale(smoke.transform.localScale, smokeDiffusion);
    }
}
