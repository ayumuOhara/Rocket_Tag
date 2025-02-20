using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UILogManager : MonoBehaviourPunCallbacks
{
    public enum LogType
    {
        ChangeTagger,
        Dead,
        Event,
    }

    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private int maxLogCount = 5;

    private Queue<string> logQueue = new Queue<string>();

    public void AddLog(string message, LogType logType)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        string log = GetLogText(message, logType);

        logQueue.Enqueue(log);

        if (logQueue.Count > maxLogCount)
        {
            logQueue.Dequeue();
        }

        // ���O��S�N���C�A���g�ɑ��M
        photonView.RPC("UpdateLogDisplay", RpcTarget.All, string.Join("\n", logQueue));
    }

    [PunRPC]
    private void UpdateLogDisplay(string logTextContent)
    {
        logQueue = new Queue<string>(logTextContent.Split('\n'));
        logText.text = logTextContent;
    }

    string GetLogText(string message, LogType logType)
    {
        switch (logType)
        {
            case LogType.ChangeTagger: return $"���P�b�g������ : {message}";
            case LogType.Dead: return $"�E�� : {message}";
            case LogType.Event: return $"�C�x���g���� : {message}";
            default: return $"���݂��Ȃ��^�C�v�ł��B";
        }
    }
}
