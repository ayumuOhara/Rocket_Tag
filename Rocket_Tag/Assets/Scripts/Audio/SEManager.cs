using UnityEngine;
using System.Collections.Generic;

public class SEManager : MonoBehaviour
{
    [SerializeField] private AudioSource seAudioSource;
    [SerializeField] private List<AudioClip> seClips;

    // Enum�ɂ��SE�Ǘ�
    public enum SEType
    {
        Button_Click,  // �{�^�����N���b�N�����Ƃ��̉�
        Dash,          // ���������̉�
        Rocket_Set,    // ���P�b�g�������t�����Ƃ��̉�
        Skill_Use,     // �X�L���g�p���̉�
        Bumper,        // �W�����v��̉�
        Landing,       // ���n�����Ƃ��̉�
        Smash_Punch,   // �X�}�b�V���p���`�̉�
        Collision_Dash_1,  // �Ԃ���_�b�V���g�p���̉�
        Collision_Dash_2,  // �Ԃ���_�b�V���Փˎ��̉�
        Sticky_Zone,   // �˂΂˂΃]�[���W�J���̉�
        Pull_Hook_1,   // �����񂹃t�b�N�������̉�
        Pull_Hook_2    // �����񂹃t�b�N�����񂹂鎞�̉�
    }

    // SE�Đ����\�b�h
    public void PlaySE(SEType seType)
    {
        int index = (int)seType;  // Enum����C���f�b�N�X�֕ϊ�
        PlaySEFromList(index);
    }

    // ���X�g����SE���Đ�
    private void PlaySEFromList(int index)
    {
        if (index >= 0 && index < seClips.Count)
        {
            seAudioSource.PlayOneShot(seClips[index]);
        }
        else
        {
            Debug.LogWarning("�w�肳�ꂽ�C���f�b�N�X�ɊY������SE������܂���");
        }
    }
}
