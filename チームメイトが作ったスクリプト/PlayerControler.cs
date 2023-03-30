using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : CharacterManager
{
    //挙動分岐用enum(値は挙動優先度)
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

    //挙動分岐用
    private PLAYER_MOTION _playerMotion = PLAYER_MOTION.IDLE;

    //横入力フラグ
    private bool _isInputHorizontal = false;

    //ジャンプ入力開始フラグ
    private bool _isInputJumpDown = false;

    //ジャンプ入力フラグ
    private bool _isInputJump = false;

    //オイル射し入力フラグ
    private bool _isInputInsertOil = false;

    //ブリンク入力フラグ
    private bool _isInputBlink = false;

    //攻撃入力フラグ
    private bool _isInputAttack = false;

    //落下フラグ
    private bool _isFall = false;

    //ブリンクフラグ
    private bool _isBlink = false;

    //歩きスクリプト格納用
    private PlayerWalk _playerWalk;

    //ジャンプスクリプト格納用
    private PlayerJumpNoma _playerJump;

    //落下スクリプト格納用
    private PlayerFall _playerFall;

    //ブリンクスクリプト格納用
    private PlayerBlink _playerBlink;

    //攻撃スクリプト格納用
    private PlayerAttack _playerAttack;

    //オイルスクリプト格納用
    private PlayerOil _playerOil;

    //被攻撃スクリプト格納用
    private PlayerTakeHit _playerTakeHit;

    //入力値格納用
    private Vector2 _stickInput;

    //プレイヤーのモデル格納用
    [SerializeField, Header("プレイヤーのモデル")]
    private Transform _playerModelTransform;

    //攻撃のアニメーションクリップ格納用
    [SerializeField, Header("攻撃のアニメーション")]
    private AnimationClip[] _attackAnimClip;

    //攻撃のアニメーションの長さ格納用
    private float[] _attackAnimLength;

    //プレイヤーのAnimator取得用
    private Animator _playerAnim;

    //プレイヤー右向きフラグ
    private bool _isDirectionRight = true;

    //歩きフラグ
    private bool _isWalk = false;

    //現在の攻撃パターン番号格納用
    private int _attackAnimNumber = 0;

    //次回攻撃フラグ
    private bool _isNextAttack = false;

    [SerializeField] private GameObject _gameOverUI = default;

    //ブリンクのアイドル時間
    private float _blinkTimer = 0;

    [SerializeField, Header("ブリンクできるようになるまでの時間")]
    private float _blinkReCastTime = 2;

    private void Awake()
    {
        #region ローカル変数

        //プレイヤー挙動親クラス取得用
        PlayerMotionManager[] playerMotionManagers;

        //自身のCharacterManager取得用
        CharacterManager characterManager;

        //自身のPlayerControler取得
        PlayerControler playerControler;

        #endregion


        //コリジョン初期設定
        CollisionStart();

        //プレイヤー挙動親クラスを全て取得
        playerMotionManagers = GetComponents<PlayerMotionManager>();

        //プレイヤーのAnimator取得
        _playerAnim = GetComponent<Animator>();

        //自身のCollisionCast取得
        characterManager = GetComponent<CharacterManager>();

        //自身のPlayerControler取得
        playerControler = GetComponent<PlayerControler>();

        //プレイヤー挙動親クラスに必要なデータを全て格納
        foreach (PlayerMotionManager playerMotion in playerMotionManagers)
        {
            //自身のクラスを渡す
            playerMotion.GetPlayerClass(characterManager, playerControler);
        }

        //歩きスクリプト格納
        _playerWalk = GetComponent<PlayerWalk>();

        //ジャンプスクリプト格納
        _playerJump = GetComponent<PlayerJumpNoma>();

        //落下スクリプト格納
        _playerFall = GetComponent<PlayerFall>();

        //ブリンクスクリプト格納
        _playerBlink = GetComponent<PlayerBlink>();

        //攻撃スクリプト格納
        _playerAttack = GetComponent<PlayerAttack>();

        //オイルスクリプト格納
        _playerOil = GetComponent<PlayerOil>();

        //被攻撃スクリプト格納
        _playerTakeHit = GetComponent<PlayerTakeHit>();

        //攻撃のアニメーションの長さ配列長指定
        _attackAnimLength = new float[_attackAnimClip.Length];

        //攻撃のアニメーションの長さ格納
        for (int i = 0; i < _attackAnimLength.Length; i++)
        {
            _attackAnimLength[i] = _attackAnimClip[i].length;
        }

    }



    /// <summary>
    /// 入力処理
    /// </summary>
    private void Update()
    {
        //---------デバック用----------------
        //横入力されたか
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            //横入力フラグtrue
            _isInputHorizontal = true;

            //入力値格納
            _stickInput.x = Input.GetAxis("Horizontal");
            _stickInput.y = Input.GetAxis("Vertical");
        }
        else
        {
            //横入力フラグfalse
            _isInputHorizontal = false;

            //入力値格納
            _stickInput = Vector2.zero;
        }

        //ジャンプ入力されたか
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ジャンプ入力開始フラグtrue
            _isInputJumpDown = true;
        }

        //ジャンプ入力されているか
        if (Input.GetKey(KeyCode.Space))
        {
            //ジャンプ入力中フラグtrue
            _isInputJump = true;
        }
        else
        {
            //ジャンプ入力中フラグfalse
            _isInputJump = false;
        }

        //オイル差し入力されたか
        if (Input.GetKeyDown(KeyCode.F))
        {
            //オイル差し入力フラグtrue
            _isInputInsertOil = true;
        }

        //ブリンク入力されたか
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //ブリンク入力フラグtrue
            _isInputBlink = true;
        }

        //攻撃入力されたか
        if (Input.GetKeyDown(KeyCode.K))
        {
            //攻撃入力フラグtrue
            _isInputAttack = true;
        }
        //---------------------------------------------



        //    //入力値格納
        //    _stickInput.x = Input.GetAxis("Horizontal");
        //    _stickInput.y = Input.GetAxis("Vertical");

        //    //横入力されたか
        //    if (_stickInput.x != 0)
        //    {
        //        //横入力フラグtrue
        //        _isInputHorizontal = true;
        //    }
        //    else
        //    {
        //        //横入力フラグfalse
        //        _isInputHorizontal = false;
        //    }

        //    //ジャンプ入力されたか
        //    if (Input.GetKeyDown("joystick button 0"))
        //    {
        //        //ジャンプ入力開始フラグtrue
        //        _isInputJumpDown = true;
        //    }

        //    //ジャンプ入力されているか
        //    if (Input.GetKey("joystick button 0"))
        //    {
        //        //ジャンプ入力中フラグtrue
        //        _isInputJump = true;
        //    }
        //    else
        //    {
        //        //ジャンプ入力中フラグfalse
        //        _isInputJump = false;
        //    }

        //    //オイル差し入力されたか
        //    if (Input.GetKeyDown("joystick button 4"))
        //    {
        //        //オイル差し入力フラグtrue
        //        _isInputInsertOil = true;
        //    }

        //    //ブリンク入力されたか
        //    if (Input.GetAxisRaw("Trigger") < 0 && _playerOil.GetIsInsertOil() && _blinkTimer >= _blinkReCastTime)
        //    {
        //        //ブリンク入力フラグtrue
        //        _isInputBlink = true;

        //        _blinkTimer = 0;
        //    }

        //    //攻撃入力されたか
        //    if (Input.GetKeyDown("joystick button 5"))
        //    {
        //        //攻撃入力フラグtrue
        //        _isInputAttack = true;
        //    }
    }



    /// <summary>
    /// 挙動分岐処理
    /// </summary>
    private void FixedUpdate()
    {
        InvincibleTimeUpdate();

        #region 入力チェック

        _blinkTimer += Time.fixedDeltaTime;

        #region 移動

        //横移動入力されたか
        if (_isInputHorizontal)
        {
            //接地判定取得
            GroundRay();

            //接地しているか
            if (_isGroundTouch)
            {
                //次の挙動を歩きへ
                _playerMotion = ChangeMotion(PLAYER_MOTION.WALK,
                                                _playerMotion);
            }
        }

        #endregion


        #region ジャンプ

        //ジャンプ入力されたか
        if (_isInputJumpDown)
        {
            //接地判定取得
            GroundRay();

            //接地しているか
            if (_isGroundTouch)
            {
                //次の挙動をジャンプへ
                _playerMotion = ChangeMotion(PLAYER_MOTION.JUMP,
                                                _playerMotion);
            }
        }

        #endregion


        #region ブリンク

        //ブリンク入力されたか、又はブリンク中か
        if (_isInputBlink)
        {
            //ブリンク使用可能フラグ
            bool isBlinkReady;


            //ブリンク使用可能フラグ取得
            isBlinkReady = _playerBlink.GetIsBlink();

            //ブリンクできるか
            if (isBlinkReady)
            {
                //次の挙動をブリンクへ
                _playerMotion = ChangeMotion(PLAYER_MOTION.STEP,
                                                    _playerMotion);
            }
        }

        #endregion


        #region 落下

        //落下フラグがtrueか
        if (_isFall)
        {
            //次の挙動を落下へ
            _playerMotion = ChangeMotion(PLAYER_MOTION.FALL, _playerMotion);
        }

        #endregion


        #region 攻撃

        if (_isInputAttack)
        {
            //次の挙動を攻撃へ
            _playerMotion = ChangeMotion(PLAYER_MOTION.ATTACK,
                                                _playerMotion);
        }

        #endregion


        #region ヒット

        //攻撃をくらったか
        if (_isDamage)
        {
            //被攻撃フラグ初期化
            _isDamage = false;

            //挙動更新
            _playerMotion = ChangeMotion(PLAYER_MOTION.HIT, _playerMotion);
        }

        #endregion


        #region デス

        //死亡しているか
        if (_isDead)
        {
            //デスモーションへ
            _playerMotion = ChangeMotion(PLAYER_MOTION.DEATH, _playerMotion);
        }

        #endregion

        #endregion


        //オイルを差すか
        if (_isInputInsertOil && !_isBlink)
        {
            //オイル差し
            _playerOil.InsertOil();
        }

        //挙動により分岐
        switch (_playerMotion)
        {
            #region アイドル

            //アイドル
            case PLAYER_MOTION.IDLE:
                break;

            #endregion


            #region 歩き

            //歩き
            case PLAYER_MOTION.WALK:
                //移動フラグ
                bool isMove;

                //油差しフラグ取得用
                bool isInsertOil1;

                //移動速度倍率取得用
                float walkSpeedMagnification;


                //油差しフラグ取得
                isInsertOil1 = _playerOil.GetIsInsertOil();

                //油がさされているか
                if (isInsertOil1)
                {
                    //移動速度倍率取得
                    walkSpeedMagnification = _playerWalk.GetWalkSpeedMagnification();
                }
                else
                {
                    //移動速度倍率初期化
                    walkSpeedMagnification = 1;
                }

                //歩きアニメーション再生速度更新
                _playerAnim.SetFloat("walkSpeed", walkSpeedMagnification);

                //歩きアニメーション
                _playerAnim.SetBool("isWalk", true);

                //歩き
                isMove = _playerWalk.Walk(isInsertOil1, false, Input.GetAxisRaw("Horizontal"));

                //歩いていないか
                if (!isMove)
                {
                    //モーション初期化
                    DefaultMotion();
                }

                break;

            #endregion


            #region ジャンプ

            //ジャンプ
            case PLAYER_MOTION.JUMP:
                //ジャンプ高度取得用
                float jumpHeight;


                //ジャンプ
                jumpHeight = _playerJump.Jump(_isInputJump);

                //横移動
                _playerWalk.Walk(false, true, Input.GetAxisRaw("Horizontal"));

                //ジャンプアニメーションの値代入
                _playerAnim.SetFloat("verticalMove", jumpHeight);

                break;

            #endregion


            #region 攻撃

            //攻撃
            case PLAYER_MOTION.ATTACK:
                //攻撃アニメーション時間取得用
                float attackAnimTime;

                //接地フラグ取得用
                bool isGround;

                //攻撃アニメーション速度倍率取得用
                float attackAnimMagnification;

                //油差しフラグ取得用
                bool isInsertOil2;


                //油差しフラグ取得
                isInsertOil2 = _playerOil.GetIsInsertOil();

                //接地判定取得
                isGround = GetIsCollision(COLLISION.GROUND);

                //攻撃
                _playerAttack.Attack(_isDirectionRight, isInsertOil2, !isGround);

                //攻撃アニメーション時間取得
                attackAnimTime = _playerAttack.GetAttackTime();

                //攻撃アニメーション速度倍率取得
                attackAnimMagnification = 1 / attackAnimTime * _attackAnimLength[_attackAnimNumber];

                //攻撃アニメーション速度更新
                _playerAnim.SetFloat("attackAnimSpeed", attackAnimMagnification);

                //攻撃中に攻撃入力されたか
                if (!_isNextAttack && _isInputAttack)
                {
                    //次回攻撃確定
                    _isNextAttack = true;

                    //攻撃アニメーション更新
                    _playerAnim.SetTrigger("attackTrigger");
                }

                break;

            #endregion


            #region ブリンク

            //ブリンク
            case PLAYER_MOTION.STEP:
                //ブリンクの向きベクトル取得用
                Vector2 blinkDirection;

                //ブリンクしつつ向きベクトル取得
                blinkDirection = _playerBlink.Blink(_stickInput, _isDirectionRight);

                //ブリンクアニメーション
                _playerAnim.SetFloat("blinkHorizontalMove", blinkDirection.x);
                _playerAnim.SetFloat("blinkVerticalMove", blinkDirection.y);

                break;

            #endregion


            #region 落下

            //落下
            case PLAYER_MOTION.FALL:
                //落下距離取得用
                float fallDownHeight;

                //落下
                fallDownHeight = _playerFall.Fall(_playerJump.ReturnJumpMaxHeight());

                //横移動
                _playerWalk.Walk(false, true, Input.GetAxisRaw("Horizontal"));

                //落下アニメーション
                _playerAnim.SetFloat("verticalMove", fallDownHeight);

                break;

            #endregion


            #region 被攻撃

            //被攻撃
            case PLAYER_MOTION.HIT:
                //
                _playerTakeHit.TakeHit(true);
                
                if (!GetIsCollision(COLLISION.GROUND))
                {
                    _playerFall.Fall(_playerJump.ReturnJumpMaxHeight());
                }

                break;

            #endregion


            #region デス

            //デス
            case PLAYER_MOTION.DEATH:
                //接地しているか
                if (GetIsCollision(COLLISION.GROUND))
                {
                    //デスアニメーション
                    _playerAnim.SetTrigger("deathTrigger");
                    _gameOverUI.SetActive(true);
                }
                else
                {
                    //落下
                    fallDownHeight = _playerFall.Fall(_playerJump.ReturnJumpMaxHeight());

                    //落下アニメーション
                    _playerAnim.SetFloat("verticalMove", fallDownHeight);
                }

                break;

            #endregion
        }


        #region 入力フラグ初期化

        //ブリンク入力フラグ初期化
        _isInputBlink = false;

        //ジャンプ入力フラグ初期化
        _isInputJumpDown = false;

        //攻撃入力初期化
        _isInputAttack = false;

        //油差し入力初期化
        _isInputInsertOil = false;

        #endregion
    }



    /// <summary>
    /// 優先度を見て挙動を更新するメソッド
    /// </summary>
    /// <param name="nextMotion">次に入れる予定の挙動</param>
    /// <param name="nowMotion">現在の挙動</param>
    /// <returns>更新後の挙動</returns>
    private PLAYER_MOTION ChangeMotion(PLAYER_MOTION nextMotion,
                                        PLAYER_MOTION nowMotion)
    {
        //次の挙動が現在の挙動より優先度が高いか
        if ((int)nowMotion < (int)nextMotion)
        {
            //モーション初期化
            DefaultMotion();

            //次の挙動へ
            nowMotion = nextMotion;

            //次の挙動により分岐
            switch (nowMotion)
            {
                case PLAYER_MOTION.ATTACK:
                    //攻撃アニメーション
                    _playerAnim.SetTrigger("attackTrigger");
                    _playerAnim.SetTrigger("attackStartTrigger");
                    _playerAnim.SetBool("isAttack", true);

                    //攻撃入力初期化
                    _isInputAttack = false;

                    break;


                case PLAYER_MOTION.STEP:
                    //ブリンクフラグtrue
                    _isBlink = true;

                    //ブリンクアニメーションtrue
                    _playerAnim.SetBool("isBlink", true);
                    _playerAnim.SetTrigger("blinkTrigger");

                    break;
            }
        }

        return nowMotion;
    }



    /// <summary>
    /// プレイヤーの右向きフラグを返すメソッド
    /// </summary>
    /// <param name="isRight">右向きフラグ</param>
    public void GetPlayerDirection(bool isRight)
    {
        //向きが変わったか
        if (isRight != _isDirectionRight)
        {
            //プレイヤーの角度取得用
            Vector3 playerRotation;


            //プレイヤーの角度取得
            playerRotation = _playerModelTransform.eulerAngles;

            //プレイヤーを逆向きにする
            playerRotation.y += 180;
            _playerModelTransform.eulerAngles = playerRotation;

            //プレイヤー右向きフラグを逆にする
            _isDirectionRight = isRight;
        }
    }

    

    /// <summary>
    /// 落下開始メソッド
    /// </summary>
    public void FallStart()
    {
        //落下パラメータ初期化
        _playerFall.ResetParamater();

        //落下フラグtrue
        _isFall = true;
    }



    /// <summary>
    /// 挙動初期化メソッド
    /// </summary>
    public void DefaultMotion()
    {
        //接地フラグ
        bool isGround;


        //移行前のプレイヤー挙動により分岐
        switch (_playerMotion)
        {
            //歩き
            case PLAYER_MOTION.WALK:
                //歩きアニメーションfalse
                _playerAnim.SetBool("isWalk", false);

                break;

            //落下
            case PLAYER_MOTION.FALL:
                //落下フラグfalse
                _isFall = false;

                break;

            //攻撃
            case PLAYER_MOTION.ATTACK:
                //現在の攻撃パターン番号初期化
                _attackAnimNumber = 0;

                //攻撃アニメーションfalse
                _playerAnim.SetBool("isAttack", false);

                break;

            //ブリンク
            case PLAYER_MOTION.STEP:
                //ブリンクフラグfalse
                _isBlink = false;

                //ブリンクアニメーションfalse
                _playerAnim.SetBool("isBlink", false);

                break;
        }

        //接地判定取得
        isGround = GetIsCollision(COLLISION.GROUND);

        //接地しているか
        if (isGround)
        {
            //挙動をアイドルへ
            _playerMotion = PLAYER_MOTION.IDLE;
        }
        else
        {
            //挙動を落下へ
            _playerMotion = PLAYER_MOTION.FALL;
        }
    }



    /// <summary>
    /// 攻撃終了時AnimationEvent呼び出し用メソッド
    /// </summary>
    public void AttackEnd()
    {
        //連続で攻撃するか
        if (_isNextAttack)
        {
            /*次フレームで次の攻撃へ*/

            //攻撃パターン更新(0～2)
            _attackAnimNumber++;
            if (_attackAnimNumber > 2)
            {
                _attackAnimNumber = 0;
            }
        }
        else
        {
            /*攻撃終了*/

            //モーション初期化
            DefaultMotion();
        }

        //次回攻撃フラグ初期化
        _isNextAttack = false;

        //攻撃初期化
        _playerAttack.AttackEnd();
    }



    public void ChangeEventScene()
    {
        _playerMotion = ChangeMotion(PLAYER_MOTION.EVENT_SCENE, _playerMotion);
    }
}
