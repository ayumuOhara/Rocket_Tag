//using UnityEngine;

//public class ThrowRocket_Practice : MonoBehaviour    //  ���P�b�g�����R�[�h�ύX
//{
//    IState currentState;
//    void Start()
//    {
//        ChangeState(new HoldState());
//    }
//    void Update()
//    {
//        currentState.Update();
//    }
//    public void ChangeState(IState newState)    //  State��ύX����
//    {
//        if (currentState != null)
//        {
//            currentState.Exit();
//        }
//        currentState = newState;
//        currentState.Enter();
//    }
//}
//public interface IState    //  ���P�b�g�����C���^�[�t�F�C�X
//{
//    void Enter();
//    void Update();
//    void Exit();
//}
//public class HoldState : IState    //  �����Ă�����
//{
//    public void Enter()
//    {
        
//    }
//    public void Update()
//    {
//        if(Input.GetKeyDown(KeyCode.F))
//        {
            
//        }
//    }
//    public void Exit()
//    {
        
//    }
//    void GetScreenCenter()
//    {
//        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1000);
//        Vector3 worldCenter = playerCamera.ScreenToWorldPoint(screenCenter);
//        return playerCamera.ScreenToWorldPoint(screenCenter);
//    }
//}