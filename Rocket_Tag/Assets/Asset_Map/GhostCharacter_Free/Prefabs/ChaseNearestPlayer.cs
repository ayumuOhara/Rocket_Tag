using System.Collections.Generic;
using UnityEngine;

public class ChaseNearestPlayer : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRadius = 25f;

    private Transform targetPlayer;

    void Update()
    {
        FindNearestPlayer();

        if (targetPlayer != null)
        {
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // プレイヤーの方向を向く（必要に応じて）
            transform.LookAt(new Vector3(targetPlayer.position.x, transform.position.y, targetPlayer.position.z));
        }
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float minDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance && distance <= detectionRadius)
            {
                minDistance = distance;
                nearest = player.transform;
            }
        }

        targetPlayer = nearest;
    }
}
