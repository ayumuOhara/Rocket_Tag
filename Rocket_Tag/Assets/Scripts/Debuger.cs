using UnityEngine;

public class Debuger : MonoBehaviour
{
    GameObject PlayerObj;
    [SerializeField] GameObject skillObject;      // スキルアイテム
    [SerializeField] Transform skillSpawner;      // スキルアイテム生成場所

    PlayerMovement playerMovement;                // プレイヤーの移動処理クラス
    PlayerRocketAction playerRocketAction;        // プレイヤーのロケットアクションクラス
    SetPlayerBool setPlayerBool;                  // bool値を管理するクラス
    SkillManager skillManager;                    // スキルを管理するクラス
    ObserveDistance observeDistance;              // 対象との距離を測るクラス
    ChangeObjColor changeObjColor;                // オブジェクトの色変更をするクラス

    [SerializeField] SkillDataBase skillDataBase; // スキルのデータ
    [SerializeField] int Debug_skillIdx;          // スキル番号

    TimeManager timeManager;
    GameManager gameManager;

    // InstantiatePlayersから呼び出す
    public void SetComponents(GameObject gameObject)
    {
        // プレイヤーについているコンポーネント
        PlayerObj          = gameObject;
        playerMovement     = gameObject.GetComponent<PlayerMovement>();
        playerRocketAction = gameObject.GetComponent<PlayerRocketAction>();
        setPlayerBool      = gameObject.GetComponent<SetPlayerBool>();
        skillManager       = gameObject.GetComponent<SkillManager>();
        observeDistance    = gameObject.GetComponent<ObserveDistance>();
        changeObjColor     = gameObject.GetComponent<ChangeObjColor>();

        // プレイヤー以外のコンポーネント
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
