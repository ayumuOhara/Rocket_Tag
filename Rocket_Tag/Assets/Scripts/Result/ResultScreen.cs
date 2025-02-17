using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class ResultScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rankText;

    //void Update()
    //{
    //    ShowMyResult();
    //}

    public void ShowMyResult()
    {
        int myRank = 1;

        //�����̏��ʂ��J�X�^���v���p�e�B����擾
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerRank"))
        {
            myRank = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerRank"];
        }

        rankText.text = myRank.ToString() + "��!";
        this.gameObject.SetActive(true);
    }

    // ���r�[��ʂւ̑J��
    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }
}
