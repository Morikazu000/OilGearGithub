using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemypoolSingle : MonoBehaviour
{
    [SerializeField, Header("�����������G")]
    private GameObject _enemyvariety;

    [SerializeField, Header("��������G�̏��")]
    private int _enemycount = 5;

    [SerializeField, Header("�X�e�[�W�ɂ���G�̏��")]
    private int _inStageEnemy = 2;
    [SerializeField]
    private int _instagecount = 0;
    [SerializeField]
    private int _instantCount = 0;

    /// <summary>
    /// �G����
    /// </summary>
    public void InstantiateEnemy()
    {


        //�G�̐�����������ł͂Ȃ��ꍇ
        if (_instagecount < _inStageEnemy && _instantCount < _enemycount)
        {
            //�I�u�W�F�N�g�v�[���Ő�������
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                //�A�N�e�B�u��false�̂��̂���������ʒu���Đݒ肵�āAtrue�ɂ���B
                if (gameObject.transform.GetChild(i).gameObject.activeSelf == false)
                {
                    //�������ێ��ϐ��ƃX�e�[�W�ɂ���G�̕ϐ��J�E���g�A�b�v����setavtive��true�ɂ���
                    _instagecount++;
                    _instantCount++;
                    //�����|�C���g�Ƀ|�W�V�����ړ�
                    gameObject.transform.GetChild(i).gameObject.transform.position = gameObject.transform.position;
                    gameObject.transform.GetChild(i).gameObject.SetActive(true);
                    return;
                }

            }

            //�������ێ��ϐ��ƃX�e�[�W�ɂ���G�̕ϐ��J�E���g�A�b�v���ēG����
            _instagecount++;
            _instantCount++;
            Instantiate(_enemyvariety, transform.position, Quaternion.identity, this.gameObject.transform);
        }
        //�q�I�u�W�F�N�g��active = false�ɂȂ�����X�e�[�W�ɂ���G�̕ϐ������Z����
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.activeSelf == false && _instagecount > 0)
            {
                _instagecount--;
            }
        }
    }
}
