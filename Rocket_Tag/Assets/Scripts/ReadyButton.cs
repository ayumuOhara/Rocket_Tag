using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] private PlayerReady playerReady;

    public void OnReadyButtonClicked()
    {
        playerReady.SetReady(true);
        this.gameObject.SetActive(false);
    }
}
