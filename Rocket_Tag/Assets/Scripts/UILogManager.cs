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

    [SerializeField] private TextMeshProUGUI logText; // ログ表示用のTextMeshPro
    [SerializeField] private int maxLogCount = 5; // 最大ログ表示数

    private Queue<string> logQueue = new Queue<string>();

    public void AddLog(string message, LogType logType)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        string log = GetLogText(message, logType);

        // ログをキューに追加
        logQueue.Enqueue(log);

        // 表示数を超えたら古いログを削除
        if (logQueue.Count > maxLogCount)
        {
            logQueue.Dequeue();
        }

        // UIを更新
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
            case LogType.ChangeTagger: return $"{message}が鬼になりました！";
            case LogType.Dead        : return $"{message}が脱落しました…";
            case LogType.Event       : return $"{message}イベントが発生しました！！";
            default:                   return $"存在しないタイプです。";
        }
    }
}
