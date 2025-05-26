using UnityEngine;
using UnityEngine.AI;

public class LinkSpeedController : MonoBehaviour
{
    public float linkTraverseSpeed = 2f; // 通過スピード（1秒あたりの移動距離）

    private NavMeshAgent agent;
    private bool isTraversingLink = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.isOnOffMeshLink && !isTraversingLink)
        {
            StartCoroutine(TraverseLink(agent.currentOffMeshLinkData));
        }
    }

    System.Collections.IEnumerator TraverseLink(OffMeshLinkData linkData)
    {
        isTraversingLink = true;

        Vector3 startPos = agent.transform.position;
        Vector3 endPos = linkData.endPos;

        // NavMeshAgentの自動移動を止める
        agent.isStopped = true;

        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / linkTraverseSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            agent.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // NavMeshAgentをリンクの終了位置に移動させ、リンク完了を通知
        agent.CompleteOffMeshLink();

        // 自動移動を再開
        agent.isStopped = false;
        isTraversingLink = false;
    }
}
