using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class GameManager : MonoBehaviourPun
{
    [SerializeField] InstantiatePlayer instantiatePlayer;
    private const int JOIN_CNT_MIN = 4;       // 参加人数の最小値
    bool isStart = false;                     // ゲームが始まっているか

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            CheckSuvivorCnt();
        }
    }

    // プレイヤーが指定の人数以上参加しているか
    public void CheckJoinedPlayer()
    {
        var currentCnt = instantiatePlayer.GetCurrentPlayerCount();
        Debug.Log($"現在の参加人数：{currentCnt}");

        if (currentCnt >= JOIN_CNT_MIN)       // 全員が準備完了を押したら開始にする
        {
            Debug.Log("プレイヤーが揃ったのでゲームを開始します");
            isStart = true;
            ChooseRocketPlayer();
        }
        else
        {
            Debug.Log("プレイヤーが揃うまで待機します");
        }
    }

    // 参加しているプレイヤーから１人を選び、ロケットを付与
    void ChooseRocketPlayer()
    {
        // "Player" タグを持つすべてのプレイヤーを取得
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("プレイヤーが見つかりません。");
            return;
        }

        // ランダムにプレイヤーを抽選
        int rnd = Random.Range(0, players.Length);
        GameObject selectedPlayer = players[rnd];

        // 抽選したプレイヤーの PhotonView を取得
        PhotonView targetPhotonView = selectedPlayer.GetComponent<PhotonView>();
        if (targetPhotonView != null)
        {
            // hasRocket を true に設定し、同期
            targetPhotonView.RPC("SetHasRocket", RpcTarget.All, true);
        }
        else
        {
            Debug.LogWarning("PhotonView が見つかりません。");
        }
    }

    // 生き残っている人が何人か判定
    void CheckSuvivorCnt()
    {
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        if (player.Length == 1)
        {
            Debug.Log("生存人数が１人になったのでゲームを終了します");
            isStart = false;
        }
        else
        {
            Debug.Log("生存人数が残１人になるまで待機");
        }
    }
}