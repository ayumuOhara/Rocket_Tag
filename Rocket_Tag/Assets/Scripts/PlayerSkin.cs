using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Hook;
using static UnityEngine.InputManagerEntry;

public class PlayerSkin : MonoBehaviour    //  プレイヤースキンスクリプト
{
    internal enum PlayerSkinProcess    //  プレイヤースキン処理一覧
    {
        LockerMode
    }

    Button skill;
    Button cosutume;
    Button skin0;
    Button skin1;
    Button skin2;
    Button skin3;

    string sceneName;
    int skinNo;
    bool isCosutume;

    void Start()
    {
        Initialize();
    }
    void Update()
    {
        cosutume.onClick.AddListener(() =>
        {
            isCosutume = true;
        });
        if (isCosutume)
        {
            SetSkinNoByButton();
        }
        cosutume.onClick.AddListener(() =>
        {
            isCosutume = false;
        });
    }
//    void Initialize()     //  ロッカーシーンの初期化
//    {
//        skill = GameObject.Find()
//        cosutume = GameObject.Find("CostumeTabButton").GetComponent<Button>();
//        skin0 = GameObject.Find("ccc").GetComponent<Button>();
//        skin1 = GameObject.Find("ddd").GetComponent<Button>();
//        skin2 = GameObject.Find("aaa").GetComponent<Button>();
//        skin3 = GameObject.Find("eee").GetComponent<Button>();
//        skinNo = 0;

//    }
//    void IsPushCosutumeButton()
//    {
        
//    }
//    void SetSkinNoByButton()    //  ボタン押下に応じて、スキン番号変更関数を呼ぶ
//    {
//        skin0.onClick.AddListener(() => OnButtonClick(skin0));
//        skin1.onClick.AddListener(() => OnButtonClick(skin1));
//        skin2.onClick.AddListener(() => OnButtonClick(skin2));
//        skin3.onClick.AddListener(() => OnButtonClick(skin3));
//    }
//    void ChangeSkinNo(int clickSkinNo)    //  スキン番号変更
//    {
//        skinNo = clickSkinNo;
//        PlayerPrefs.SetInt("PlayerSkinNo",skinNo);
//        PlayerPrefs.Save();
//    }
//}