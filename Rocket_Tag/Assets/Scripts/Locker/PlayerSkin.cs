using System.Linq.Expressions;                                                               ////  ロッカーのプレイヤースキン変更スクリプト  ////
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Hook;
using static UnityEngine.InputManagerEntry;

public class PlayerSkin : MonoBehaviour    //  プレイヤースキンスクリプト
{
    enum PlayerSkinNo    //  プレイヤースキン処理一覧
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
    int tmpSkinLocation;

    void Start()
    {
        Initialize();
    }

    void Initialize()     //  初期化
    {
        SceneManager.sceneUnloaded += SaveSkinNo;
        skinPrefab = SkinGenerater._SkinPrefab;
        headTF     = GameObject.Find("Head"      ).GetComponent<Transform>();
        undress    = GameObject.Find("Undress"   ).GetComponent<Button>();
        redCap     = GameObject.Find("RedCap"    ).GetComponent<Button>();
        strawHat   = GameObject.Find("StrawHat"  ).GetComponent<Button>();
        eringi     = GameObject.Find("Eringi"    ).GetComponent<Button>();
        freeza     = GameObject.Find("Freeza"    ).GetComponent<Button>();
        bear       = GameObject.Find("Bear"      ).GetComponent<Button>();
        star       = GameObject.Find("Star"      ).GetComponent<Button>();
        SetSkinNoByButton();    //  ボタン反応追加

        tmpSkinNo = PlayerPrefs.GetInt("PlayerSkinNo", 0);
        tmpSkinLocation = PlayerPrefs.GetInt("PlayerSkinLocation", 0);
        SkinGenerate(tmpSkinLocation);
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
    }
}
//void ResourceLord()    //  Resourceフォルダ内のファイルを読み込む
//{
//    //if (skinPrefab == null)
//    //{
//    //    skinPrefab[1] = Resources.Load<GameObject>("RedCap");
//    //    skinPrefab[2] = Resources.Load<GameObject>("StrawHat");
//    //    skinPrefab[3] = Resources.Load<GameObject>("Eringi");
//    //    skinPrefab[4] = Resources.Load<GameObject>("Freeza");
//    //    skinPrefab[5] = Resources.Load<GameObject>("Bear");
//    //    skinPrefab[6] = Resources.Load<GameObject>("Star");
//    //}
//}   