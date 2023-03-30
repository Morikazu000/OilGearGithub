using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemychoropu : SmallEnemyManager
{
    private EnemyChoropuParameter choropuParameter;
    private CharacterCommonParameter charaCommonParameter;
    [field: SerializeField, Header("���C�̋���")]
    public float _raylength = 5;

    [SerializeField, Header("���C�̌���")]
    private Objrayangle _angletype = Objrayangle.Right;

    [SerializeField, Header("��������")]
    private float _distanceRange = 1;

    private float _mainJumpPower;
    private float _ChoropuNowHp = default;
    private float _BeforeHp = default;
    private float _invicibleTime = default;
    private float _timeCount = default;
    private float deraycount = default;
    private float objectdistance = default;

    private int _playerlayermask;

    private bool _isjump = false;
    private bool _ischase = false;
    private bool _isanime  = false;
    private Animator _animationControll;


    private enum Objrayangle
    {
        Left,
        Right,
    }

    private void Start()
    {
        choropuParameter = Resources.Load<EnemyChoropuParameter>(characterCommonParameter._myParameterObjName);
        charaCommonParameter= Resources.Load<CharacterCommonParameter>(characterCommonParameter._myParameterObjName);

        base.Start();
        //�v���C���[���Ă��郌�C���[���擾
        _playerlayermask = 1 << _playerObj.layer;
        _animationControll = gameObject.GetComponentInChildren<Animator>();

        _ChoropuNowHp = GetMaxHp();
        _BeforeHp = GetMaxHp();

        _invicibleTime = charaCommonParameter._damageInvincibleTime;

        EnemyCollisionStart();

        _mainJumpPower = choropuParameter._jumpPower;

    }


    private void FixedUpdate()
    {
        _ChoropuNowHp = GetNowHp();

        //ray���o���ăv���C���[��҂�
        if (_ischase == false)
        {
            UseGravity();

            switch (_angletype)
            {
                //���Ƀ��C���o���G
                case Objrayangle.Left:
                    Leftray();
                    break;

                //�E�Ƀ��C���o���G
                case Objrayangle.Right:
                    Rightray();
                    break;
            }
        }
        //�v���C���[���������疳���ɒǂ�������
        else
        {


            // �I�u�W�F�N�g�Ԃ̋����v��
            objectdistance = gameObject.transform.position.x - _playerObj.transform.position.x;

            //���̗̑͂Ƃ��̑O�̗̑͂������ł���Ȃ�ړ��ł���
            if (Mathf.Abs(objectdistance) >= _distanceRange )
            {
                MoveAround();
            }
            else
            {
                StanTime();
            }


            if (_isjump == false)
            {
                UseGravity();
            }
            else
            {
                Jump();
            }
        }

        PlayerBoxCollision();
        InvincibleTimeUpdate();

        if (_isanime == true)
        {
            DerayAnimation();
        }

        if (_isDead == true)
        {
            gameObject.SetActive(false);
        }

    }
    /// <summary>
    /// �E��ray������
    /// </summary>
    private void Rightray()
    {
        //�f�o�b�O�p
        Debug.DrawRay(new Vector2(transform.position.x + choropuParameter._startray, transform.position.y), Vector2.right * _raylength, Color.red, _raylength);

        //�v���C���[�ɂ��Ă��郌�C���[�����Ƀ��C�̐ڐG���������悤�ɂ��Ă�
        RaycastHit2D _objhit = Physics2D.Raycast(new Vector2(transform.position.x + choropuParameter._startray, transform.position.y), Vector2.right, _raylength, _playerlayermask);
        if (_objhit)
        {
            _isanime = true;
        }
    }

    /// <summary>
    /// ����ray������
    /// </summary>
    private void Leftray()
    {
        //�f�o�b�N�p
        Debug.DrawRay(new Vector2(transform.position.x - choropuParameter._startray, transform.position.y), Vector2.left * _raylength, Color.red,_raylength);

        RaycastHit2D _objhit = Physics2D.Raycast(new Vector2(transform.position.x - choropuParameter._startray, transform.position.y), Vector2.left, _raylength, _playerlayermask);

        if (_objhit)
        {
            _isanime = true;
        }

    }

    /// <summary>
    /// �W�����v����
    /// </summary>
    private void Jump()
    {
        //�W�����v
        this.transform.Translate(Vector3.up * _mainJumpPower * Time.fixedDeltaTime);
        //���̃t���[���ł̃W�����v�͂��v�Z����B
        _mainJumpPower -= choropuParameter._gravity * Time.fixedDeltaTime;

        //�W�����v�͂��[���ŁA�ǂɐG��Ă��Ȃ�������
        if (_mainJumpPower <= 0)
        {
            //�W�����v�̏�����
            UseGravity();

            if (_isGroundTouch == true)
            {

                _isjump = false;
                _mainJumpPower = choropuParameter._jumpPower;

            }

        }

    }

    //�A�j���[�V������x�ꂳ����B
    private void DerayAnimation()
    {

        _animationControll.SetBool("Open_Anim", true);


        deraycount += Time.fixedDeltaTime;
        if(deraycount > choropuParameter._animationDeray)
        {
            _ischase = true;
            _isanime = false;
            deraycount = 0;
        }

    }

    /// <summary>
    /// �ǂ������܂��@
    /// </summary>
    private void MoveAround()
    {

        //���g���v���C���[�����ɂ����獶�Ɉړ�
        if (objectdistance > 0)
        { 
            LeftWallRay();
            _animationControll.SetBool("Walk_Anim", true);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);


            if (_isLeftWallTouch == true)
            {
                _isjump = true;
            }

            else
            {
                transform.Translate(Vector2.left * choropuParameter._moveSpeed * Time.fixedDeltaTime);
            }
        }

        //���g���v���C���[���E�ɂ�����E�Ɉړ�
        else
        {
            RightWallRay();

            _animationControll.SetBool("Walk_Anim", true);

            transform.rotation = Quaternion.Euler(0f, 180f, 0f);


            if (_isRightWallTouch == true)
            {
                _isjump = true;
            }

            else
            {
                transform.Translate(Vector2.left * choropuParameter._moveSpeed * Time.fixedDeltaTime);
            }

        }

    }

    private void StanTime()
    {
        _timeCount += Time.fixedDeltaTime;
        if (_timeCount > _invicibleTime)
        {
            _timeCount = 0;
            _BeforeHp = GetNowHp();
        }

    }
}