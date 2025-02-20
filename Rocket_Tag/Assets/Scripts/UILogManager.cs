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
        UpdateLogDisplay();
    }

    private void UpdateLogDisplay()
    {
        logText.text = string.Join("\n", logQueue);
    }

    string GetLogText(string message, LogType logType)
    {
        switch (logType)
        {
            case LogType.ChangeTagger: return $"{message}���S�ɂȂ�܂����I";
            case LogType.Dead        : return $"{message}���E�����܂����c";
            case LogType.Event       : return $"{message}�C�x���g���������܂����I�I";
            default:                   return $"���݂��Ȃ��^�C�v�ł��B";
        }
    }
}
