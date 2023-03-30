using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyKuriboChase : SmallEnemyManager
{
    private EnemyKuriboChaseParameter kuribochaseParameter;
    private CharacterCommonParameter charaCommonParameter;

    [field: SerializeField, Header("移動スピード")]
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

        //自分ポジション
        _myPos = gameObject.transform.position;

        //自分の向き
        _myDirection = gameObject.transform.right;

        //プレイヤーのポジション
        _playerPos = _playerObj.transform.position;

        //プレイヤーへの向き
        _targetDirection = _playerPos - _myPos;

        //プレイヤーとの距離
        _targetDistance = _targetDirection.magnitude;

        _cosHalf = Mathf.Cos(kuribochaseParameter._myangle * Mathf.Deg2Rad);


        //視野範囲中の面積の設定
        _innerProduct = Vector2.Dot(_myDirection, _targetDirection.normalized);

        //プレイヤーとの距離を取得
        float _objdistance = Mathf.Abs(_playerObj.transform.position.x - transform.position.x);

        //距離が遠い場合
        if (_objdistance >= kuribochaseParameter._kuribodistance)
        {
            //アニメーション
            _isAnime.SetBool("Walking", true);
            _isAnime.SetBool("Attack2", false);


            //右を向いているときに視界に入った場合
            if (_innerProduct > _cosHalf && _targetDistance < kuribochaseParameter._maxrange
            && Mathf.Sign(_moveSpeed) == 1)
            {
                _moveSpeed = Mathf.Abs(_moveSpeed);
                transform.position = Vector3.MoveTowards(transform.position, _playerObj.transform.position, _moveSpeed * Time.deltaTime);
                _defaultPos = gameObject.transform.position.x;

            }

            //左を向いているときに視界に入った場合
            else if (_innerProduct < -_cosHalf && _targetDistance < kuribochaseParameter._maxrange
                    && Mathf.Sign(_moveSpeed) == -1)
            {
                transform.position = Vector3.MoveTowards(transform.position, _playerObj.transform.position, Mathf.Abs(_moveSpeed) * Time.deltaTime);
                _defaultPos = gameObject.transform.position.x;

            }

            if (_kuriboChaseNowHp == _beforeHp)
            {
                //左右移動
                LeftRight();
            }
            else
            {
                StanTime();
            }



            UseGravity();
        }

        //視界に入っていて、isattackがfalseの場合
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
    /// 左右移動
    /// </summary>
    private void LeftRight()
    {
        //アニメーション
        _isAnime.SetBool("Walking", true);
        _isAnime.SetBool("Attack2", false);


        // 地面に接触しているときのみ
        if (_isGroundTouch == true)
        {


            //自分のポジションが移動範囲以上になっていたら
            if (this.gameObject.transform.position.x > _defaultPos + kuribochaseParameter._moverange)
            {
                print("右超えた");
                _moveLeft = true;
                _moveRight = false;

                //敵が移動範囲を超えたら、戻す
                transform.position = new Vector2(_defaultPos + kuribochaseParameter._moverange, transform.position.y);
                TurnAngle();
            }

            else if (this.gameObject.transform.position.x < _defaultPos - kuribochaseParameter._moverange)
            {
                print("左超えた");
                _moveRight = true;
                _moveLeft = false;

                //敵が移動範囲を超えたら、戻す
                transform.position = new Vector2(_defaultPos - kuribochaseParameter._moverange, transform.position.y);
                TurnAngle();
            }

            //壁に触れていたら、
            if (_isRightWallTouch == true || _isLeftWallTouch == true)
            {
                TurnAngle();

                _isLeftWallTouch = false;
                _isRightWallTouch = false;

            }

            //敵の移動
            this.gameObject.transform.Translate(Vector2.right * _moveSpeed * Time.deltaTime);

        }
    }
    /// <summary>
    /// 向いている方向を変えるメソッド
    /// </summary>
    /// <returns></returns>
    private Quaternion TurnAngle()
    {

        //進行方向の変更
        _moveSpeed = -(_moveSpeed);
        if (_moveRight == true || _isLeftWallTouch == true)
        {
            //敵の向き回転
            transform.rotation = Quaternion.Euler(0f, -180f, 0f);
        }
        else if (_moveLeft == true || _isRightWallTouch == true)
        {
            //敵の向き回転
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        return this.transform.rotation;
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private void Attack()
    {
        //近い場合攻撃
        if (_isAttack == false)
        {
            //アニメーション
            _isAnime.SetBool("Walking", false);
            _isAnime.SetBool("Attack2", true);
            _isAttack = true;
            PlayerBoxCollision();

            //攻撃判定
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