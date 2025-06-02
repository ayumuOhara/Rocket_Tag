using Photon.Pun;
using UnityEngine;

public class ItemTransfer : MonoBehaviourPun
{
    public GameObject effectPrefab; // インスペクターでセットする or Resourcesから読み込む

    public void TransferItem(GameObject item, Transform receiver)
    {
        if (item == null || receiver == null) return;

        // アイテムを渡す
        item.transform.SetParent(receiver);
        item.transform.localPosition = Vector3.zero;

        // 全クライアントにエフェクト再生を通知
        photonView.RPC("PlayEffect", RpcTarget.All, receiver.position);
    }

    [PunRPC]
    void PlayEffect(Vector3 position)
    {
        // エフェクトPrefabがセットされていない場合はResourcesから読み込む（必要に応じて）
        if (effectPrefab == null)
        {
            effectPrefab = Resources.Load<GameObject>("Effects/YourEffectPrefabName"); // Resources/Effects にPrefabがある場合
        }

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            Destroy(effect, 0.5f); // 2秒後に破棄（必要に応じて時間調整）
        }
        else
        {
            Debug.LogWarning("Effect Prefab が見つかりません！");
        }
    }
}
