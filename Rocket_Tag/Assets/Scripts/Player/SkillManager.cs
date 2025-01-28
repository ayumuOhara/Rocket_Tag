using Photon.Pun;
using System.Collections;
using UnityEngine;

public class SkillManager : MonoBehaviourPunCallbacks
{
    [SerializeField] SkillDataBase skillDataBase;
    SkillData skillData;
    int skillIdx;

    ChangeObjColor changeObjColor;
    PlayerMovement playerMovement;

    public bool finishSkill = true;

    void SetSkill()
    {
        skillData = skillDataBase.skillDatas[skillIdx];
    }

    private void Start()
    {
        changeObjColor = GetComponent<ChangeObjColor>();
        playerMovement = GetComponent<PlayerMovement>();
        skillIdx = 0;
        SetSkill();
    }

    public void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (skillData.countLimit > 0)
            {
                StartCoroutine(DashSkill());
            }
        }
    }

    float boostValue = 2f;
    float dashTime = 3.0f;

    IEnumerator DashSkill()
    {
        finishSkill = false;
        skillData.countLimit--;

        float speed = playerMovement.GetMoveSpeed();
        playerMovement.SetMoveSpeed(speed * boostValue);
        photonView.RPC("ChangeColor", RpcTarget.All,
            changeObjColor.colorMaterial[2].color.r,
            changeObjColor.colorMaterial[2].color.g,
            changeObjColor.colorMaterial[2].color.b,
            changeObjColor.colorMaterial[2].color.a);

        yield return new WaitForSeconds(dashTime);

        playerMovement.SetMoveSpeed(speed);
        photonView.RPC("ChangeColor", RpcTarget.All,
            changeObjColor.colorMaterial[0].color.r,
            changeObjColor.colorMaterial[0].color.g,
            changeObjColor.colorMaterial[0].color.b,
            changeObjColor.colorMaterial[0].color.a);

        finishSkill = true;
    }
}
