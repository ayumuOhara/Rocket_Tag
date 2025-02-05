using UnityEngine;
using UnityEngine.AI;

// NavMeshAgentコンポーネントがアタッチされていない場合アタッチ
[RequireComponent(typeof(NavMeshAgent))]
public class WaypointPatrol : MonoBehaviour
{
    [SerializeField]
    [Tooltip("巡回する地点の配列")]
    private Transform[] waypointArray;

    // NavMeshAgentコンポーネントを入れる変数
    private NavMeshAgent navMeshAgent;

    //現在の目的地
    private int currentWaypointIndex = 0;

    void Start()
    {
        // navMeshAgent変数にNavMeshAgentコンポーネントを入れる
        navMeshAgent = GetComponent<NavMeshAgent>();

        //最初の目的地を入れる
        navMeshAgent.SetDestination(waypointArray[currentWaypointIndex].position);
    }

    void Update()
    {
        //目的地に到着したら次のウェイポイントを設定
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            //目的地の番号を１に更新(右辺を剰余演算子にすることで目的地をループすることができる)
            currentWaypointIndex = (currentWaypointIndex + 1) % waypointArray.Length;

            //目的地を次の場所に設定
            navMeshAgent.SetDestination(waypointArray[currentWaypointIndex].position);
        }
    }
}
