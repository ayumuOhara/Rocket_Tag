using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    private Rigidbody rb;
    private bool isKnockedBack = false;
    private float knockBackTime = 0.5f;
    private float timer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void KnockBack(Vector3 direction, float force)
    {
        isKnockedBack = true;
        timer = knockBackTime;

        rb.linearVelocity = Vector3.zero;  // 現在の力をリセット
        rb.linearVelocity = direction.normalized * force;  // 指定方向に飛ばす
    }

    private void Update()
    {
        if (isKnockedBack)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isKnockedBack = false;
            }
        }
    }

    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }
}