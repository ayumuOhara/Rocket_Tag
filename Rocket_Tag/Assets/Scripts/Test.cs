using UnityEngine;

public class Test : MonoBehaviour
{
    GameObject[] frameEffectPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        frameEffectPrefab[0] = Resources.Load<GameObject>("FirstRocketFrame");
        frameEffectPrefab[1] = Resources.Load<GameObject>("SecondRocketFrame");
        frameEffectPrefab[2] = Resources.Load<GameObject>("ThirdRocketFrame");
        frameEffectPrefab[3] = Resources.Load<GameObject>("LastRocketFrame");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(frameEffectPrefab[0]);
    }
}
