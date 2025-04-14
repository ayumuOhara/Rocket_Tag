using UnityEngine;
using UnityEngine.UI;

public class Yajirusi : MonoBehaviour
{
    [SerializeField] private Text Up;
    [SerializeField] private Text Left;
    [SerializeField] private Text Right;
    [SerializeField] private Text Down;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Up.text = "ª";//‚±‚±‚É“ü—Í
        Left.text = "©";//‚±‚±‚É“ü—Í
        Right.text = "¨";//‚±‚±‚É“ü—Í
        Down.text = "«";//‚±‚±‚É“ü—Í
    }
}
