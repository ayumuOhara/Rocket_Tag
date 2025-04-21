using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;                                                                          ////  プレイヤーを指し示すコンパスの管理script  ////

public class FollowUpCompas : MonoBehaviour
{
    public Transform[] playersTF;
    public Transform bombPlayerTF;
    GameObject compas;
    GameManager gameManager;
    List<Material> compasMaterials;

    bool isFadeIn;

    void Awake()
    {
        Initialize();
    }
    void OnEnable()
    {
        Fading(compas, isFadeIn);
        isFadeIn = false;
    }
    void Start()
    {
        Initialize();
    }
    void Update()
    {
        if(!transform.parent.Find("Rocket").gameObject.activeSelf)
        {
            Fading(compas, isFadeIn);
        }
        Quaternion tmpAngle = Quaternion.LookRotation(GetCloserPlayer() - bombPlayerTF.position);
        //Quaternion tmpAngle1;
        tmpAngle.x /= 36;
        this.gameObject.transform.rotation = Quaternion.Lerp(bombPlayerTF.rotation, tmpAngle, 50f * Time.deltaTime);
        //tmpAngle1 = this.gameObject.transform.rotation;
        //tmpAngle1.x /= 36;
    }
    void Initialize()    //  初期化
    {
        bombPlayerTF = this.transform.root;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playersTF = gameManager.GetPlayerList().ConvertAll(x => x.transform).ToArray();
        compasMaterials = new List<Material>();

        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            foreach (Material mat in rend.materials)
            {
                SetupMaterialWithFadeMode(mat);
                compasMaterials.Add(mat);
            }
        }

        isFadeIn = true;

        Debug.Log(playersTF.Length + "show playersTF");
        for (int i = 0; i < 4; i++)
        {
            Debug.Log(playersTF[i]);
        }
        compas = this.gameObject;
    }
    Vector3 GetCloserPlayer()    //  最も近いプレイヤーのトランスフォームを取得する
    {
        float tmpLineDis;
        float minLineDis;
        int closestPlayerNo = playersTF.Length - 1;

        minLineDis = 2;
        //tmpLineDis = Vector3.Distance(playersTF[closestPlayerNo].position, bombPlayerTF.position);

        for (int arrayNum = playersTF.Length - 1; arrayNum != -1; arrayNum--)
        {
            if (playersTF[arrayNum] != bombPlayerTF)
            {
                tmpLineDis = Vector3.Distance(playersTF[arrayNum].position, bombPlayerTF.position);
                if (minLineDis > tmpLineDis)
                {
                    Debug.Log("Closest is chagned");
                    minLineDis = tmpLineDis;
                    closestPlayerNo = arrayNum;
                }
            }
        }
        Debug.Log(minLineDis);
        return playersTF[closestPlayerNo].position;
    }
    IEnumerator Fading(GameObject obj, bool fadeIn)    //  フェードアウト
    {
        float FadingTime;

        FadingTime = 0.7f;
        if (isFadeIn)
        {
            for (float elapsed = 0; elapsed < FadingTime; elapsed += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsed / FadingTime);
                {
                    foreach (Material mat in compasMaterials)
                    {
                        Color color = mat.color;
                        color.a = alpha;
                        mat.color = color;
                    }
                }
                yield return null;
            }
        }
        else
        {
            if (isFadeIn)
            {
                for (float elapsed = 0; elapsed < FadingTime; elapsed += Time.deltaTime)
                {
                    float alpha = Mathf.Lerp(0f, 1f, elapsed / FadingTime);
                    {
                        foreach (Material mat in compasMaterials)
                        {
                            Color color = mat.color;
                            color.a = alpha;
                            mat.color = color;
                        }
                    }
                    yield return null;
                }
            }
        }
    }
    Vector3 GetCloserObj(Vector3 axis, Vector3[] objArray)    //  最も近いオブジェクトのポジションを取得する
    {
        float tmpLineDis;
        float minLineDis;
        int closestObjNo = 3;

        tmpLineDis = Vector3.Distance(objArray[closestObjNo], axis);
        minLineDis = tmpLineDis;

        for (int arrayNum = playersTF.Length - 2; arrayNum != -1; arrayNum--)
        {
            tmpLineDis = Vector3.Distance(objArray[arrayNum], axis);
            if (minLineDis > tmpLineDis)
            {
                minLineDis = tmpLineDis;
                closestObjNo = arrayNum;
            }
        }
        return playersTF[closestObjNo].position;
    }
    void SetupMaterialWithFadeMode(Material mat)
    {
        if (mat.shader.name != "Standard") return;

        mat.SetFloat("_Mode", 2f);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}
