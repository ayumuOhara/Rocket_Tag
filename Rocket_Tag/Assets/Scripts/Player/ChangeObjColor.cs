using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ChangeObjColor : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public List<Material> colorMaterial = new List<Material>();
    // [0] DefaultBodyColor
    // [1] DefaultEyeColor
    // [2] UseSkillBodyColor
    // [3] StunBodyColor

    // プレイヤーの色変更
    [PunRPC]
    void ChangeColor(float r, float g, float b, float a)
    {
        Color newColor = new Color(r, g, b, a);
        this.gameObject.GetComponent<Renderer>().material.color = newColor;
    }
}
