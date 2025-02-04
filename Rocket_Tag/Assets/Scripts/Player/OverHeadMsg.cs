using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class OverHeadMsg : MonoBehaviour
{
    public Transform targetTran;

    private void Start()
    {
        ShowMsg();
    }

    void Update()
    {
        transform.position = RectTransformUtility.WorldToScreenPoint(
             Camera.main,
             targetTran.position + Vector3.up);
    }

    public void ShowMsg()
    {
        TextMeshProUGUI playerName = GetComponent<TextMeshProUGUI>();
        playerName.text = PhotonNetwork.NickName;
    }
}