using Photon.Pun;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StunPlayer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
         PhotonView targetPhotonView = target.GetComponent<PhotonView>();
         targetPhotonView.RPC("SetIsStun", RpcTarget.All, true);
    }
   
}
