using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class InputPlayerName : MonoBehaviour
{
    [SerializeField] TMP_InputField inputName;
    [SerializeField] OverHeadMsgCreater msgCreater;

    // �v���C���[�l�[�����T�[�o�[�ɐݒ�
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

        msgCreater.MsgCreate();

        // ����UI���\���ɂ���
        this.gameObject.SetActive(false);
    }
}
