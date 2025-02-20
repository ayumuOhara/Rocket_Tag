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

        // ログを全クライアントに送信
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
            case LogType.ChangeTagger: return $"ロケットを所持 : {message}";
            case LogType.Dead: return $"脱落 : {message}";
            case LogType.Event: return $"イベント発生 : {message}";
            default: return $"存在しないタイプです。";
        }
    }
}
