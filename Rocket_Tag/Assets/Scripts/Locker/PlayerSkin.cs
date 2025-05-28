using System.Collections.Generic;
using System.Linq.Expressions;                                                               ////  ロッカーのプレイヤースキン変更スクリプト  ////
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class PlayerSkin : MonoBehaviour    //  プレイヤースキンスクリプト
{
    enum PlayerSkinNo    //  プレイヤースキン処理一覧                                        ////  以下宣言区    ////
    {
        NONE,
        RED_CAP,
        STRAW_HAT,
        ERINGI,
        FREEZA,
        BEAR,
        STAR,
    }

    enum SkinLocation    //  スキンの場所
    {
        HEAD,
        ARM,
        CHEST,
        LEG,
    }

    Dictionary<PlayerSkinNo, string> skinButtonNameMap ;
    GameObject[] skinPrefab;
    GameObject skinEntity;
    Transform headTF;
    Button undress;
    Button redCap;
    Button strawHat;
    Button eringi;
    Button freeza;
    Button bear;
    Button star;

    int tmpSkinNo;
    int tmpSkinLocation;                                                                     ////  宣言区終了  ////

    void Start()                                                                             ////  以下処理区  ////
    {
        Initialize();
    }                                                                                        ////  処理区終了  ////

    void Initialize()     //  初期化                                                         ////  以下関数区  ////
    {
        SceneManager.sceneUnloaded += SaveSkinNo;
        skinPrefab = SkinGenerater._SkinPrefab;
        headTF     = GameObject.Find("Head"      ).GetComponent<Transform>();
        undress = GameObject.Find(skinButtonNameMap[PlayerSkinNo.NONE]).GetComponent<Button>();
        redCap = GameObject.Find(skinButtonNameMap[PlayerSkinNo.RED_CAP]).GetComponent<Button>();
        strawHat = GameObject.Find(skinButtonNameMap[PlayerSkinNo.STRAW_HAT]).GetComponent<Button>();
        eringi = GameObject.Find(skinButtonNameMap[PlayerSkinNo.ERINGI]).GetComponent<Button>();
        freeza = GameObject.Find(skinButtonNameMap[PlayerSkinNo.FREEZA]).GetComponent<Button>();
        bear = GameObject.Find(skinButtonNameMap[PlayerSkinNo.BEAR]).GetComponent<Button>();
        star = GameObject.Find(skinButtonNameMap[PlayerSkinNo.STAR]).GetComponent<Button>();
        SetSkinNoByButton();    //  ボタン反応追加

        tmpSkinNo = PlayerPrefs.GetInt("PlayerSkinNo", 0);
        tmpSkinLocation = PlayerPrefs.GetInt("PlayerSkinLocation", 0);
        SkinGenerate(tmpSkinLocation);
    }
    void InitializeDicitonry()    //  辞書が多変数初期化
    {
        skinButtonNameMap = new Dictionary<PlayerSkinNo, string>
        {
            {PlayerSkinNo.NONE, "Undress" },
            {PlayerSkinNo.RED_CAP, "RedCap" },
            {PlayerSkinNo.STRAW_HAT, "StrawHat"},
            {PlayerSkinNo.ERINGI, "Eringi"},     //  辞書が多変数初期化途中------------------------------ここまで
            {PlayerSkinNo.FREEZA, "Freeza"},
            {PlayerSkinNo.BEAR, "Bear"},
            {PlayerSkinNo.STAR, "Star"},
        };
    }
    void SetSkinNoByButton()    //  ボタン押下に応じて、スキン番号変更関数を呼ぶ
    {
        undress.onClick.AddListener (() => ChangeSkin((int)PlayerSkinNo.NONE,      (int)SkinLocation.HEAD));
        redCap.onClick.AddListener  (() => ChangeSkin((int)PlayerSkinNo.RED_CAP,   (int)SkinLocation.HEAD));
        strawHat.onClick.AddListener(() => ChangeSkin((int)PlayerSkinNo.STRAW_HAT, (int)SkinLocation.HEAD));
        eringi.onClick.AddListener  (() => ChangeSkin((int)PlayerSkinNo.ERINGI,    (int)SkinLocation.HEAD));
        freeza.onClick.AddListener  (() => ChangeSkin((int)PlayerSkinNo.FREEZA,    (int)SkinLocation.HEAD));
        bear.onClick.AddListener    (() => ChangeSkin((int)PlayerSkinNo.BEAR,      (int)SkinLocation.HEAD));
        star.onClick.AddListener    (() => ChangeSkin((int)PlayerSkinNo.STAR,      (int)SkinLocation.HEAD));
    }
    void ChangeSkin(int clickSkinNo, int skinLocation)    //  スキン番号変更
    {
        AudioManager.Instance.PlaySE(SEManager.SEType.Button_Click); //ボタンクリック音
        if (tmpSkinNo != clickSkinNo)
        {
            tmpSkinNo = clickSkinNo;
            tmpSkinLocation = skinLocation;
            SkinGenerate(skinLocation);
        }
    }
    void SkinGenerate(int skinLocation_)    //  スキンの生成
    {
        Destroy(skinEntity);
        if (tmpSkinNo != 0)
        {
            switch (skinLocation_)
            {
                case 0:
                    {
                        skinEntity = Instantiate(skinPrefab[tmpSkinNo], headTF);
                        break;
                    }
            }
        }
    }
    void SaveSkinNo(Scene scene)    //  シーンアンロード時にスキンセーブ
    {
        SaveDate_Skin();
        SceneManager.sceneUnloaded -= SaveSkinNo;
    }
    void OnApplicationQuit()    //  途中でアプリを落としたときにスキン番号をセーブ
    {
        SaveDate_Skin();
    }
    void SaveDate_Skin()    //  スキン関係のデータをセーブ
    {
        PlayerPrefs.SetInt("PlayerSkinNo", tmpSkinNo);
        PlayerPrefs.SetInt("PlayerSkinLocation", tmpSkinLocation);
        PlayerPrefs.Save();
    }                                                                                           ////  関数区終了  ////
}                                            