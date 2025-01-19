using Photon.Pun;
using TMPro;
using UnityEngine;
using System.Collections;

public class SkillManager : MonoBehaviourPunCallbacks
{
    ChangeObjColor changeObjColor;
    PlayerMovement playerMovement;

    private float skillCT; // �X�L���̃N�[���^�C��(���ł̂݁B�}�X�^�[�łł�CSV�t�@�C�����g�p)
    [SerializeField] TextMeshProUGUI skillTimerText;
    [SerializeField] GameObject skillCTUI;
    float time = 0;
    public bool finishSkill = true;

    private void Start()
    {
        skillCTUI = GameObject.Find("SkillCTUI");
        skillTimerText = GameObject.Find("SkillTimerText").GetComponent<TextMeshProUGUI>();
        changeObjColor = GetComponent<ChangeObjColor>();
        playerMovement = GetComponent<PlayerMovement>();

        skillCTUI.SetActive(false);
        
        // ���ňȊO�ł͍폜
        skillCT = 0;
    }

    private void Update()
    {
        if (skillCT >= 0 && finishSkill)
        {
            if (skillCT <= 0)
            {
                skillCTUI.SetActive(false);
            }
            else
            {
                // ���ł̂ݎg�p(�}�X�^�[�łł͍폜)
                SkillCool();
            }
        }
    }

    // �X�L���g�p
    public void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"�X�L��CT�F{skillCT}");
            if (skillCT <= 0f)
            {
                Debug.Log("�X�L���P���g�p����");
                StartCoroutine(DashSkill());
            }
            else
            {
                Debug.Log("�X�L���P�̎g�p�����𖞂����Ă��܂���");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (skillCT >= 30.0f)
            {
                Debug.Log("�X�L���Q���g�p����");
            }
            else
            {
                Debug.Log("�X�L���Q�̎g�p�����𖞂����Ă��܂���");
            }
        }
    }

    // �_�b�V���X�L������(�C����)
    IEnumerator DashSkill()
    {
        skillCT = 30.0f;
        finishSkill = false;
        skillCTUI.SetActive(true);
        skillTimerText.text = skillCT.ToString();
        float speed = playerMovement.GetMoveSpeed();

        // �X�L�����g�p�������
        playerMovement.SetMoveSpeed(3.0f);
        photonView.RPC("ChangeColor", RpcTarget.All,
                        changeObjColor.colorMaterial[2].color.r,
                        changeObjColor.colorMaterial[2].color.g,
                        changeObjColor.colorMaterial[2].color.b,
                        changeObjColor.colorMaterial[2].color.a);

        yield return new WaitForSeconds(2.0f);

        // �X�L�����g�p����O�̏��
        playerMovement.SetMoveSpeed(speed);
        photonView.RPC("ChangeColor", RpcTarget.All,
                        changeObjColor.colorMaterial[0].color.r,
                        changeObjColor.colorMaterial[0].color.g,
                        changeObjColor.colorMaterial[0].color.b,
                        changeObjColor.colorMaterial[0].color.a);

        finishSkill = true;

        yield break;
    }

    // �X�L���N�[���^�C��
    void SkillCool()
    {
        time += Time.deltaTime;
        if (time > 1)
        {
            skillCT = Mathf.Clamp(skillCT - 1, 0, 30.0f);
            skillTimerText.text = skillCT.ToString();
            time = 0;
        }
    }
}
