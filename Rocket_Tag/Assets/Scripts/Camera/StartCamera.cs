using Photon.Pun;
using UnityEngine;
using UnityEngine.Splines;

public class StartCamera : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject gameManagerObj;

    SplineAnimate splineAnimate;
    SplineContainer container;

    public bool isEndAnim = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        container = FindObjectOfType<SplineContainer>();

        if(container != null)
        {
            splineAnimate.Container = container;
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
            PhotonView photonCam = gameManagerObj.GetComponent<PhotonView>();
            if (PhotonNetwork.IsMasterClient)
            {
                photonCam.RPC("WaitTimer", RpcTarget.All);
            }
        }
    }
}
