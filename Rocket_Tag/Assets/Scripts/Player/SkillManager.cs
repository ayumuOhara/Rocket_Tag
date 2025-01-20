using Photon.Pun;
using TMPro;
using UnityEngine;
using System.Collections;

public class SkillManager : MonoBehaviourPunCallbacks
{
    ChangeObjColor changeObjColor;
    PlayerMovement playerMovement;

    float skillCT = 30.0f; // スキルのクールタイム(α版のみ。マスター版ではCSVファイルを使用)
    float skillTimer = 0; 
    [SerializeField] TextMeshProUGUI skillTimerText;
    [SerializeField] GameObject skillCTUI;
    float time = 0;
    bool finishSkill = true;

    private void Start()
    {
        if (GameObject.Find("SkillCTUI") && GameObject.Find("SkillTimerText"))
        {
            skillCTUI = GameObject.Find("SkillCTUI");
            skillTimerText = GameObject.Find("SkillTimerText").GetComponent<TextMeshProUGUI>();
        }
        
        changeObjColor = GetComponent<ChangeObjColor>();
        playerMovement = GetComponent<PlayerMovement>();

        if(skillCTUI != null) skillCTUI.SetActive(false);
    }

    private void Update()
    {
        if (skillTimer >= 0 && finishSkill)
        {
            if (skillTimer <= 0)
            {
                if (skillCTUI != null) skillCTUI.SetActive(false);
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
            Debug.Log($"スキルCT：{skillTimer}");
            if (skillTimer <= 0)
            {
                Debug.Log("スキルを使用した");
                StartCoroutine(DashSkill());
            }
            else
            {
                Debug.Log("スキルの使用条件を満たしていません");
            }
        }
    }

    float boostValue = 3.0f;
    float dashTime = 2.0f;

    // ダッシュスキル効果(修正版)
    IEnumerator DashSkill()
    {
        skillTimer = skillCT;
        finishSkill = false;
        skillCTUI.SetActive(true);
        skillTimerText.text = skillCT.ToString();
        float speed = playerMovement.GetMoveSpeed();

        // スキルを使用した状態
        playerMovement.SetMoveSpeed(speed * boostValue);
        photonView.RPC("ChangeColor", RpcTarget.All,
                        changeObjColor.colorMaterial[2].color.r,
                        changeObjColor.colorMaterial[2].color.g,
                        changeObjColor.colorMaterial[2].color.b,
                        changeObjColor.colorMaterial[2].color.a);

        yield return new WaitForSeconds(dashTime);

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
            skillTimer = Mathf.Clamp(skillTimer - 1, 0, skillCT);
            skillTimerText.text = skillTimer.ToString();
            time = 0;
        }
    }
}
