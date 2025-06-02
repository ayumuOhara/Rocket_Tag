using UnityEngine;

public class RandomBackMap : MonoBehaviour
{

    [SerializeField] GameObject Map1;               // Map1
    [SerializeField] GameObject Map2;               // Map2

    void Start()
    {
        Map1.SetActive(false);
        Map2.SetActive(false);

        int rnd = Random.Range(0, 2);

        if (rnd == 0)
        {
            Map1.SetActive(true);
        }
        else if (rnd == 1)
        {
            Map2.SetActive(true);
        }
    }
}
