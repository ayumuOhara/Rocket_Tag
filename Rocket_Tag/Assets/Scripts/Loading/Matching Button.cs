using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Text statusText;
    public Button button; // �{�^���̎Q��
    private RectTransform rectTransform; // �{�^���� RectTransform
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ���݂̈ʒu���擾
        Vector3 currentPosition = rectTransform.position;

        // Y����-1000�̈ʒu��ǉ�
        Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y - 1000, currentPosition.z);

        // �{�^���̈ʒu��ύX
        rectTransform.position = newPosition;
    }

    // Update is called once per frame
    void Update()
    {
        statusText.text = "�}�b�`���O�X�^�[�g";
    }
}
