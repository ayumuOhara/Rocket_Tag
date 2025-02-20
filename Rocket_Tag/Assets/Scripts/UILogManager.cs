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
            case LogType.ChangeTagger: return $"ロケットを所持 : {message}";
            case LogType.Dead        : return $"脱落 : {message}";
            case LogType.Event       : return $"イベント発生 : {message}";
            default:                   return $"存在しないタイプです。";
        }
    }
}
