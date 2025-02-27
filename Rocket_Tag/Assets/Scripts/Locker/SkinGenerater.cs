using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
////  ロケットエフェクト生成・切り替え  ////
public class SkinGanarater : MonoBehaviour
{
    internal enum SkinGenerateProcces    //  スキンジェネレート内の処理一覧
    {
        IN_GAME_GENERATE,
    }

    static GameObject[] skinPrefab;                                                        ////  以下宣言区  ////
    GameObject skinEntity;
    List<GameObject> TmpPlayerList;
    Transform playerHipTF;
    GameManager gameManager;
    static int skinLocation;

    static internal GameObject[] _SkinPrefab
    { get { return skinPrefab; } }                                                        ////  宣言区終了  ////                 
    void Start()                                                                           ////  以下処理区  ////
    {
        Initialize();    //  初期化
    }                                                                                      ////  処理区終了  ////
    void Initialize()     //  初期化                                                       ////  以下関数区  ////
    {
        skinPrefab = new GameObject[7];
        ResourceLord();
        playerHipTF = GameObject.Find("Hip").GetComponent<Transform>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        SkinGenerate(playerHipTF);
    }
    internal void SkinGenerateWrapper(SkinGenerateProcces skinGenerateProcces)   // ロケットエフェクトのラッパー関数
    {
        switch (skinGenerateProcces)
        {
            case SkinGenerateProcces.IN_GAME_GENERATE:    //  スキン生成処理群
                {
                    List<GameObject> tmpPlayerList = gameManager.GetPlayerList();
                    for (int tmpPlayerListLen = 0; tmpPlayerListLen == tmpPlayerList.Count - 1; tmpPlayerListLen++)
                    {
                        SkinGenerate(tmpPlayerList[tmpPlayerListLen].transform);
                    }
                    break;
                }
        }
    }
    void SkinGenerate(Transform playerHipTF_)    //  プレイヤーのスキンの生成
    {
        int tmpSkinNo = PlayerPrefs.GetInt("PlayerSkinNo", 0);
        int tmpSkinLocation = PlayerPrefs.GetInt("PlayerSkinLocation", 0);

        if (tmpSkinNo != 0)
        {
            switch (tmpSkinLocation)
            {
                case 0:
                    {
                        skinEntity = Instantiate(skinPrefab[tmpSkinNo], playerHipTF.Find("Spine/Head"));
                        break;
                    }
            }
        }
    }
    void ResourceLord()    //  Resourceフォルダ内のファイルを読み込む
    {
        if (skinPrefab[1] == null)
        {
            skinPrefab[1] = Resources.Load<GameObject>("RedCap");
            skinPrefab[2] = Resources.Load<GameObject>("StrawHat");
            skinPrefab[3] = Resources.Load<GameObject>("Eringi");
            skinPrefab[4] = Resources.Load<GameObject>("Freeza");
            skinPrefab[5] = Resources.Load<GameObject>("Bear");
            skinPrefab[6] = Resources.Load<GameObject>("Star");
        }
    }                                                                                      ////  関数区終了  ////
}