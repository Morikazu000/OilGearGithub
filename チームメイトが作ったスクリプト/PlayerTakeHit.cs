using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeHit : PlayerMotionManager
{
    //�Ђ�ގ���(s)
    [SerializeField, Header("�Ђ�ݎ���(s)")]
    private float _stanTime;

    //�m�b�N�o�b�N���鋗��
    [SerializeField, Header("�m�b�N�o�b�N����(m)")]
    private float _knockBackRange;

    //���݂̂Ђ�ݎ���
    private float _stanNowTime = 0;

    //�m�b�N�o�b�N�����t���O
    private bool _isKnockBackRight;

    //�m�b�N�o�b�N�J�n�t���O
    private bool _isKnockBackStart = true;



    public void TakeHit(bool isKnockBackRight)
    {
        //�m�b�N�o�b�N�J�n����
        if (_isKnockBackStart)
        {
            //�m�b�N�o�b�N�����t���O�擾
            _isKnockBackRight = isKnockBackRight;

            //���݂̃X�^�����ԏ�����
            _stanNowTime = 0;

            _isKnockBackStart = false;
        }

        //���݂̂Ђ�ݎ��ԍX�V
        _stanNowTime += GetFixedDeltaTime();

        //�Ђ�ݎ��Ԓ���
        if (_stanNowTime < _stanTime)
        {
            //�m�b�N�o�b�N�����擾�p
            float knockBackRange;

            //�m�b�N�o�b�N�����擾�p
            Vector3 knockBackDirection;


            //�m�b�N�o�b�N�����擾
            knockBackRange = _knockBackRange / (_stanTime / GetFixedDeltaTime());

            //�m�b�N�o�b�N����������
            if (_isKnockBackRight)
            {
                //�m�b�N�o�b�N���E��
                knockBackDirection = Vector3.right;
            }
            else
            {
                //�m�b�N�o�b�N������
                knockBackDirection = Vector3.left;
            }

            //�m�b�N�o�b�N������
            transform.position += knockBackDirection * knockBackRange;
        }
        else
        {
            _isKnockBackStart = true;

            _playerControler.DefaultMotion();
        }
    }
}
