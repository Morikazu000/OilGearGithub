using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemychoropu : SmallEnemyManager
{
    private EnemyChoropuParameter choropuParameter;
    private CharacterCommonParameter charaCommonParameter;
    [field: SerializeField, Header("レイの距離")]
    public float _raylength = 5;

    [SerializeField, Header("レイの向き")]
    private Objrayangle _angletype = Objrayangle.Right;

    [SerializeField, Header("距離制限")]
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
        //プレイヤーついているレイヤーを取得
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

        //rayを出してプレイヤーを待つ
        if (_ischase == false)
        {
            UseGravity();

            switch (_angletype)
            {
                //左にレイを出す敵
                case Objrayangle.Left:
                    Leftray();
                    break;

                //右にレイを出す敵
                case Objrayangle.Right:
                    Rightray();
                    break;
            }
        }
        //プレイヤーを見つけたら無限に追いかける
        else
        {


            // オブジェクト間の距離計測
            objectdistance = gameObject.transform.position.x - _playerObj.transform.position.x;

            //今の体力とその前の体力が同じであるなら移動できる
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
    /// 右にrayをだす
    /// </summary>
    private void Rightray()
    {
        //デバッグ用
        Debug.DrawRay(new Vector2(transform.position.x + choropuParameter._startray, transform.position.y), Vector2.right * _raylength, Color.red, _raylength);

        //プレイヤーについているレイヤーだけにレイの接触判定をつけるようにしてる
        RaycastHit2D _objhit = Physics2D.Raycast(new Vector2(transform.position.x + choropuParameter._startray, transform.position.y), Vector2.right, _raylength, _playerlayermask);
        if (_objhit)
        {
            _isanime = true;
        }
    }

    /// <summary>
    /// 左にrayをだす
    /// </summary>
    private void Leftray()
    {
        //デバック用
        Debug.DrawRay(new Vector2(transform.position.x - choropuParameter._startray, transform.position.y), Vector2.left * _raylength, Color.red,_raylength);

        RaycastHit2D _objhit = Physics2D.Raycast(new Vector2(transform.position.x - choropuParameter._startray, transform.position.y), Vector2.left, _raylength, _playerlayermask);

        if (_objhit)
        {
            _isanime = true;
        }

    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    private void Jump()
    {
        //ジャンプ
        this.transform.Translate(Vector3.up * _mainJumpPower * Time.fixedDeltaTime);
        //次のフレームでのジャンプ力を計算する。
        _mainJumpPower -= choropuParameter._gravity * Time.fixedDeltaTime;

        //ジャンプ力がゼロで、壁に触れていなかったら
        if (_mainJumpPower <= 0)
        {
            //ジャンプの初期化
            UseGravity();

            if (_isGroundTouch == true)
            {

                _isjump = false;
                _mainJumpPower = choropuParameter._jumpPower;

            }

        }

    }

    //アニメーションを遅れさせる。
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
    /// 追いかけまわる　
    /// </summary>
    private void MoveAround()
    {

        //自身よりプレイヤーが左にいたら左に移動
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

        //自身よりプレイヤーが右にいたら右に移動
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