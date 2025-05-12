//using UnityEngine;

//public class PlayerRespawnTrigger : MonoBehaviour
//{
//    [SerializeField] private string playerTag = "Player"; // プレイヤーのタグ
//    [SerializeField] private string stageTag = "Stage"; // ステージのタグ
//    [SerializeField] private float searchRadius = 10f; // ステージ検索範囲
//    [SerializeField] private float slowDuration = 3f;  // 遅くする時間（秒）


//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag(playerTag))
//        {
//            //FindFirstObjectByType<FadeManager>().StartFadeSequence();
//            Transform nearestStage = FindNearestStage(other.transform.position);
//            if (nearestStage != null)
//            {
//                Vector3 respawnPosition = nearestStage.position + Vector3.up * 1.5f; // ステージの上にリスポーン
//                other.transform.position = respawnPosition;
//                Debug.Log("プレイヤーが近くのステージにリスポーンしました");

//                // Rigidbodyの慣性をリセット
//                Rigidbody rb = other.GetComponent<Rigidbody>();
//                if (rb != null)
//                {
//                    rb.linearVelocity = Vector3.zero;
//                    rb.angularVelocity = Vector3.zero;
//                    Debug.Log("プレイヤーの慣性をリセットしました。");
//                }
//                // 移動速度を3秒間だけ半分にする
//                PlayerMovement movement = other.GetComponent<PlayerMovement>();
//                if (movement != null)
//                {
//                    StartCoroutine(TemporarilySlowPlayer(movement, slowDuration));
//                }
//            }
//            else
//            {
//                Debug.LogWarning("近くにステージが見つかりません！");
//            }
//        }
//    }
//    private Transform FindNearestStage(Vector3 playerPosition)
//    {
//        GameObject[] stages = GameObject.FindGameObjectsWithTag(stageTag);
//        Transform nearest = null;
//        float minDistance = Mathf.Infinity;

//        foreach (GameObject stage in stages)
//        {
//            float distance = Vector3.Distance(playerPosition, stage.transform.position);
//            if (distance < minDistance)
//            {
//                minDistance = distance;
//                nearest = stage.transform;
//            }
//        }

//        return nearest;

//        private IEnumerator TemporarilySlowPlayer(PlayerMovement player, float duration)
//        {
//            float originalSpeed = player.GetDefaultMoveSpeed();
//            player.SetMoveSpeed(originalSpeed * 0.5f);

//            yield return new WaitForSeconds(duration);

//            player.SetMoveSpeed(originalSpeed);
//        }
//    }
//}
using UnityEngine;
using System.Collections;

public class PlayerRespawnTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player"; // プレイヤーのタグ
    [SerializeField] private string stageTag = "Stage"; // ステージのタグ
    [SerializeField] private float searchRadius = 10f; // ステージ検索範囲
    [SerializeField] private float slowDuration = 3f;  // 遅くする時間（秒）

    [SerializeField] SkillCoolTime skillCoolTime;
    float SCL = 3.0f;

    void Start()
    {
        if (skillCoolTime == null)
        {
            skillCoolTime = GetComponentInChildren<SkillCoolTime>();
        }
        if (skillCoolTime == null)
        {
            skillCoolTime = FindObjectOfType<SkillCoolTime>();
        }
        if (skillCoolTime == null)
        {
            Debug.Log("skillCoolTimeが見つからない");
        }
        else
        {
            Debug.Log("skillCoolTime発見");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Transform nearestStage = FindNearestStage(other.transform.position);
            if (nearestStage != null)
            {
                Vector3 respawnPosition = nearestStage.position + Vector3.up * 1.5f;
                other.transform.position = respawnPosition;
                Debug.Log("プレイヤーが近くのステージにリスポーンしました");

                if (skillCoolTime.SkillCool == true)
                {
                    StartCoroutine(skillCoolTime.CoolTime(SCL));//スキルを三秒間使用不可能にする。
                }

                //Rigidbody rb = other.GetComponent<Rigidbody>();
                //if (rb != null)
                //{
                //    rb.velocity = Vector3.zero;
                //    rb.angularVelocity = Vector3.zero;
                //    Debug.Log("プレイヤーの慣性をリセットしました。");
                //}
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    Debug.Log("プレイヤーの慣性をリセットしました。");
                }

                // 移動速度を3秒間だけ半分にする
                PlayerMovement movement = other.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    StartCoroutine(TemporarilySlowPlayer(movement, slowDuration));
                }
            }
            else
            {
                Debug.LogWarning("近くにステージが見つかりません！");
            }
        }
    }

    private Transform FindNearestStage(Vector3 playerPosition)
    {
        GameObject[] stages = GameObject.FindGameObjectsWithTag(stageTag);
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject stage in stages)
        {
            float distance = Vector3.Distance(playerPosition, stage.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = stage.transform;
            }
        }

        return nearest;
    }

    private IEnumerator TemporarilySlowPlayer(PlayerMovement player, float duration)
    {
        float originalSpeed = player.GetDefaultMoveSpeed();
        player.SetMoveSpeed(originalSpeed * 0.3f);

        yield return new WaitForSeconds(duration);

        player.SetMoveSpeed(originalSpeed);
    }
}

