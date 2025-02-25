using System.Linq.Expressions;
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
    }

    enum SkinLocation    //  スキンの場所
    {
        HEAD,
        ARM,
        CHEST,
        LEG,
    }

    static GameObject[] skinPrefab;
    GameObject skinEntity;
    Transform headTF;
    Button undress;
    Button redCap;
    Button strawHat;
    Button eringi;
    Button freeza;

    int skinNo;
    static int skinLocation;

    static internal GameObject[] _SkinPrefab
    { get { return _SkinPrefab; }  }
    static internal int _SkinLocation
    { get { return skinLocation; } set { skinLocation = value; } }

    void Start()
    {
        Initialize();
    }

    void Initialize()     //  初期化
    {
        SceneManager.sceneUnloaded += SaveSkinNo;
        skinPrefab = new GameObject[5];
        ResourceLord();
        headTF     = GameObject.Find("Head"            ).GetComponent<Transform>();
        undress    = GameObject.Find("Undress"         ).GetComponent<Button>();
        redCap     = GameObject.Find("RedCap"          ).GetComponent<Button>();
        strawHat   = GameObject.Find("StrawHat"        ).GetComponent<Button>();
        eringi     = GameObject.Find("Eringi"          ).GetComponent<Button>();
        freeza     = GameObject.Find("Freeza"          ).GetComponent<Button>();
        SetSkinNoByButton();    //  ボタン反応追加

        skinNo = PlayerPrefs.GetInt("PlayerSkinNo", 0);
        Debug.Log(skinNo);
        SkinGenerate(skinLocation);
    }
    void SetSkinNoByButton()    //  ボタン押下に応じて、スキン番号変更関数を呼ぶ
    {
        undress.onClick.AddListener(() => ChangeSkin((int)PlayerSkinNo.NONE, (int)SkinLocation.HEAD));
        redCap.onClick.AddListener(() => ChangeSkin((int)PlayerSkinNo.RED_CAP, (int)SkinLocation.HEAD));
        strawHat.onClick.AddListener(() => ChangeSkin((int)PlayerSkinNo.STRAW_HAT, (int)SkinLocation.HEAD));
        eringi.onClick.AddListener(() => ChangeSkin((int)PlayerSkinNo.ERINGI, (int)SkinLocation.HEAD));
        freeza.onClick.AddListener(() => ChangeSkin((int)PlayerSkinNo.FREEZA, (int)SkinLocation.HEAD));
        Debug.Log(3);
    }
    void ChangeSkin(int clickSkinNo, int skinLocation)    //  スキン番号変更
    {
        if(clickSkinNo == 0)
        {
            skinNo = clickSkinNo;
            Destroy(skinEntity);
        }
        if (skinNo != clickSkinNo)
        {
            skinNo = clickSkinNo;
            SkinGenerate(skinLocation);
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
        }
    }
    void SkinGenerate(int skinLocation_)    //  スキンの生成
    {
        if(skinNo == 0)
        {
            Destroy(skinEntity);
        }
        else
        {
            switch (skinLocation)
            {
                case 0:
                    {
                        Destroy(skinEntity);
                        skinEntity = Instantiate(skinPrefab[skinNo], headTF);
                        break;
                    }
            }
        }
        skinLocation = skinLocation_;
    }
    void SaveSkinNo(Scene scene)    //  シーンアンロード時にスキンセーブ
    {
        PlayerPrefs.SetInt("PlayerSkinNo", skinNo);
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.GetInt("PlayerSkinNo"));
        SceneManager.sceneUnloaded -= SaveSkinNo;
    }
    void OnApplicationQuit()    //  途中でアプリを落としたときにスキン番号をセーブ
    {
        PlayerPrefs.SetInt("PlayerSkinNo", skinNo);
        PlayerPrefs.Save();
    }
}

////////////////////////////////////////////  スキン生成方法  //////////////////////////////////

/* 下記の関数を定義して
    void SkinGenerate(int skinLocation_)    //  スキンの生成
    {
        int tmpSkinNo = PlayerPrefs.GetInt("PlayerSkinNo", 0);
        if (tmpSkinNo != 0)
        {
            switch (skinLocation_)
            {
                case 0:
                    {
                        skinEntity = Instantiate(PlayerSkin._SkinPrefab , head);
                        break;
                    }
            }
        }
        PlayerSkin._SkinLocation = skinLocation_;
    }
 * こいつで生成    SkinGenerate(PlayerSkin._PlayerSkin._SkinLocation);
   ※※　　HeadのTransformとスキンのエンティティがないと生成されません  ※※
*/