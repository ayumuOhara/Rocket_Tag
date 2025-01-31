using UnityEngine;

public class SkillGetter : MonoBehaviour
{
    [SerializeField] GameObject skillObject;
    [SerializeField] SkillDataBase skillDataBase;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            SendSkillData(other.gameObject);
            DestroyObj();
        }
    }

    // スキルをプレイヤーに与える
    void SendSkillData(GameObject player)
    {
        SkillManager skillManager = player.gameObject.GetComponent<SkillManager>();
        int rnd = Random.Range(0, skillDataBase.skillDatas.Length);

        SkillData giveSkill = skillDataBase.skillDatas[rnd];
        skillManager.SetSkill(giveSkill);
    }

    // スキルアイテムを消去
    void DestroyObj()
    {
        Destroy(this.gameObject);
    }
}
