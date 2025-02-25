using UnityEngine;
using UnityEngine.SceneManagement;

public class SkinGanarater : MonoBehaviour
{
    enum PlayerSkinNo    //  �v���C���[�X�L�������ꗗ
    {
        NONE,
        RED_CAP,
        STRAW_HAT,
        ERINGI,
        FREEZA,
    }

    enum SkinLocation    //  �X�L���̏ꏊ
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

    void Initialize()     //  ������
    {
        skinPrefab = new GameObject[5];
        ResourceLord();
        headTF = GameObject.Find("Head").GetComponent<Transform>();
        SkinGenerate(skinLocation);
    }
    void SkinGenerate(int skinLocation_)    //  �X�L���̐���
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
    void ResourceLord()    //  Resource�t�H���_���̃t�@�C����ǂݍ���
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
