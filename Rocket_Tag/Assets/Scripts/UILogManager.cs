using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using static UILogManager;

public class UILogManager : MonoBehaviourPunCallbacks
{
    public enum LogType
    {
        ChangeTagger,
        Dead,
        Event,
    }

    [SerializeField] private TextMeshProUGUI logText; // ���O�\���p��TextMeshPro
    [SerializeField] private int maxLogCount = 5; // �ő働�O�\����

    private Queue<string> logQueue = new Queue<string>();

    public void AddLog(string message, LogType logType)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        string log = GetLogText(message, logType);

        // ���O���L���[�ɒǉ�
        logQueue.Enqueue(log);

        // �\�����𒴂�����Â����O���폜
        if (logQueue.Count > maxLogCount)
        {
            logQueue.Dequeue();
        }

        // UI���X�V
        photonView.RPC("UpdateLogDisplay", RpcTarget.All);
    }

    [PunRPC]
    private void UpdateLogDisplay()
    {
        logText.text = string.Join("\n", logQueue);
    }

    string GetLogText(string message, LogType logType)
    {
        switch (logType)
        {
            case LogType.ChangeTagger: return $"���P�b�g������ : {message}";
            case LogType.Dead        : return $"�E�� : {message}";
            case LogType.Event       : return $"�C�x���g���� : {message}";
            default:                   return $"���݂��Ȃ��^�C�v�ł��B";
        }
    }
}
