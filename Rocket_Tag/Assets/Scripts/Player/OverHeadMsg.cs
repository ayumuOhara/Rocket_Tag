using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OverHeadMsg : MonoBehaviour
{
    public Transform targetTran;

    void Update()
    {
        transform.position = RectTransformUtility.WorldToScreenPoint(
             Camera.main,
             targetTran.position + Vector3.up);
    }

    public void ShowMsg(string msg)
    {
        TextMeshProUGUI playerName = GetComponent<TextMeshProUGUI>();
        playerName.text = msg;
    }
}