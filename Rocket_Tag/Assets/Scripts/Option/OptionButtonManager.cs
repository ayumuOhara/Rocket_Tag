using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionButtonManager : MonoBehaviour
{

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(1);
        //ロビーシーンでのみ表示する
        if(scene.name == "Lobby")
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
