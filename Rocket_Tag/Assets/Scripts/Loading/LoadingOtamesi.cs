using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LoadingOtamesi : MonoBehaviour
{
    [SerializeField] private GameObject Bot1;
    [SerializeField] private GameObject Bot2;
    [SerializeField] private GameObject Bot3;
    [SerializeField] private GameObject Bot4;
    [SerializeField] private GameObject MyObject;

    Animator animator;

    public float speed = 0.1f;

    float timer = 0.0f;

    bool move = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (move == true)
        {
            CMove();
        }
        else
        {
            SCharacter();
        }

        if (MyObject == Bot4)
        {
            animator.SetBool("RunTagger", true);
        }
        else
        {
            animator.SetBool("RunRunner", true);
        }
    }

    public void SCharacter()
    {
        timer += Time.deltaTime;

        if (MyObject == Bot2)
        {
            if (timer >= 1.7f)
            {
                move = true;
            }
        }
        else if (MyObject == Bot3)
        {
            if (timer >= 2.8f)
            {
                move = true;
            }
        }
        else if (MyObject == Bot4)
        {
            if (timer >= 60.0f)
            {
                move = true;
            }
        }
        else
        {
            move = true;
        }
    }
    public void CMove()//Bot‚ð“®‚©‚·
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (transform.position.x <= -8)
        {
            transform.position = new Vector3(7f, -0.6f, -6.5f);
        }
    }
}
