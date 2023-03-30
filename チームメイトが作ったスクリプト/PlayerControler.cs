using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : CharacterManager
{
    //��������penum(�l�͋����D��x)
    private enum PLAYER_MOTION
    {
        IDLE = 0,
        WALK = 1,
        ATTACK = 4,
        STEP = 6,
        JUMP = 2,
        FALL = 3,
        HIT = 5,
        EVENT_SCENE = 7,
        DEATH = 8,
    }

    //��������p
    private PLAYER_MOTION _playerMotion = PLAYER_MOTION.IDLE;

    //�����̓t���O
    private bool _isInputHorizontal = false;

    //�W�����v���͊J�n�t���O
    private bool _isInputJumpDown = false;

    //�W�����v���̓t���O
    private bool _isInputJump = false;

    //�I�C���˂����̓t���O
    private bool _isInputInsertOil = false;

    //�u�����N���̓t���O
    private bool _isInputBlink = false;

    //�U�����̓t���O
    private bool _isInputAttack = false;

    //�����t���O
    private bool _isFall = false;

    //�u�����N�t���O
    private bool _isBlink = false;

    //�����X�N���v�g�i�[�p
    private PlayerWalk _playerWalk;

    //�W�����v�X�N���v�g�i�[�p
    private PlayerJumpNoma _playerJump;

    //�����X�N���v�g�i�[�p
    private PlayerFall _playerFall;

    //�u�����N�X�N���v�g�i�[�p
    private PlayerBlink _playerBlink;

    //�U���X�N���v�g�i�[�p
    private PlayerAttack _playerAttack;

    //�I�C���X�N���v�g�i�[�p
    private PlayerOil _playerOil;

    //��U���X�N���v�g�i�[�p
    private PlayerTakeHit _playerTakeHit;

    //���͒l�i�[�p
    private Vector2 _stickInput;

    //�v���C���[�̃��f���i�[�p
    [SerializeField, Header("�v���C���[�̃��f��")]
    private Transform _playerModelTransform;

    //�U���̃A�j���[�V�����N���b�v�i�[�p
    [SerializeField, Header("�U���̃A�j���[�V����")]
    private AnimationClip[] _attackAnimClip;

    //�U���̃A�j���[�V�����̒����i�[�p
    private float[] _attackAnimLength;

    //�v���C���[��Animator�擾�p
    private Animator _playerAnim;

    //�v���C���[�E�����t���O
    private bool _isDirectionRight = true;

    //�����t���O
    private bool _isWalk = false;

    //���݂̍U���p�^�[���ԍ��i�[�p
    private int _attackAnimNumber = 0;

    //����U���t���O
    private bool _isNextAttack = false;

    [SerializeField] private GameObject _gameOverUI = default;

    //�u�����N�̃A�C�h������
    private float _blinkTimer = 0;

    [SerializeField, Header("�u�����N�ł���悤�ɂȂ�܂ł̎���")]
    private float _blinkReCastTime = 2;

    private void Awake()
    {
        #region ���[�J���ϐ�

        //�v���C���[�����e�N���X�擾�p
        PlayerMotionManager[] playerMotionManagers;

        //���g��CharacterManager�擾�p
        CharacterManager characterManager;

        //���g��PlayerControler�擾
        PlayerControler playerControler;

        #endregion


        //�R���W���������ݒ�
        CollisionStart();

        //�v���C���[�����e�N���X��S�Ď擾
        playerMotionManagers = GetComponents<PlayerMotionManager>();

        //�v���C���[��Animator�擾
        _playerAnim = GetComponent<Animator>();

        //���g��CollisionCast�擾
        characterManager = GetComponent<CharacterManager>();

        //���g��PlayerControler�擾
        playerControler = GetComponent<PlayerControler>();

        //�v���C���[�����e�N���X�ɕK�v�ȃf�[�^��S�Ċi�[
        foreach (PlayerMotionManager playerMotion in playerMotionManagers)
        {
            //���g�̃N���X��n��
            playerMotion.GetPlayerClass(characterManager, playerControler);
        }

        //�����X�N���v�g�i�[
        _playerWalk = GetComponent<PlayerWalk>();

        //�W�����v�X�N���v�g�i�[
        _playerJump = GetComponent<PlayerJumpNoma>();

        //�����X�N���v�g�i�[
        _playerFall = GetComponent<PlayerFall>();

        //�u�����N�X�N���v�g�i�[
        _playerBlink = GetComponent<PlayerBlink>();

        //�U���X�N���v�g�i�[
        _playerAttack = GetComponent<PlayerAttack>();

        //�I�C���X�N���v�g�i�[
        _playerOil = GetComponent<PlayerOil>();

        //��U���X�N���v�g�i�[
        _playerTakeHit = GetComponent<PlayerTakeHit>();

        //�U���̃A�j���[�V�����̒����z�񒷎w��
        _attackAnimLength = new float[_attackAnimClip.Length];

        //�U���̃A�j���[�V�����̒����i�[
        for (int i = 0; i < _attackAnimLength.Length; i++)
        {
            _attackAnimLength[i] = _attackAnimClip[i].length;
        }

    }



    /// <summary>
    /// ���͏���
    /// </summary>
    private void Update()
    {
        //---------�f�o�b�N�p----------------
        //�����͂��ꂽ��
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            //�����̓t���Otrue
            _isInputHorizontal = true;

            //���͒l�i�[
            _stickInput.x = Input.GetAxis("Horizontal");
            _stickInput.y = Input.GetAxis("Vertical");
        }
        else
        {
            //�����̓t���Ofalse
            _isInputHorizontal = false;

            //���͒l�i�[
            _stickInput = Vector2.zero;
        }

        //�W�����v���͂��ꂽ��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //�W�����v���͊J�n�t���Otrue
            _isInputJumpDown = true;
        }

        //�W�����v���͂���Ă��邩
        if (Input.GetKey(KeyCode.Space))
        {
            //�W�����v���͒��t���Otrue
            _isInputJump = true;
        }
        else
        {
            //�W�����v���͒��t���Ofalse
            _isInputJump = false;
        }

        //�I�C���������͂��ꂽ��
        if (Input.GetKeyDown(KeyCode.F))
        {
            //�I�C���������̓t���Otrue
            _isInputInsertOil = true;
        }

        //�u�����N���͂��ꂽ��
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //�u�����N���̓t���Otrue
            _isInputBlink = true;
        }

        //�U�����͂��ꂽ��
        if (Input.GetKeyDown(KeyCode.K))
        {
            //�U�����̓t���Otrue
            _isInputAttack = true;
        }
        //---------------------------------------------



        //    //���͒l�i�[
        //    _stickInput.x = Input.GetAxis("Horizontal");
        //    _stickInput.y = Input.GetAxis("Vertical");

        //    //�����͂��ꂽ��
        //    if (_stickInput.x != 0)
        //    {
        //        //�����̓t���Otrue
        //        _isInputHorizontal = true;
        //    }
        //    else
        //    {
        //        //�����̓t���Ofalse
        //        _isInputHorizontal = false;
        //    }

        //    //�W�����v���͂��ꂽ��
        //    if (Input.GetKeyDown("joystick button 0"))
        //    {
        //        //�W�����v���͊J�n�t���Otrue
        //        _isInputJumpDown = true;
        //    }

        //    //�W�����v���͂���Ă��邩
        //    if (Input.GetKey("joystick button 0"))
        //    {
        //        //�W�����v���͒��t���Otrue
        //        _isInputJump = true;
        //    }
        //    else
        //    {
        //        //�W�����v���͒��t���Ofalse
        //        _isInputJump = false;
        //    }

        //    //�I�C���������͂��ꂽ��
        //    if (Input.GetKeyDown("joystick button 4"))
        //    {
        //        //�I�C���������̓t���Otrue
        //        _isInputInsertOil = true;
        //    }

        //    //�u�����N���͂��ꂽ��
        //    if (Input.GetAxisRaw("Trigger") < 0 && _playerOil.GetIsInsertOil() && _blinkTimer >= _blinkReCastTime)
        //    {
        //        //�u�����N���̓t���Otrue
        //        _isInputBlink = true;

        //        _blinkTimer = 0;
        //    }

        //    //�U�����͂��ꂽ��
        //    if (Input.GetKeyDown("joystick button 5"))
        //    {
        //        //�U�����̓t���Otrue
        //        _isInputAttack = true;
        //    }
    }



    /// <summary>
    /// �������򏈗�
    /// </summary>
    private void FixedUpdate()
    {
        InvincibleTimeUpdate();

        #region ���̓`�F�b�N

        _blinkTimer += Time.fixedDeltaTime;

        #region �ړ�

        //���ړ����͂��ꂽ��
        if (_isInputHorizontal)
        {
            //�ڒn����擾
            GroundRay();

            //�ڒn���Ă��邩
            if (_isGroundTouch)
            {
                //���̋����������
                _playerMotion = ChangeMotion(PLAYER_MOTION.WALK,
                                                _playerMotion);
            }
        }

        #endregion


        #region �W�����v

        //�W�����v���͂��ꂽ��
        if (_isInputJumpDown)
        {
            //�ڒn����擾
            GroundRay();

            //�ڒn���Ă��邩
            if (_isGroundTouch)
            {
                //���̋������W�����v��
                _playerMotion = ChangeMotion(PLAYER_MOTION.JUMP,
                                                _playerMotion);
            }
        }

        #endregion


        #region �u�����N

        //�u�����N���͂��ꂽ���A���̓u�����N����
        if (_isInputBlink)
        {
            //�u�����N�g�p�\�t���O
            bool isBlinkReady;


            //�u�����N�g�p�\�t���O�擾
            isBlinkReady = _playerBlink.GetIsBlink();

            //�u�����N�ł��邩
            if (isBlinkReady)
            {
                //���̋������u�����N��
                _playerMotion = ChangeMotion(PLAYER_MOTION.STEP,
                                                    _playerMotion);
            }
        }

        #endregion


        #region ����

        //�����t���O��true��
        if (_isFall)
        {
            //���̋����𗎉���
            _playerMotion = ChangeMotion(PLAYER_MOTION.FALL, _playerMotion);
        }

        #endregion


        #region �U��

        if (_isInputAttack)
        {
            //���̋������U����
            _playerMotion = ChangeMotion(PLAYER_MOTION.ATTACK,
                                                _playerMotion);
        }

        #endregion


        #region �q�b�g

        //�U�������������
        if (_isDamage)
        {
            //��U���t���O������
            _isDamage = false;

            //�����X�V
            _playerMotion = ChangeMotion(PLAYER_MOTION.HIT, _playerMotion);
        }

        #endregion


        #region �f�X

        //���S���Ă��邩
        if (_isDead)
        {
            //�f�X���[�V������
            _playerMotion = ChangeMotion(PLAYER_MOTION.DEATH, _playerMotion);
        }

        #endregion

        #endregion


        //�I�C����������
        if (_isInputInsertOil && !_isBlink)
        {
            //�I�C������
            _playerOil.InsertOil();
        }

        //�����ɂ�蕪��
        switch (_playerMotion)
        {
            #region �A�C�h��

            //�A�C�h��
            case PLAYER_MOTION.IDLE:
                break;

            #endregion


            #region ����

            //����
            case PLAYER_MOTION.WALK:
                //�ړ��t���O
                bool isMove;

                //�������t���O�擾�p
                bool isInsertOil1;

                //�ړ����x�{���擾�p
                float walkSpeedMagnification;


                //�������t���O�擾
                isInsertOil1 = _playerOil.GetIsInsertOil();

                //����������Ă��邩
                if (isInsertOil1)
                {
                    //�ړ����x�{���擾
                    walkSpeedMagnification = _playerWalk.GetWalkSpeedMagnification();
                }
                else
                {
                    //�ړ����x�{��������
                    walkSpeedMagnification = 1;
                }

                //�����A�j���[�V�����Đ����x�X�V
                _playerAnim.SetFloat("walkSpeed", walkSpeedMagnification);

                //�����A�j���[�V����
                _playerAnim.SetBool("isWalk", true);

                //����
                isMove = _playerWalk.Walk(isInsertOil1, false, Input.GetAxisRaw("Horizontal"));

                //�����Ă��Ȃ���
                if (!isMove)
                {
                    //���[�V����������
                    DefaultMotion();
                }

                break;

            #endregion


            #region �W�����v

            //�W�����v
            case PLAYER_MOTION.JUMP:
                //�W�����v���x�擾�p
                float jumpHeight;


                //�W�����v
                jumpHeight = _playerJump.Jump(_isInputJump);

                //���ړ�
                _playerWalk.Walk(false, true, Input.GetAxisRaw("Horizontal"));

                //�W�����v�A�j���[�V�����̒l���
                _playerAnim.SetFloat("verticalMove", jumpHeight);

                break;

            #endregion


            #region �U��

            //�U��
            case PLAYER_MOTION.ATTACK:
                //�U���A�j���[�V�������Ԏ擾�p
                float attackAnimTime;

                //�ڒn�t���O�擾�p
                bool isGround;

                //�U���A�j���[�V�������x�{���擾�p
                float attackAnimMagnification;

                //�������t���O�擾�p
                bool isInsertOil2;


                //�������t���O�擾
                isInsertOil2 = _playerOil.GetIsInsertOil();

                //�ڒn����擾
                isGround = GetIsCollision(COLLISION.GROUND);

                //�U��
                _playerAttack.Attack(_isDirectionRight, isInsertOil2, !isGround);

                //�U���A�j���[�V�������Ԏ擾
                attackAnimTime = _playerAttack.GetAttackTime();

                //�U���A�j���[�V�������x�{���擾
                attackAnimMagnification = 1 / attackAnimTime * _attackAnimLength[_attackAnimNumber];

                //�U���A�j���[�V�������x�X�V
                _playerAnim.SetFloat("attackAnimSpeed", attackAnimMagnification);

                //�U�����ɍU�����͂��ꂽ��
                if (!_isNextAttack && _isInputAttack)
                {
                    //����U���m��
                    _isNextAttack = true;

                    //�U���A�j���[�V�����X�V
                    _playerAnim.SetTrigger("attackTrigger");
                }

                break;

            #endregion


            #region �u�����N

            //�u�����N
            case PLAYER_MOTION.STEP:
                //�u�����N�̌����x�N�g���擾�p
                Vector2 blinkDirection;

                //�u�����N�������x�N�g���擾
                blinkDirection = _playerBlink.Blink(_stickInput, _isDirectionRight);

                //�u�����N�A�j���[�V����
                _playerAnim.SetFloat("blinkHorizontalMove", blinkDirection.x);
                _playerAnim.SetFloat("blinkVerticalMove", blinkDirection.y);

                break;

            #endregion


            #region ����

            //����
            case PLAYER_MOTION.FALL:
                //���������擾�p
                float fallDownHeight;

                //����
                fallDownHeight = _playerFall.Fall(_playerJump.ReturnJumpMaxHeight());

                //���ړ�
                _playerWalk.Walk(false, true, Input.GetAxisRaw("Horizontal"));

                //�����A�j���[�V����
                _playerAnim.SetFloat("verticalMove", fallDownHeight);

                break;

            #endregion


            #region ��U��

            //��U��
            case PLAYER_MOTION.HIT:
                //
                _playerTakeHit.TakeHit(true);
                
                if (!GetIsCollision(COLLISION.GROUND))
                {
                    _playerFall.Fall(_playerJump.ReturnJumpMaxHeight());
                }

                break;

            #endregion


            #region �f�X

            //�f�X
            case PLAYER_MOTION.DEATH:
                //�ڒn���Ă��邩
                if (GetIsCollision(COLLISION.GROUND))
                {
                    //�f�X�A�j���[�V����
                    _playerAnim.SetTrigger("deathTrigger");
                    _gameOverUI.SetActive(true);
                }
                else
                {
                    //����
                    fallDownHeight = _playerFall.Fall(_playerJump.ReturnJumpMaxHeight());

                    //�����A�j���[�V����
                    _playerAnim.SetFloat("verticalMove", fallDownHeight);
                }

                break;

            #endregion
        }


        #region ���̓t���O������

        //�u�����N���̓t���O������
        _isInputBlink = false;

        //�W�����v���̓t���O������
        _isInputJumpDown = false;

        //�U�����͏�����
        _isInputAttack = false;

        //���������͏�����
        _isInputInsertOil = false;

        #endregion
    }



    /// <summary>
    /// �D��x�����ċ������X�V���郁�\�b�h
    /// </summary>
    /// <param name="nextMotion">���ɓ����\��̋���</param>
    /// <param name="nowMotion">���݂̋���</param>
    /// <returns>�X�V��̋���</returns>
    private PLAYER_MOTION ChangeMotion(PLAYER_MOTION nextMotion,
                                        PLAYER_MOTION nowMotion)
    {
        //���̋��������݂̋������D��x��������
        if ((int)nowMotion < (int)nextMotion)
        {
            //���[�V����������
            DefaultMotion();

            //���̋�����
            nowMotion = nextMotion;

            //���̋����ɂ�蕪��
            switch (nowMotion)
            {
                case PLAYER_MOTION.ATTACK:
                    //�U���A�j���[�V����
                    _playerAnim.SetTrigger("attackTrigger");
                    _playerAnim.SetTrigger("attackStartTrigger");
                    _playerAnim.SetBool("isAttack", true);

                    //�U�����͏�����
                    _isInputAttack = false;

                    break;


                case PLAYER_MOTION.STEP:
                    //�u�����N�t���Otrue
                    _isBlink = true;

                    //�u�����N�A�j���[�V����true
                    _playerAnim.SetBool("isBlink", true);
                    _playerAnim.SetTrigger("blinkTrigger");

                    break;
            }
        }

        return nowMotion;
    }



    /// <summary>
    /// �v���C���[�̉E�����t���O��Ԃ����\�b�h
    /// </summary>
    /// <param name="isRight">�E�����t���O</param>
    public void GetPlayerDirection(bool isRight)
    {
        //�������ς������
        if (isRight != _isDirectionRight)
        {
            //�v���C���[�̊p�x�擾�p
            Vector3 playerRotation;


            //�v���C���[�̊p�x�擾
            playerRotation = _playerModelTransform.eulerAngles;

            //�v���C���[���t�����ɂ���
            playerRotation.y += 180;
            _playerModelTransform.eulerAngles = playerRotation;

            //�v���C���[�E�����t���O���t�ɂ���
            _isDirectionRight = isRight;
        }
    }

    

    /// <summary>
    /// �����J�n���\�b�h
    /// </summary>
    public void FallStart()
    {
        //�����p�����[�^������
        _playerFall.ResetParamater();

        //�����t���Otrue
        _isFall = true;
    }



    /// <summary>
    /// �������������\�b�h
    /// </summary>
    public void DefaultMotion()
    {
        //�ڒn�t���O
        bool isGround;


        //�ڍs�O�̃v���C���[�����ɂ�蕪��
        switch (_playerMotion)
        {
            //����
            case PLAYER_MOTION.WALK:
                //�����A�j���[�V����false
                _playerAnim.SetBool("isWalk", false);

                break;

            //����
            case PLAYER_MOTION.FALL:
                //�����t���Ofalse
                _isFall = false;

                break;

            //�U��
            case PLAYER_MOTION.ATTACK:
                //���݂̍U���p�^�[���ԍ�������
                _attackAnimNumber = 0;

                //�U���A�j���[�V����false
                _playerAnim.SetBool("isAttack", false);

                break;

            //�u�����N
            case PLAYER_MOTION.STEP:
                //�u�����N�t���Ofalse
                _isBlink = false;

                //�u�����N�A�j���[�V����false
                _playerAnim.SetBool("isBlink", false);

                break;
        }

        //�ڒn����擾
        isGround = GetIsCollision(COLLISION.GROUND);

        //�ڒn���Ă��邩
        if (isGround)
        {
            //�������A�C�h����
            _playerMotion = PLAYER_MOTION.IDLE;
        }
        else
        {
            //�����𗎉���
            _playerMotion = PLAYER_MOTION.FALL;
        }
    }



    /// <summary>
    /// �U���I����AnimationEvent�Ăяo���p���\�b�h
    /// </summary>
    public void AttackEnd()
    {
        //�A���ōU�����邩
        if (_isNextAttack)
        {
            /*���t���[���Ŏ��̍U����*/

            //�U���p�^�[���X�V(0�`2)
            _attackAnimNumber++;
            if (_attackAnimNumber > 2)
            {
                _attackAnimNumber = 0;
            }
        }
        else
        {
            /*�U���I��*/

            //���[�V����������
            DefaultMotion();
        }

        //����U���t���O������
        _isNextAttack = false;

        //�U��������
        _playerAttack.AttackEnd();
    }



    public void ChangeEventScene()
    {
        _playerMotion = ChangeMotion(PLAYER_MOTION.EVENT_SCENE, _playerMotion);
    }
}
