using Photon.Pun;
using TMPro;
using UnityEngine;
using System.Collections;

public class SkillManager : MonoBehaviourPunCallbacks
{
    ChangeObjColor changeObjColor;
    PlayerMovement playerMovement;

    private float skillCT; // スキルのクールタイム(α版のみ。マスター版ではCSVファイルを使用)
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
        
        // α版以外では削除
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
                // α版のみ使用(マスター版では削除)
                SkillCool();
            }
        }
    }

    // スキル使用
    public void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"スキルCT：{skillCT}");
            if (skillCT <= 0f)
            {
                Debug.Log("スキル１を使用した");
                StartCoroutine(DashSkill());
            }
            else
            {
                Debug.Log("スキル１の使用条件を満たしていません");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (skillCT >= 30.0f)
            {
                Debug.Log("スキル２を使用した");
            }
            else
            {
                Debug.Log("スキル２の使用条件を満たしていません");
            }
        }
    }

    // ダッシュスキル効果(修正版)
    IEnumerator DashSkill()
    {
        skillCT = 30.0f;
        finishSkill = false;
        skillCTUI.SetActive(true);
        skillTimerText.text = skillCT.ToString();
        float speed = playerMovement.GetMoveSpeed();

        // スキルを使用した状態
        playerMovement.SetMoveSpeed(3.0f);
        photonView.RPC("ChangeColor", RpcTarget.All,
                        changeObjColor.colorMaterial[2].color.r,
                        changeObjColor.colorMaterial[2].color.g,
                        changeObjColor.colorMaterial[2].color.b,
                        changeObjColor.colorMaterial[2].color.a);

        yield return new WaitForSeconds(2.0f);

        // スキルを使用する前の状態
        playerMovement.SetMoveSpeed(speed);
        photonView.RPC("ChangeColor", RpcTarget.All,
                        changeObjColor.colorMaterial[0].color.r,
                        changeObjColor.colorMaterial[0].color.g,
                        changeObjColor.colorMaterial[0].color.b,
                        changeObjColor.colorMaterial[0].color.a);

        finishSkill = true;

        yield break;
    }

    // スキルクールタイム
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
