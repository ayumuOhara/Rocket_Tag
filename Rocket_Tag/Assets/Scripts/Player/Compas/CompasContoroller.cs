//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;                                                                          ////  プレイヤーを指し示すコンパスの管理script  ////

//public class FollowUpCompas : MonoBehaviour
//{
//    public Transform[] playersTF;
//    public Transform bombPlayerTF;
//    GameManager gameManager;
//    List<Material> compasMaterials;

//    void Start()
//    {
//        foreach(Renderer rend in GetComponentInChildren<Renderer>())
//        {
//            foreach (Material mat in rend.materials)
//            {
//                SetupMaterialWithFadeMode(mat);
//                compasMaterials.Add(mat);
//            }
//        }

//        StartCoroutine(FadeOut())
//        Initialize();
//    }
//    void Update()
//    {
//        Quaternion tmpAngle = Quaternion.LookRotation(GetCloserPlayer() - bombPlayerTF.position);
//        Quaternion tmpAngle1;
//        tmpAngle.x /= 36;
//        this.gameObject.transform.rotation = Quaternion.Lerp(bombPlayerTF.rotation, tmpAngle, 50f * Time.deltaTime);
//        //tmpAngle1 = this.gameObject.transform.rotation;
//        //tmpAngle1.x /= 36;
//    }
//    void Initialize()    //  初期化
//    {
//        bombPlayerTF = this.transform.root;
//        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
//        playersTF = gameManager.GetPlayerList().ConvertAll(x => x.transform).ToArray();
//    }
//    Vector3 GetCloserPlayer()    //  最も近いプレイヤーのトランスフォームを取得する
//    {
//        float tmpLineDis;
//        float minLineDis;
//        int closestPlayerNo = playersTF.Length;
        
//        tmpLineDis = Vector3.Distance(playersTF[closestPlayerNo].position, bombPlayerTF.position);
//        minLineDis = tmpLineDis;
        
//        for (int arrayNum = playersTF.Length - 2; arrayNum != -1; arrayNum--)
//        {
//            tmpLineDis = Vector3.Distance(playersTF[arrayNum].position, bombPlayerTF.position);
//            if (minLineDis > tmpLineDis)
//            {
//                minLineDis = tmpLineDis;
//                closestPlayerNo = arrayNum;
//            }
//        }
//        return playersTF[closestPlayerNo].position;
//    }
//    IEnumerator FadeOut(GameObject obj)    //  フェードアウト
//    {
//        float FadingTime;

//        FadingTime = 0.7f;

//        for(float elapsed = 0; elapsed < FadingTime; elapsed += Time.deltaTime)
//        {
//            float alpha = Mathf.Lerp(0f, 1f, elapsed / FadingTime);
//            {
//                foreach(Material mat in compasMaterials)
//                {
//                    Color color = mat.color;
//                    color.a = alpha;
//                    mat.color = color;
//                }
//            }
//        }
//    }
//    Vector3 GetCloserObj(Vector3 axis, Vector3[] objArray)    //  最も近いオブジェクトのポジションを取得する
//    {
//        float tmpLineDis;
//        float minLineDis;
//        int closestObjNo = 3;

//        tmpLineDis = Vector3.Distance(objArray[closestObjNo], axis);
//        minLineDis = tmpLineDis;

//        for (int arrayNum = playersTF.Length - 2; arrayNum != -1; arrayNum--)
//        {
//            tmpLineDis = Vector3.Distance(objArray[arrayNum], axis);
//            if (minLineDis > tmpLineDis)
//            {
//                minLineDis = tmpLineDis;
//                closestObjNo = arrayNum;
//            }
//        }
//        return playersTF[closestObjNo].position;
//    }
//    void SetupMaterialWithFadeMode(Material mat)
//    {
//        if (mat.shader.name != "Standard") return;

//        mat.SetFloat("_Mode", 2f);
//        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
//        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//        mat.SetInt("_ZWrite", 0);
//        mat.DisableKeyword("_ALPHATEST_ON");
//        mat.EnableKeyword("_ALPHABLEND_ON");
//        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
//        mat.renderQueue = 3000;
//    }
//}
