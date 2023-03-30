using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bossmove : SmallEnemyManager
{
    private EnemyBossParameter bossParameter;

    [SerializeField]
    private GameObject _bossmesh;

    [SerializeField, Header("���ōU�����鋗��")]
    private float _attackdistance = 10;

    [SerializeField, Header("��������")]
    private float _heightrange = 30;

    [SerializeField, Header("�X�e�[�W�N���A�A�j���[�V����")]
    private GameObject _clearAnimation;

    private float _objdistance;
    private float objPosition = default;
    private float nowHp = default;
    private float _jumpAnimationDeray = 0;
    private float _jumpStartPos = default;

    private bool _isSwordAttack = false;
    private bool _isjumpAttack = false;
    private bool _isDownAttack = false;
    private bool _isDeath = false;

    private Animator _animator;
    private Vector3 _playerpos;


    private void Start()
    {
        bossParameter = Resources.Load<EnemyBossParameter>(characterCommonParameter._myParameterObjName);
        base.Start();
        //�I�u�W�F�N�g�̋���
        _objdistance = Mathf.Abs(_playerObj.transform.position.x - transform.position.x);
        _animator = _bossmesh.GetComponent<Animator>();
        EnemyCollisionStart();

    }

    void FixedUpdate()
    {
        InvincibleTimeUpdate();

        nowHp = GetNowHp();
        print(nowHp);
        DeathCheck();

        //���񂾂Ƃ��̔���
        if (_isDeath == false)
        {

            PlayerBoxCollision();
            RightWallRay();
            LeftWallRay();
            //�U�������Ă��Ȃ��Ƃ�������ς���
            if(_isSwordAttack == false && _isjumpAttack == false)
            {

                TurnObject();

            }
            //�I�u�W�F�N�g�̋���
            _objdistance = Mathf.Abs(_playerObj.transform.localPosition.x - transform.localPosition.x);

            #region �U������

            //����������Ă��āA�W�����v�U�����Ă��Ȃ��Ƃ�
            if (_objdistance < _attackdistance && _isjumpAttack == false)
            {
                print("���U��");
                _animator.SetBool("Attack", true);
                _animator.SetBool("Jump", false);
                _isSwordAttack = true;
            }

            #endregion

            #region �W�����v����
            //��苗������Ă���ꍇ
            if (_objdistance >= bossParameter._jumpstartrange && _isjumpAttack == false )
            {
                print("�����Ղ����");
                _isSwordAttack = false;
                _animator.SetBool("Attack", false);
                _animator.SetBool("Jump", true);
                _isjumpAttack = true;
                //�v���C���[�̃|�W�V�����擾�@�W�����v�܂ł̖ړI�n
                _playerpos = _playerObj.transform.position;
                _jumpStartPos = transform.position.y;
            }

            //�W�����v�U���J�n
            if (_isjumpAttack == true)
            {
                _jumpAnimationDeray += Time.fixedDeltaTime;
                //�W�����v���n�߂鎞�Ԃ܂őҋ@������
                if (_jumpAnimationDeray > bossParameter._jumpStartTime)
                {
                    //�W�����v����
                    Jump();
                }
            }
            #endregion
        }
        else
        {
            DeathCheck();
        }

    }

    /// <summary>
    /// �W�����v����
    /// </summary>
    private void Jump()
    {
        //��������
        if (transform.position.y < _heightrange)
        {
            transform.Translate(Vector2.up * bossParameter._speed * Time.deltaTime);
        }
        else
        {
            //�������������ȏ�ɍs���Ȃ��悤�ɂ���
            transform.position = new Vector2(transform.position.x, _heightrange);
        }

        //�v���C���[�̂���ʒu�܂ōs���ĂȂ��ꍇ
        if (transform.position.x > _playerpos.x)
        {
            //�E��Ɉړ�
            transform.Translate(Vector2.left * bossParameter._speed * Time.deltaTime);
        }
        else if (transform.position.x < _playerpos.x)
        {
            //����Ɉړ�
            transform.Translate(Vector2.right * bossParameter._speed * Time.deltaTime);
        }
    

        //�v���C���[�ƃ{�X��x���������ɂȂ�����
        if (Mathf.Round(transform.position.x) == Mathf.Round(_playerpos.x))
        {
            _animator.SetBool("Jump", false);
            _animator.SetBool("Attack", true);
            //�����U���J�n
            DownAttack();
          
            transform.position = new Vector2(_playerpos.x, transform.position.y);
        }

    }

    /// <summary>
    /// �X�^���v�U��
    /// </summary>
    private void DownAttack()
    {
        if (Mathf.Round(transform.position.y) > _jumpStartPos && _isDownAttack == false)
        {
            //���ɗ��������Ă���
            transform.Translate(Vector2.down * bossParameter._speed * bossParameter._stampspeed * Time.deltaTime);
        }
        else
        {
            _isDownAttack = true;
            transform.position = new Vector2(transform.position.x, _jumpStartPos);
            ActionDeray();
        }
    }
    /// <summary>
    /// �W�����v�̃f�B���C����
    /// </summary>
    private void ActionDeray()
    {
        _animator.SetBool("Jump", false);
        _animator.SetBool("Attack", false);

        //�f�B���C����
        _jumpAnimationDeray += Time.fixedDeltaTime;
        //�A�N�V�������I��������Ԍo�߂ōēx�A�N�V�����\�ɂ���
        if (_jumpAnimationDeray >= bossParameter.jumpderaytime)
        {
            print("�S�����Z�b�g������");
            _isDownAttack = false;
            _isjumpAttack = false;
            _jumpAnimationDeray = 0;
        }
    }

    /// <summary>
    /// �����Ă�����ύX
    /// </summary>
    private void TurnObject()
    {
        objPosition = _playerObj.transform.localPosition.x - transform.localPosition.x;
        if (objPosition > 0)
        {
            _bossmesh.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            _bossmesh.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
    }

    /// <summary>
    /// ����ł��邩�̔���
    /// </summary>
    private void DeathCheck()
    {

        bool deathCheack = false;


        if (nowHp <= 0 && _isDeath == false)
        {
            deathCheack = true;
            _isDeath = true;
            _clearAnimation.SetActive(true);

        }

        if (deathCheack == true)
        {
            _animator.SetTrigger("Death");
            GetComponent<Bossmove>().enabled = false;
            ClearCheak();
        }

    }

    private void ClearCheak()
    {
        gameObject.SetActive(false);
    }
}
