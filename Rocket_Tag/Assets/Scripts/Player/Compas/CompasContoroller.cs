using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;                                                                          ////  プレイヤーを指し示すコンパスの管理script  ////

public class FollowUpCompas : MonoBehaviour
{
    public Transform[] playersTF;
    public Transform bombPlayerTF;
    Transform compas;
    GameManager gameManager;

    float trackSpd;
    int xRotCap;
    bool isLoaded;

    void Start()                                                                            ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }
    void Update()
    {
        Vector3[] tmpPlayerPos = playersTF.Select(tf => tf.position).ToArray();
        ChangeRot(bombPlayerTF, compas, GetCloserObj(bombPlayerTF.position, tmpPlayerPos), trackSpd, xRotCap);
    }                                                                                       ////  処理区終了  ////
    void Initialize()    //  初期化
    {
        bombPlayerTF = this.transform.root;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playersTF = gameManager.GetPlayerList().ConvertAll(x => x.transform).ToArray();
        compas = this.transform;

        trackSpd = 11f;
        xRotCap = 36;
        isLoaded = true;
    }
    void ChangeRot(Transform watcher,Transform changedObj, Vector3 target, float trackSpd, int xRotCap)    //  オブジェクトをターゲットの方向に向ける
    {
        Quaternion tmpAngle = Quaternion.LookRotation(target - watcher.position);
        tmpAngle.x /= 360;
        changedObj.rotation = Quaternion.Lerp(changedObj.rotation, tmpAngle, trackSpd * Time.deltaTime);
    }
    Vector3 GetCloserObj(Vector3 axis, Vector3[] poss)    //  最も近いプレイヤーのトランスフォームを取得する
    {
        float tmpLineDis;
        float minLineDis;
        int closestPlayerNo = poss.Length - 1;

        minLineDis = 200;

        for (int arrayNum = poss.Length - 1; arrayNum != -1; arrayNum--)
        {
            if (poss[arrayNum] != axis)
            {
                tmpLineDis = Vector3.Distance(axis, poss[arrayNum]);   //  order is wired---------------------
                if (minLineDis > tmpLineDis)
                {
                    Debug.Log("Closest is chagned");
                    minLineDis = tmpLineDis;
                    closestPlayerNo = arrayNum;
                }
            }
        }
        return poss[closestPlayerNo];
    }
}
//    internal class CompasFadin : MonoBehaviour
//    {
//        GameObject compas;
//        List<Material> compasMaterials;

//        bool isFadein;

//        void OnEnable()
//        {
//            //Fading(compas, isFadeIn);    //  for debug-----------------
//            //isFadein = false;    //  for debug-----------------
//        }
//        void Start()
//        {
//            //isFadeIn = false;    //  for debug-----------------
//        }
//        void Update()
//        {
//            //if(!transform.parent.Find("Rocket").gameObject.activeSelf)
//            //{
//            //    Fading(compas, isFadeIn);
//            //}
//        }
//        void Initialize()    //  初期化
//        {
//            compas = this.gameObject;
//            compasMaterials = new List<Material>();

//            /*  for debug----------------  */
//            //foreach (Renderer rend in GetComponentsInChildren<Renderer>())
//            //{
//            //    foreach (Material mat in rend.materials)
//            //    {
//            //        SetupMaterialWithFadeMode(mat);
//            //        compasMaterials.Add(mat);
//            //    }
//            //}
//        }

//        void SetupMaterialWithFadeMode(Material mat)    //  マテリアルモード設定
//        {
//            if (mat.shader.name != "Standard") return;

//            mat.SetFloat("_Mode", 2f);
//            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
//            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//            mat.SetInt("_ZWrite", 0);
//            mat.DisableKeyword("_ALPHATEST_ON");
//            mat.EnableKeyword("_ALPHABLEND_ON");
//            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
//            mat.renderQueue = 3000;
//        }
//        //IEnumerator Fading(GameObject obj, float FadingTime, bool fadeIn)    //  オブジェクトをフェード
//        //{
//        //    //if (isFadeIn)
//        //    //{
//        //    //    for (float elapsed = 0; elapsed < FadingTime; elapsed += Time.deltaTime)
//        //    //    {
//        //    //        float alpha = Mathf.Lerp(0f, 1f, elapsed / FadingTime);
//        //    //        {
//        //    //            foreach (Material mat in compasMaterials)
//        //    //            {
//        //    //                Color color = mat.color;
//        //    //                color.a = alpha;
//        //    //                mat.color = color;
//        //    //            }
//        //    //        }
//        //    //        yield return null;
//        //    //    }
//        //    //}
//        //    //else
//        //    //{
//        //    //    //if (isFadeIn)
//        //    //    {
//        //    //        for (float elapsed = 0; elapsed < FadingTime; elapsed += Time.deltaTime)
//        //    //        {
//        //    //            float alpha = Mathf.Lerp(0f, 1f, elapsed / FadingTime);
//        //    //            {
//        //    //                foreach (Material mat in compasMaterials)
//        //    //                {
//        //    //                    Color color = mat.color;
//        //    //                    color.a = alpha;
//        //    //                    mat.color = color;
//        //    //                }
//        //    //            }
//        //    //            yield return null;
//        //    //        }
//        //    //    }
//        //    //}
//        //}
//    }
//}
