using UnityEngine;
using UnityEngine.AI;

// NavMeshAgent�R���|�[�l���g���A�^�b�`����Ă��Ȃ��ꍇ�A�^�b�`
[RequireComponent(typeof(NavMeshAgent))]
public class WaypointPatrol : MonoBehaviour
{
    [SerializeField]
    [Tooltip("���񂷂�n�_�̔z��")]
    private Transform[] waypointArray;

    // NavMeshAgent�R���|�[�l���g������ϐ�
    private NavMeshAgent navMeshAgent;

    //���݂̖ړI�n
    private int currentWaypointIndex = 0;

    void Start()
    {
        // navMeshAgent�ϐ���NavMeshAgent�R���|�[�l���g������
        navMeshAgent = GetComponent<NavMeshAgent>();

        //�ŏ��̖ړI�n������
        navMeshAgent.SetDestination(waypointArray[currentWaypointIndex].position);
    }

    void Update()
    {
        //�ړI�n�ɓ��������玟�̃E�F�C�|�C���g��ݒ�
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            //�ړI�n�̔ԍ����P�ɍX�V(�E�ӂ���]���Z�q�ɂ��邱�ƂŖړI�n�����[�v���邱�Ƃ��ł���)
            currentWaypointIndex = (currentWaypointIndex + 1) % waypointArray.Length;

            //�ړI�n�����̏ꏊ�ɐݒ�
            navMeshAgent.SetDestination(waypointArray[currentWaypointIndex].position);
        }
    }
}
