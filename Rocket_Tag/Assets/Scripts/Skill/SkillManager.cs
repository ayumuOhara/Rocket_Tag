using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SkillManager : MonoBehaviourPunCallbacks
{
    [SerializeField] SkillDataBase skillDataBase;
    [SerializeField] public SkillData skillData;
    public int skillIdx;

    [SerializeField] ObserveDistance observeDistance;

    PlayerMovement playerMovement;
    TimeManager timeManager;
    GameManager gameManager;

    [SerializeField] GameObject player;
    [SerializeField] GameObject rocketObj;
    [SerializeField] GameObject stickyZone;

    public bool finishSkill = true;

    // �X�L����ݒ�
    public void SetSkill(SkillData newSkillData)
    {
        skillData = newSkillData;
    }

    // �����X�L�����폜
    public void RemoveSkill()
    {
        skillData = null;
    }

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        skillIdx = 0;
        SetSkill(skillDataBase.SkillData[skillIdx]);
    }

    // �ݒ肳��Ă���X�L���g�p
    public void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (finishSkill == true)
            {
                Debug.Log($"�y{skillData.skillName}�z���g�p");

                switch (skillData.SkillId)
                {
                    case 101: break;
                    case 102: photonView.RPC("PutStickyZone", RpcTarget.All); break;
                    case 103: DangerousGift();                                break;
                    case 104: SmashPunch();                                   break;
                    case 105: StartCoroutine(Dash());                         break;
                }

                SendSkillData();
            }
        }
    }

    // �˂΂˂΃]�[���ݒu
    [PunRPC]
    void PutStickyZone()
    {
        GameObject zone = Instantiate(stickyZone);
        Vector3 playerPos = player.transform.position;
        playerPos += Vector3.down;
        zone.transform.position = playerPos;
    }

    // �v���C���[�Ƀ��P�b�g��z�z
    void DangerousGift()
    {
        int playerCnt = gameManager.GetPlayerList().Count;

        int minCnt = 1;
        int maxCnt = playerCnt > 3 ? 3 : playerCnt;
        int rocketCnt = Random.Range(minCnt, maxCnt);

        for(int i = 0; i < rocketCnt; i++)
        {
            gameManager.ChooseRocketPlayer();
        }
    }

    // �X�}�b�V���p���`
    void SmashPunch()
    {
        GameObject target = observeDistance.GetTargetDistance();

        if (target == null) return; // �^�[�Q�b�g�����Ȃ��ꍇ�͏������Ȃ�

        // �v���C���[���^�[�Q�b�g�̕����֌�����
        transform.LookAt(target.transform.position);
        KnockBackTarget(target);
    }

    // �Ώۂ𐁂���΂�
    public void KnockBackTarget(GameObject target)
    {
        PhotonView targetView = target.GetComponent<PhotonView>();
        targetView.RPC("SetIsStun", RpcTarget.All, true);

        // �^�[�Q�b�g�� Rigidbody ���擾
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            // ������΂��������v�Z�i�v���C���[ �� �^�[�Q�b�g �̕����j
            Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;

            // ������΂��́i�K�X�����j
            float knockbackForce = 30f;

            // �^�[�Q�b�g�ɗ͂�������
            targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }        
    }
    
    // �_�b�V���X�L��
    IEnumerator Dash()
    {
        float boostValue = 1.5f;     // �_�b�V���̉����x
        float dashLimit = 3.0f;      // �_�b�V���̌��ʎ���

        finishSkill = false;

        float speed = playerMovement.GetMoveSpeed();
        playerMovement.SetMoveSpeed(speed * boostValue);

        yield return new WaitForSeconds(dashLimit);

        playerMovement.SetMoveSpeed(speed);

        finishSkill = true;

        yield break;
    }

    // �X�L�����v���C���[�ɗ^����
    void SendSkillData()
    {
        int rnd = Random.Range(0, skillDataBase.SkillData.Count);
        SkillData giveSkill = skillDataBase.SkillData[rnd];
        SetSkill(giveSkill);
    }
}