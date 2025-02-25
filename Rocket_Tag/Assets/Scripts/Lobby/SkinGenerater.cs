using UnityEngine;
using UnityEngine.SceneManagement;

public class SkinGanarater : MonoBehaviour
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
    Transform headTF;
    GameObject skinEntity;
    static int skinLocation;

    static internal GameObject[] _SkinPrefab
    { get { return _SkinPrefab; } }
    static internal int _SkinLocation
    { get { return skinLocation; } set { skinLocation = value; } }

    void Start()
    {
        Initialize();
    }

    void Initialize()     //  初期化
    {
        skinPrefab = new GameObject[5];
        ResourceLord();
        headTF = GameObject.Find("Head").GetComponent<Transform>();
        SkinGenerate(skinLocation);
    }
    void SkinGenerate(int skinLocation_)    //  スキンの生成
    {
        int tmpSkinNo = PlayerPrefs.GetInt("PlayerSkinNo", 0);
        if (tmpSkinNo != 0)
        {
            switch (skinLocation_)
            {
                case 0:
                    {
                        skinEntity = Instantiate(skinPrefab[PlayerPrefs.GetInt("PlayerSkinNo", 0)],headTF) ;
                        break;
                    }
            }
        }
        skinLocation = skinLocation_;
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
}
