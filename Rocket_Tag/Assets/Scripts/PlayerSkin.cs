using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Hook;
using static UnityEngine.InputManagerEntry;

public class PlayerSkin : MonoBehaviour    //  �v���C���[�X�L���X�N���v�g
{
    internal enum PlayerSkinProcess    //  �v���C���[�X�L�������ꗗ
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
//    void Initialize()     //  ���b�J�[�V�[���̏�����
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
//    void SetSkinNoByButton()    //  �{�^�������ɉ����āA�X�L���ԍ��ύX�֐����Ă�
//    {
//        skin0.onClick.AddListener(() => OnButtonClick(skin0));
//        skin1.onClick.AddListener(() => OnButtonClick(skin1));
//        skin2.onClick.AddListener(() => OnButtonClick(skin2));
//        skin3.onClick.AddListener(() => OnButtonClick(skin3));
//    }
//    void ChangeSkinNo(int clickSkinNo)    //  �X�L���ԍ��ύX
//    {
//        skinNo = clickSkinNo;
//        PlayerPrefs.SetInt("PlayerSkinNo",skinNo);
//        PlayerPrefs.Save();
//    }
//}