using Photon.Pun;
using UnityEngine;
using UnityEngine.Splines;

public class StartCamera : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject gameManagerObj;

    SplineAnimate splineAnimate;
    SplineContainer container;

    public bool isEndAnim = false;

    public void Initialize()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        container = FindObjectOfType<SplineContainer>();

        if(container != null)
        {
            splineAnimate.Container = container;
            splineAnimate.Play();
        }
        else
        {
            Debug.Log("コンテナーが見つかりません");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(splineAnimate != null && splineAnimate.NormalizedTime >= 1.0f)
        {
            isEndAnim = true;
            PhotonView photonMana = gameManagerObj.GetComponent<PhotonView>();
            if (PhotonNetwork.IsMasterClient)
            {
                photonMana.RPC("WaitTimer", RpcTarget.All);
            }
        }
    }
}
