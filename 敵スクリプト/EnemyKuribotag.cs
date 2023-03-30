using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKuribotag : SmallEnemyManager
{
    private EnemyKuriboParameter kuriboParameter;
    private CharacterCommonParameter charaCommonParameter;


    private bool _isAttack = false;
    private bool _moveLeft = false;
    private bool _moveRight = false;

    private float _kuriboNowHp = default;
    private float _beforeHp = default;
    private float _moveSpeed = default;
    private float _timeCount = default;
    private float _invicibleTime = default;

    private Vector2 pos;

    private Animator _isanime;



    private void Start()
    {
        kuriboParameter = Resources.Load<EnemyKuriboParameter>(characterCommonParameter._myParameterObjName);
        charaCommonParameter = Resources.Load<CharacterCommonParameter>(characterCommonParameter._myParameterObjName);
        base.Start();
        _kuriboNowHp = GetNowHp();
        _beforeHp = GetNowHp();
        _invicibleTime = charaCommonParameter._damageInvincibleTime;

        pos = this.gameObject.transform.position;
        _isanime = gameObject.GetComponentInChildren<Animator>();
        _moveSpeed = kuriboParameter._movespeed;
        EnemyCollisionStart();
    }

    void FixedUpdate()
    {

        // ���݂�HP�擾
        _kuriboNowHp = GetNowHp();

        // �d�͏���
        UseGravity();

        //���̗̑͂Ƃ��̑O�̗̑͂������ł���Ȃ�ړ��ł���
        if (_kuriboNowHp == _beforeHp)
        {
            KuriboAttackmove();
        }
        else
        {
            StanTime();
        }

        PlayerBoxCollision();
        InvincibleTimeUpdate();
        RightWallRay();
        LeftWallRay();

        //HP���O�ȉ��ɂȂ����玀��
        if (_kuriboNowHp < 0)
        {
            gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// ���E�ړ�
    /// </summary>
    private void LeftRight()
    {
        // �n�ʂɐڐG���Ă���Ƃ��̂�
        if (_isGroundTouch == true)
        {

            //�A�j���[�V����
            _isanime.SetBool("Walking", true);
            _isanime.SetBool("Attack2", false);

            //�����̃|�W�V�������ړ��͈͈ȏ�ɂȂ��Ă�����
            if (this.gameObject.transform.position.x > pos.x + kuriboParameter._moverange)
            {

                _moveLeft  = true;
                _moveRight = false;
                //�G���ړ��͈͂𒴂�����A�߂�
                transform.position = new Vector2(pos.x + kuriboParameter._moverange, transform.position.y);
                TurnAngle();
            }

            else if (this.gameObject.transform.position.x < pos.x - kuriboParameter._moverange)
            {

                _moveRight = true;
                _moveLeft = false;

                //�G���ړ��͈͂𒴂�����A�߂�
                transform.position = new Vector2(pos.x - kuriboParameter._moverange, transform.position.y);
                TurnAngle();
            }

            //�ǂɐG��Ă�����A
            if (_isRightWallTouch == true || _isLeftWallTouch == true)
            {
                TurnAngle();

                _isLeftWallTouch = false;
                _isRightWallTouch = false;

            }

            //�G�̈ړ�
            this.gameObject.transform.Translate(Vector2.right * _moveSpeed * Time.deltaTime);

        }
    }
    /// <summary>
    /// �����Ă��������ς��郁�\�b�h
    /// </summary>
    private Quaternion TurnAngle()
    {

        //�i�s�����̕ύX
        _moveSpeed = -(_moveSpeed);
        if (_moveRight == true || _isLeftWallTouch == true)
        {
            //�G�̌�����]
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (_moveLeft == true  || _isRightWallTouch == true)
        {
            //�G�̌�����]
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        return this.transform.rotation;
    }

    /// <summary>
    /// �~�܂��čU������N���{�[
    /// </summary>
    private void KuriboAttackmove()
    {
        if (_isGroundTouch == true)
        {
            //�v���C���[�Ƃ̋������擾
            float _objdistance = Mathf.Abs(_playerObj.transform.position.x - transform.position.x);
            //�����������ꍇ
            if (_objdistance >= kuriboParameter._kuribo_distance)
            {
                //���E�ړ�
                LeftRight();
            }
            //�߂��ꍇ�U��
            else if (_isAttack == false)
            {

                //�A�j���[�V����
                _isanime.SetBool("Walking", false);
                _isanime.SetBool("Attack2", true);
                //�U������
                Invoke("Attackderay", kuriboParameter._attackderay);
                _isAttack = true;


            }
        }
    }

    /// <summary>
    /// �U���̒x��
    /// </summary>
    private bool Attackderay()
    {
        return _isAttack = false;
    }

    /// <summary>
    /// �X�^�����鎞��
    /// </summary>
    private void StanTime()
    {
        _isanime.SetBool("Walk_Anim", false);
        _timeCount += Time.fixedDeltaTime;
        if (_timeCount > _invicibleTime)
        {
            _timeCount = 0;
            _beforeHp = GetNowHp();
        }

    }

}
