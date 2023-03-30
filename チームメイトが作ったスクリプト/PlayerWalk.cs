using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : PlayerMotionManager
{
    //�ʏ펞1m������̈ړ��ɂ�����b��
    [SerializeField, Header("�ʏ펞�ړ����x(s/m)")]
    private float _defaultWalkTime;

    //��������1m������̈ړ��ɂ�����b��
    [SerializeField, Header("���������ړ����x(s/m)")]
    private float _insertOilWalkTime;

    //�󒆎�1m������̈ړ��ɂ�����b��
    [SerializeField, Header("�󒆎��ړ����x(s/m)")]
    private float _airWalkTime;

    //�E���͒l
    const float RIGHT_INPUT = 1;

    //�����͒l
    const float LEFT_INPUT = -1;

    //�ړ��Ȃ����͒l
    const float NO_INPUT = 0;



    /// <summary>
    /// �ړ����\�b�h
    /// </summary>
    /// <param name="isInsertOil">�������t���O</param>
    /// <param name="isAir">�󒆃t���O</param>
    /// <param name="inputValue">���͒l</param>
    public bool Walk(bool isInsertOil,�@bool isAir, float inputValue)
    {
        //�����t���O
        bool isWalk;

        //���͒l���ړ������ɕϊ�
        if (inputValue > 0)
        {
            //�E����
            inputValue = RIGHT_INPUT;
        }
        else if (inputValue < 0)
        {
            //������
            inputValue = LEFT_INPUT;
        }
        else
        {
            //���͂Ȃ�
            inputValue = NO_INPUT;
        }

        //�ړ������ɂ�蕪��
        switch (inputValue)
        {
            //�E
            case RIGHT_INPUT:
                //�E�ǂɓ������Ă��邩
                if (_characterManager.GetIsCollision
                        (CharacterManager.COLLISION.RIGHT_WALL))
                {
                    //�ړ����Ȃ��悤�ɂ���ד��͂�����
                    inputValue = NO_INPUT;
                }

                //�E������
                _playerControler.GetPlayerDirection(true);

                break;


            //��
            case LEFT_INPUT:
                //���ǂɓ������Ă��邩
                if (_characterManager.GetIsCollision
                        (CharacterManager.COLLISION.LEFT_WALL))
                {
                    //�ړ����Ȃ��悤�ɂ���ד��͂�����
                    inputValue = NO_INPUT;
                }

                //��������
                _playerControler.GetPlayerDirection(false);

                break;
        }

        //�ړ����邩
        if (inputValue != NO_INPUT)
        {
            //1�t���[���ɑ΂���ړ������擾�p
            float oneFrameWalkRange;

            //�ړ����x�擾�p
            float walkTime;


            //�󒆂ɂ��邩
            if (isAir)
            {
                //�󒆎��ړ����x�i�[
                walkTime = _airWalkTime;
            }
            //����������Ă��邩
            else if (isInsertOil)
            {
                //���������ړ����x�i�[
                walkTime = _insertOilWalkTime;
            }
            else
            {
                //�ʏ펞�ړ����x�i�[
                walkTime = _defaultWalkTime;
            }

            //1�t���[���ɑ΂���ړ������擾
            oneFrameWalkRange = GetFixedDeltaTime() / walkTime;

            //�ړ�
            transform.position += Vector3.right * 
                                    inputValue * oneFrameWalkRange;

            //�ڒn���Ă��炸�A���󒆂ɂ��Ȃ���
            if (!_characterManager.GetIsCollision
                        (CharacterManager.COLLISION.GROUND) && !isAir)
            {
                //�����J�n
                _playerControler.FallStart();
            }

            //�ړ��t���Otrue
            isWalk = true;
        }
        else
        {
            //�ړ��t���Ofalse
            isWalk = false;
        }

        return isWalk;
    }



    /// <summary>
    /// �ړ����x�{���擾���\�b�h
    /// </summary>
    /// <returns>�ړ����x�{��</returns>
    public float GetWalkSpeedMagnification()
    {
        //�ړ����x�{���擾�p
        float walkSpeedMagnification;


        //�ړ����x�{���擾
        walkSpeedMagnification = _defaultWalkTime / _insertOilWalkTime;

        return walkSpeedMagnification;
    }
}
