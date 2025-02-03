using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class InputPlayerName : MonoBehaviour
{
    [SerializeField] TMP_InputField inputName;
    OverHeadMsg overHeadMsg;

    public void SetOverHeadMsg(OverHeadMsg _overHeadMsg)
    {
        overHeadMsg = _overHeadMsg;
    }

    public void InputName()
    {
        string playerName = inputName.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("�v���C���[�������͂���Ă��܂���B");
            return;
        }

        // PUN��NickName�ɐݒ�
        PhotonNetwork.NickName = playerName;

        // �J�X�^���v���p�e�B��ݒ�
        Hashtable playerProps = new Hashtable();
        playerProps["PlayerName"] = playerName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log($"�v���C���[����ݒ�: {playerName}");

        overHeadMsg.ShowMsg(playerName);

        // ����UI���\���ɂ���
        this.gameObject.SetActive(false);
    }
}
