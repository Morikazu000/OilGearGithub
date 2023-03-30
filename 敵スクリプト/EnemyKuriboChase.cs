using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyKuriboChase : SmallEnemyManager
{
    private EnemyKuriboChaseParameter kuribochaseParameter;
    private CharacterCommonParameter charaCommonParameter;

    [field: SerializeField, Header("�ړ��X�s�[�h")]
    private float _moveSpeed = 5;

    private Vector2 _myPos;
    private Vector2 _playerPos;
    private Vector2 _myDirection;
    private Vector2 _targetDirection;

    private float _defaultPos;
    private float _targetDistance;
    private float _cosHalf;
    private float _innerProduct;
    private float _invicibleTime = default;
    private float _timeCount = default;
    private float _kuriboChaseNowHp = default;
    private float _beforeHp = default;
    private float _spawnPosX = default;
    private bool _isAttack = false;
    private bool _moveLeft = false;
    private bool _moveRight = false;

    private Animator _isAnime;

   
    private void Start()
    {
        kuribochaseParameter = Resources.Load<EnemyKuriboChaseParameter>(characterCommonParameter._myParameterObjName);
        charaCommonParameter = Resources.Load<CharacterCommonParameter>(characterCommonParameter._myParameterObjName);

        base.Start();
        _kuriboChaseNowHp = GetNowHp();
        _beforeHp = GetNowHp();

        _defaultPos = gameObject.transform.position.x;
        _spawnPosX = _defaultPos;
        _isAnime = gameObject.GetComponentInChildren<Animator>();
        EnemyCollisionStart();
    }


    private void FixedUpdate()
    {

        //�����|�W�V����
        _myPos = gameObject.transform.position;

        //�����̌���
        _myDirection = gameObject.transform.right;

        //�v���C���[�̃|�W�V����
        _playerPos = _playerObj.transform.position;

        //�v���C���[�ւ̌���
        _targetDirection = _playerPos - _myPos;

        //�v���C���[�Ƃ̋���
        _targetDistance = _targetDirection.magnitude;

        _cosHalf = Mathf.Cos(kuribochaseParameter._myangle * Mathf.Deg2Rad);


        //����͈͒��̖ʐς̐ݒ�
        _innerProduct = Vector2.Dot(_myDirection, _targetDirection.normalized);

        //�v���C���[�Ƃ̋������擾
        float _objdistance = Mathf.Abs(_playerObj.transform.position.x - transform.position.x);

        //�����������ꍇ
        if (_objdistance >= kuribochaseParameter._kuribodistance)
        {
            //�A�j���[�V����
            _isAnime.SetBool("Walking", true);
            _isAnime.SetBool("Attack2", false);


            //�E�������Ă���Ƃ��Ɏ��E�ɓ������ꍇ
            if (_innerProduct > _cosHalf && _targetDistance < kuribochaseParameter._maxrange
            && Mathf.Sign(_moveSpeed) == 1)
            {
                _moveSpeed = Mathf.Abs(_moveSpeed);
                transform.position = Vector3.MoveTowards(transform.position, _playerObj.transform.position, _moveSpeed * Time.deltaTime);
                _defaultPos = gameObject.transform.position.x;

            }

            //���������Ă���Ƃ��Ɏ��E�ɓ������ꍇ
            else if (_innerProduct < -_cosHalf && _targetDistance < kuribochaseParameter._maxrange
                    && Mathf.Sign(_moveSpeed) == -1)
            {
                transform.position = Vector3.MoveTowards(transform.position, _playerObj.transform.position, Mathf.Abs(_moveSpeed) * Time.deltaTime);
                _defaultPos = gameObject.transform.position.x;

            }

            if (_kuriboChaseNowHp == _beforeHp)
            {
                //���E�ړ�
                LeftRight();
            }
            else
            {
                StanTime();
            }



            UseGravity();
        }

        //���E�ɓ����Ă��āAisattack��false�̏ꍇ
        else
        {
            Attack();
        }

        RightWallRay();
        LeftWallRay();
        InvincibleTimeUpdate();

        if (_isDead == true)
        {
            _defaultPos = _spawnPosX;
            gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// ���E�ړ�
    /// </summary>
    private void LeftRight()
    {
        //�A�j���[�V����
        _isAnime.SetBool("Walking", true);
        _isAnime.SetBool("Attack2", false);


        // �n�ʂɐڐG���Ă���Ƃ��̂�
        if (_isGroundTouch == true)
        {


            //�����̃|�W�V�������ړ��͈͈ȏ�ɂȂ��Ă�����
            if (this.gameObject.transform.position.x > _defaultPos + kuribochaseParameter._moverange)
            {
                print("�E������");
                _moveLeft = true;
                _moveRight = false;

                //�G���ړ��͈͂𒴂�����A�߂�
                transform.position = new Vector2(_defaultPos + kuribochaseParameter._moverange, transform.position.y);
                TurnAngle();
            }

            else if (this.gameObject.transform.position.x < _defaultPos - kuribochaseParameter._moverange)
            {
                print("��������");
                _moveRight = true;
                _moveLeft = false;

                //�G���ړ��͈͂𒴂�����A�߂�
                transform.position = new Vector2(_defaultPos - kuribochaseParameter._moverange, transform.position.y);
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
    /// <returns></returns>
    private Quaternion TurnAngle()
    {

        //�i�s�����̕ύX
        _moveSpeed = -(_moveSpeed);
        if (_moveRight == true || _isLeftWallTouch == true)
        {
            //�G�̌�����]
            transform.rotation = Quaternion.Euler(0f, -180f, 0f);
        }
        else if (_moveLeft == true || _isRightWallTouch == true)
        {
            //�G�̌�����]
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        return this.transform.rotation;
    }

    /// <summary>
    /// �U������
    /// </summary>
    private void Attack()
    {
        //�߂��ꍇ�U��
        if (_isAttack == false)
        {
            //�A�j���[�V����
            _isAnime.SetBool("Walking", false);
            _isAnime.SetBool("Attack2", true);
            _isAttack = true;
            PlayerBoxCollision();

            //�U������
            Invoke("Attackderay", kuribochaseParameter._attacktime);
        }

    }
    private void Attackderay()
    {
        _isAttack = false;
    }
    private void StanTime()
    {
        _isAnime.SetBool("Walk_Anim", false);
        _timeCount += Time.fixedDeltaTime;
        if (_timeCount > _invicibleTime)
        {
            _timeCount = 0;
            _beforeHp = GetNowHp();
        }

    }

}