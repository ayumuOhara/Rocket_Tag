using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ChangeObjColor : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public List<Material> colorMaterial = new List<Material>();
    
    // �I�u�W�F�N�g�̐F�ʕύX
    [PunRPC]
    void ChangeColor(float r, float g, float b, float a)
    {
        Color newColor = new Color(r, g, b, a);
        this.gameObject.GetComponent<Renderer>().material.color = newColor;
    }

    // �I�u�W�F�N�g�̐F�ʐݒ�
    public void SetColor(int colorIdx)
    {
        photonView.RPC("ChangeColor", RpcTarget.All,
                        colorMaterial[colorIdx].color.r,
                        colorMaterial[colorIdx].color.g,
                        colorMaterial[colorIdx].color.b,
                        colorMaterial[colorIdx].color.a);
    }
}
