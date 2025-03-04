using Photon.Pun.Demo.PunBasics;
using Unity.VisualScripting;
using UnityEngine;

public class NewEmptyCSharpScript:MonoBehaviour 
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindFirstObjectByType<FadeManager>().StartFadeSequence();
        }
    }
}
