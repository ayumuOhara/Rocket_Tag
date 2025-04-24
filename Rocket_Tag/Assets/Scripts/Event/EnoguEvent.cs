using Photon.Pun;
using System.Collections;
using UnityEngine;

public class EnoguEvent : MonoBehaviourPun
{
    [SerializeField] GameObject paintUIGroup;               // 目つぶしイベント用UI
    [SerializeField] GameObject blindEffect;               // 目つぶしイベント用UI
    [SerializeField] GameObject enogu1;               // 目つぶしイベント用UI
    [SerializeField] GameObject enogu3;               // 目つぶしイベント用UI
    [SerializeField] GameObject enogu4_1;               // 目つぶしイベント用UI
    [SerializeField] GameObject enogu4_2;               // 目つぶしイベント用UI
    [SerializeField] SetPlayerBool setPlayerBool;
    private bool alreadyHidden = false;

    void Start()
    {
        if (photonView.IsMine)
        {
            if (setPlayerBool == null)
            {
                setPlayerBool = GetComponentInChildren<SetPlayerBool>();
            }

            if (setPlayerBool == null)
            {
                setPlayerBool = FindObjectOfType<SetPlayerBool>();
            }

            if (setPlayerBool == null)
            {
                Debug.LogWarning("SetPlayerBool が見つかりませんでした！");
            }
        }
    }

    void Update()
    {
        if (!alreadyHidden && setPlayerBool != null && setPlayerBool.isDead && photonView.IsMine)
        {
            if (paintUIGroup != null)
            {
                paintUIGroup.SetActive(false);
                alreadyHidden = true; // 一度だけ実行
            }
        }
    }

        public void PaintOpen()
    {
        photonView.RPC("DoPaintOpen", RpcTarget.All);
        photonView.RPC("BlindEffect", RpcTarget.All, true);
        Debug.Log("表示");
    }

    [PunRPC]
    void DoPaintOpen()
    {
        Positioning();
    }

    public void PaintClose()
    {
        photonView.RPC("Enogu4_1", RpcTarget.All, false);
        photonView.RPC("Enogu4_2", RpcTarget.All, false);
        photonView.RPC("BlindEffect", RpcTarget.All, false);
        Debug.Log("非表示");
    }

    [PunRPC]
    void BlindEffect(bool isBlind)
    {
        blindEffect.SetActive(isBlind);
    }

    [PunRPC]
    void Enogu4_1(bool isBlind)
    {
        enogu4_1.SetActive(isBlind);
    }

    [PunRPC]
    void Enogu4_2(bool isBlind)
    {
        enogu4_2.SetActive(isBlind);
    }

    public void Positioning()
    {
        // 12
        // 43  画面割り
        RectTransform rect;
        int rnd_1 = Random.Range(1,5);
        bool rand = Random.value > 0.5;

        if(rnd_1 == 1)
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

                photonView.RPC("Enogu4_1", RpcTarget.All, true);
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

                photonView.RPC("Enogu4_2", RpcTarget.All, true);
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

                photonView.RPC("Enogu4_1", RpcTarget.All, true);
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

                photonView.RPC("Enogu4_2", RpcTarget.All, true);
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

                photonView.RPC("Enogu4_1", RpcTarget.All, true);
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

                photonView.RPC("Enogu4_2", RpcTarget.All, true);
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

                photonView.RPC("Enogu4_1", RpcTarget.All, true);
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

                photonView.RPC("Enogu4_2", RpcTarget.All, true);
                rect = enogu4_2.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-600f, 750f),
                    Random.Range(-320f, -150f));//移動4
            }
        }
    }
}
