using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Hook;
using static UnityEngine.InputManagerEntry;

public class PlayerSkin : MonoBehaviour    //  �v���C���[�X�L���X�N���v�g
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
    static GameObject skinEntity;
    Transform headTF;
    Button redCap;
    Button strawHat;
    Button eringi;
    Button freeza;

    int skinNo;
    static int skinLocation;

    static internal GameObject[] _SkinPrefab
    { get { return _SkinPrefab; } }
    static internal int _SkinLocation
    { get { return skinLocation; } }

    void Start()
    {
        Initialize();
    }

    void Initialize()     //  ������
    {
        SceneManager.sceneUnloaded += SaveSkinNo;
        skinPrefab = new GameObject[5];
        ResourceLord();
        headTF     = GameObject.Find("Head"            ).GetComponent<Transform>();
        redCap     = GameObject.Find("RedCap"          ).GetComponent<Button>();
        strawHat   = GameObject.Find("StrawHat"        ).GetComponent<Button>();
        eringi     = GameObject.Find("Eringi"          ).GetComponent<Button>();
        freeza     = GameObject.Find("Freeza"          ).GetComponent<Button>();
        SetSkinNoByButton();    //  �{�^�������ǉ�

        skinNo = PlayerPrefs.GetInt("PlayerSkinNo", 0);
        SkinGenerate(skinLocation);
    }
    void SetSkinNoByButton()    //  �{�^�������ɉ����āA�X�L���ԍ��ύX�֐����Ă�
    {
        redCap.onClick.AddListener(() => ChangeSkin((int)PlayerSkinNo.RED_CAP, (int)SkinLocation.HEAD));
        strawHat.onClick.AddListener(() => ChangeSkin((int)PlayerSkinNo.STRAW_HAT, (int)SkinLocation.HEAD));
        eringi.onClick.AddListener(() => ChangeSkin((int)PlayerSkinNo.ERINGI, (int)SkinLocation.HEAD));
        freeza.onClick.AddListener(() => ChangeSkin((int)PlayerSkinNo.FREEZA, (int)SkinLocation.HEAD));
        Debug.Log(3);
    }
    void ChangeSkin(int clickSkinNo, int skinLocation)    //  �X�L���ԍ��ύX
    {
        if (skinNo != clickSkinNo)
        {
            skinNo = clickSkinNo;
            SkinGenerate(skinLocation);
        }
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
    void SkinGenerate(int skinLocation_)    //  �X�L���̐���
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
    void SaveSkinNo(Scene scene)    //  �V�[���A�����[�h���ɃX�L���Z�[�u
    {
        PlayerPrefs.SetInt("PlayerSkinNo", skinNo);
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.GetInt("PlayerSkinNo"));
        SceneManager.sceneUnloaded -= SaveSkinNo;
    }
}

////////////////////////////////////////////  �X�L���������@  //////////////////////////////////

/* ���L�̊֐����`����
 * void SkinGenerate(int skinLocation_)    //  �X�L���̐���
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
                        skinEntity = Instantiate(skinPrefab[skinNo], head);
                        break;
                    }
            }
        }
        skinLocation = skinLocation_;
    }
 * �����Ő���    SkinGenerate(PlayerSkin._SkinPrefab[PlayerSkin._SkinLocation]);
   �����@�@Head��Transform�Ȃ��Ɛ�������܂���  ����
 */