using UnityEngine;

public class Debuger : MonoBehaviour
{
    GameObject PlayerObj;
    [SerializeField] GameObject skillObject;      // �X�L���A�C�e��
    [SerializeField] Transform skillSpawner;      // �X�L���A�C�e�������ꏊ

    PlayerMovement playerMovement;                // �v���C���[�̈ړ������N���X
    PlayerRocketAction playerRocketAction;        // �v���C���[�̃��P�b�g�A�N�V�����N���X
    SetPlayerBool setPlayerBool;                  // bool�l���Ǘ�����N���X
    SkillManager skillManager;                    // �X�L�����Ǘ�����N���X
    ObserveDistance observeDistance;              // �ΏۂƂ̋����𑪂�N���X
    ChangeObjColor changeObjColor;                // �I�u�W�F�N�g�̐F�ύX������N���X

    [SerializeField] SkillDataBase skillDataBase; // �X�L���̃f�[�^
    [SerializeField] int Debug_skillIdx;          // �X�L���ԍ�

    TimeManager timeManager;
    GameManager gameManager;

    // InstantiatePlayers����Ăяo��
    public void SetComponents(GameObject gameObject)
    {
        // �v���C���[�ɂ��Ă���R���|�[�l���g
        PlayerObj          = gameObject;
        playerMovement     = gameObject.GetComponent<PlayerMovement>();
        playerRocketAction = gameObject.GetComponent<PlayerRocketAction>();
        setPlayerBool      = gameObject.GetComponent<SetPlayerBool>();
        skillManager       = gameObject.GetComponent<SkillManager>();
        observeDistance    = gameObject.GetComponent<ObserveDistance>();
        changeObjColor     = gameObject.GetComponent<ChangeObjColor>();

        // �v���C���[�ȊO�̃R���|�[�l���g
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug_skillIdx = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchSkill();
            }
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                SpawnSkillObj();
            }
        }        
    }

    void SwitchSkill()
    {
        Debug_skillIdx = Debug_skillIdx++ >= 4 ? 0 : Debug_skillIdx++;
        skillManager.SetSkill(skillDataBase.skillDatas[Debug_skillIdx]);
    }

    void SpawnSkillObj()
    {
        GameObject item = Instantiate(skillObject);
        item.transform.position = skillSpawner.transform.position;
    }
}
