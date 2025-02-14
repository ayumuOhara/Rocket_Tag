//using System.Linq.Expressions;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using static Hook;
//using static UnityEngine.InputManagerEntry;

//public class PlayerSkin : MonoBehaviour    //  プレイヤースキンスクリプト
//{
//    enum PlayerSkinNo    //  プレイヤースキン処理一覧
//    {
//        DEFAUKT_SKIN,
//        SKIN_NO2,
//        SKIN_NO3,
//        SKIN_NO4,
//    }

//    Button skill;
//    Button cosutume;
//    Button skin0;
//    Button skin1;
//    Button skin2;
//    Button skin3;

//    string sceneName;
//    int skinNo;
//    bool isCosutume;

//    void Start()
//    {
//        Initialize();
//    }
//    void Update()
//    {
//        cosutume.onClick.AddListener(() =>
//        {
//            isCosutume = true;
//        });
//        skill.onClick.AddListener(() =>
//        {
//            isCosutume = false;
//        });
//        if (isCosutume)
//        {
//            SetSkinNoByButton();
//        }

//    }
//    void Initialize()     //  初期化
//    {
//        skill = GameObject.Find("SkillTabButton").GetComponent<Button>();
//        cosutume = GameObject.Find("CostumeTabButton").GetComponent<Button>();
//        skin0 = GameObject.Find("ccc").GetComponent<Button>();
//        skin1 = GameObject.Find("ddd").GetComponent<Button>();
//        skin2 = GameObject.Find("aaa").GetComponent<Button>();
//        skin3 = GameObject.Find("eee").GetComponent<Button>();
//        skinNo = 0;
//    }
//    void SetSkinNoByButton()    //  ボタン押下に応じて、スキン番号変更関数を呼ぶ
//    {
//        skin0.onClick.AddListener(() => ChangeSkinNo((int)PlayerSkinNo.DEFAUKT_SKIN));
//        skin1.onClick.AddListener(() => ChangeSkinNo((int)PlayerSkinNo.SKIN_NO2));
//        skin2.onClick.AddListener(() => ChangeSkinNo((int)PlayerSkinNo.SKIN_NO3));
//        skin3.onClick.AddListener(() => ChangeSkinNo((int)PlayerSkinNo.SKIN_NO4));
//    }
//    void ChangeSkinNo(int clickSkinNo)    //  スキン番号変更
//    {
//        skinNo = clickSkinNo;
//        PlayerPrefs.SetInt("PlayerSkinNo", skinNo);
//        PlayerPrefs.Save();
//    }
//}