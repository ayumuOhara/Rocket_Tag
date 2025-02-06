using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class OverHeadMsg : MonoBehaviour
{
    SetPlayerBool spb;
    public Transform targetTran;
    GameObject playerObj;

    private void Start()
    {
        ShowMsg();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        spb = playerObj.GetComponent<SetPlayerBool>();
    }

    void Update()
    {
        transform.position = RectTransformUtility.WorldToScreenPoint(
             Camera.main,
             targetTran.position + Vector3.up);

        if(spb.isDead)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void ShowMsg()
    {
        TextMeshProUGUI playerName = GetComponent<TextMeshProUGUI>();
        playerName.text = PhotonNetwork.NickName;
    }
}