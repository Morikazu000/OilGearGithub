using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpNoma : PlayerMotionManager
{
    //�W�����v�J�n����ō��_�܂œ��B�ɂ����鎞��
    [SerializeField, Header("�W�����v�ō����x���B�܂ł̎���(s)")]
    private float _jumpMaxTime;

    //�W�����v�̍ō����x
    [SerializeField, Header("�W�����v�ō����x(m)")]
    private float _jumpMaxHeight;

    //�W�����v�J�n����Œ�_�܂œ��B�ɂ����鎞��
    [SerializeField, Header("�W�����v�Œፂ�x���B�܂ł̎���(s)")]
    private float _jumpMinTime;

    //�W�����v�̍Œፂ�x
    [SerializeField, Header("�W�����v�Œፂ�x(m)")]
    private float _jumpMinHeight;

    //���݂̃W�����v���x�i�[�p
    private float _jumpNowHeight = 0;

    //���݂̃W�����v���Ԋi�[�p
    private float _jumpNowTime = 0;

    //�W�����v�I���t���O
    private bool _isJumpEnd = false;

    //�W�����v�J�n�t���O
    private bool _isJumpStart = false;

    //�W�����v���t���O
    private bool _isJumpNow = false;



    private void Update()
    {
        //�W�����v����
        if (_isJumpNow)
        {
            //�V��ɓ���������
            if (_characterManager.GetIsCollision(CharacterManager.COLLISION.ROOF))
            {
                _isJumpEnd = true;
            }
        }
    }



    /// <summary>
    /// �W�����v���W�����v�̍��x��Ԃ����\�b�h
    /// </summary>
    /// <param name="isInputJump">�W�����v���̓t���O</param>
    public float Jump(bool isInputJump)
    {
        /*-------------------*/
        //�񎟊֐�������
        //y=a(x-p)^2+q
        /*-------------------*/

        //�v�Z�p
        float a, x, y;

        //�ŐV�̃W�����v���x�擾�p
        float jumpUpHeight;

        //�W�����v���Ԏ擾�p
        float jumpTime;

        //�W�����v���x�擾�p
        float jumpHeight;


        //�W�����v�J�n���Ă��Ȃ���
        if (!_isJumpStart)
        {
            //���݂̃W�����v���x�A���ԏ�����
            _jumpNowHeight = 0;
            _jumpNowTime = 0;

            //�W�����v�I���t���Ofalse
            _isJumpEnd = false;

            //�W�����v�J�n�t���Otrue
            _isJumpStart = true;

            //�W�����v���t���O
            _isJumpNow = true;
        }

        //�W�����v�I�����邩
        if (_isJumpEnd)
        {
            //�W�����v���x�A���Ԏ擾
            jumpTime = _jumpMinTime;
            jumpHeight = _jumpMinHeight;
        }
        else
        {
            //�W�����v���͂���Ă��邩
            if (isInputJump)
            {
                //�W�����v���x�A���Ԏ擾
                jumpTime = _jumpMaxTime;
                jumpHeight = _jumpMaxHeight;
            }
            else
            {
                //�W�����v�I���t���Otrue
                _isJumpEnd = true;

                //���݂̃W�����v���x�A���ԏ�����
                _jumpNowHeight = 0;
                _jumpNowTime = 0;

                //�W�����v���x�A���Ԏ擾
                jumpTime = _jumpMinTime;
                jumpHeight = _jumpMinHeight;
            }
        }

        //�񎟊֐��v�Z
        x = Mathf.Pow(jumpTime * 2, 2) / 2;
        y = jumpHeight * -2;
        a = y / x;

        //���݂̃W�����v���ԍX�V
        _jumpNowTime += GetFixedDeltaTime();

        //�ŐV�̃W�����v���x�擾
        jumpUpHeight = a * Mathf.Pow(_jumpNowTime - jumpTime, 2) 
                            + jumpHeight;

        //�������n�߂���
        if(jumpUpHeight - _jumpNowHeight < 0)
        {
            /*�����J�n*/

            //�W�����v�J�n�t���Ofalse
            _isJumpStart = false;

            //�W�����v���t���Ofalse
            _isJumpNow = false;

            //�W�����v���x�擾
            jumpHeight = 0;

            //�����J�n
            _playerControler.FallStart();
        }
        else
        {
            /*�W�����v*/

            //�W�����v���x�擾
            jumpHeight = jumpUpHeight - _jumpNowHeight;

            //�W�����v������
            transform.position += Vector3.up * jumpHeight;

            //���݂̃W�����v���x�X�V
            _jumpNowHeight = jumpUpHeight;
        }

        return jumpHeight;
    }



    /// <summary>
    /// �W�����v�̍ō����x��Ԃ����\�b�h
    /// </summary>
    /// <returns>�W�����v�̍ō����x</returns>
    public float ReturnJumpMaxHeight()
    {
        return _jumpMaxHeight;
    }
}
