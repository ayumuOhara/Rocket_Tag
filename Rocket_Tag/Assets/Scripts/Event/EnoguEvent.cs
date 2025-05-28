using NUnit.Framework;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnoguEvent : MonoBehaviourPun
{
    [SerializeField] GameObject blindEffect;               // 目つぶしイベント用UI
    [SerializeField] GameObject enogu1;               // 目つぶしイベント用UI
    [SerializeField] GameObject enogu3;               // 目つぶしイベント用UI
    [SerializeField] GameObject enogu4_1;               // 目つぶしイベント用UI
    [SerializeField] GameObject enogu4_2;               // 目つぶしイベント用UI

    [SerializeField] GameManager gameManager;

    public void PaintOpen()//成功例
    {
        List<GameObject> alivePlayers = gameManager.GetPlayerList();

        foreach (GameObject player in alivePlayers)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null)
            {
                photonView.RPC("PlayInkSE",   pv.Owner);
                photonView.RPC("BlindEffect", pv.Owner, true);
                photonView.RPC("positioning", pv.Owner);
            }
        }
    }

    [PunRPC]
    void PlayInkSE()
    {
        AudioManager.Instance.PlaySE(SEManager.SEType.Event_ink);
    }

    [PunRPC]
    void positioning()
    {
        Positioning();
    }
    public void PaintClose()
    {
        photonView.RPC("BlindEffect", RpcTarget.All, false);
    }
    [PunRPC]
    void BlindEffect(bool isBool)
    {
        enogu4_1.SetActive(false);
        enogu4_2.SetActive(false);
        blindEffect.SetActive(isBool);
    }

    public void Positioning()
    {
        // 12
        // 43  画面割り
        RectTransform rect;
        int rnd_1 = Random.Range(1,5);
        bool rand = Random.value > 0.5;

        enogu4_1.SetActive(false);
        enogu4_2.SetActive(false);

        if (rnd_1 == 1)
        {
            rect = enogu1.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                Random.Range(-500f,200f),
                Random.Range(350f,600f));//移動1
            if (rand)//1,4,23
            {
                rect = enogu3.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-200f, 200f),
                    Random.Range(-350f, -530f));//移動3

                enogu4_1.SetActive(true);
                rect = enogu4_1.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-350f, 260f),
                    Random.Range(-300f, 180f));//移動4
            }
            else//1,2,34
            {
                rect = enogu3.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(700f, 1150f),
                    Random.Range(-20f, 180f));//移動3

                enogu4_2.SetActive(true);
                rect = enogu4_2.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-600f, 750f),
                    Random.Range(-860f, -700f));//移動4
            }
        }
        else if(rnd_1 == 2)
        {
            rect = enogu1.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                Random.Range(500f, 1200f),
                Random.Range(350f, 600f));//移動1
            if (rand)//2,3,14
            {
                rect = enogu3.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(700f, 1150f),
                    Random.Range(-350f, -530f));//移動3

                enogu4_1.SetActive(true);
                rect = enogu4_1.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-1250f, -700f),
                    Random.Range(-300f, 180f));//移動4
            }
            else//2,1,34
            {
                rect = enogu3.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-200f, 200f),
                    Random.Range(-20f, 180f));//移動3

                enogu4_2.SetActive(true);
                rect = enogu4_2.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-600f, 750f),
                    Random.Range(-860f, -700f));//移動4
            }
        }
        else if (rnd_1 == 3)
        {
            rect = enogu1.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                Random.Range(500f, 1200f),
                Random.Range(-150f, 170f));//移動1
            if (rand)//3,2,14
            {
                rect = enogu3.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(700f, 1150f),
                    Random.Range(-20f, 180f));//移動3

                enogu4_1.SetActive(true);
                rect = enogu4_1.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-1250f, -700f),
                    Random.Range(-300f, 180f));//移動4
            }
            else//3,4,12
            {
                rect = enogu3.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-200f, 200f),
                    Random.Range(-350f, -530f));//移動3

                enogu4_2.SetActive(true);
                rect = enogu4_2.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-600f, 750f),
                    Random.Range(-320f, -150f));//移動4
            }
        }
        else
        {
            rect = enogu1.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                Random.Range(-500f, 200f),
                Random.Range(-150f, 170f));//移動1
            if (rand)//4,1,23
            {
                rect = enogu3.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-200f, 200f),
                    Random.Range(-20f, 180f));//移動3

                enogu4_1.SetActive(true);
                rect = enogu4_1.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-350f, 260f),
                    Random.Range(-300f, 180f));//移動4
            }
            else//4,3,12
            {
                rect = enogu3.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(700f, 1150f),
                    Random.Range(-350f, -530f));//移動3

                enogu4_2.SetActive(true);
                rect = enogu4_2.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-600f, 750f),
                    Random.Range(-320f, -150f));//移動4
            }
        }
    }
}
