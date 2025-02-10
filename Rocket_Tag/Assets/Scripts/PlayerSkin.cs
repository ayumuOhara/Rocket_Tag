using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSkin : MonoBehaviour    //  プレイヤースキン
{
    string sceneName;
    public Button button;
    void Start()
    {
        button = GameObject.Find("ccc").GetComponent<Button>();
    }

    void Update()
    {
        
    }
    void OnButtonClick()
    {
        Debug.Log(gameObject.name);
    }
}
