using UnityEngine;

public class OverHeadMsgCreater : MonoBehaviour
{

    public RectTransform canvasRect;

    [SerializeField] OverHeadMsg overHeadMsgPrefab;
    [SerializeField] InputPlayerName inputPlayerName;

    OverHeadMsg overHeadMsg;

    void Start()
    {
        overHeadMsg = Instantiate(overHeadMsgPrefab, canvasRect);
        overHeadMsg.targetTran = transform;
        inputPlayerName.SetOverHeadMsg(overHeadMsg);
    }

    void OnEnable()
    {
        if (overHeadMsg == null) return;

        overHeadMsg.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        if (overHeadMsg == null) return;

        overHeadMsg.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (overHeadMsg == null) return;

        Destroy(overHeadMsg.gameObject);
    }
}