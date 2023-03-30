using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bossmove : SmallEnemyManager
{
    private EnemyBossParameter bossParameter;

    [SerializeField]
    private GameObject _bossmesh;

    [SerializeField, Header("剣で攻撃する距離")]
    private float _attackdistance = 10;

    [SerializeField, Header("高さ制限")]
    private float _heightrange = 30;

    [SerializeField, Header("ステージクリアアニメーション")]
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
        //オブジェクトの距離
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

        //死んだときの判定
        if (_isDeath == false)
        {

            PlayerBoxCollision();
            RightWallRay();
            LeftWallRay();
            //攻撃をしていないとき向きを変える
            if(_isSwordAttack == false && _isjumpAttack == false)
            {

                TurnObject();

            }
            //オブジェクトの距離
            _objdistance = Mathf.Abs(_playerObj.transform.localPosition.x - transform.localPosition.x);

            #region 攻撃処理

            //距離が離れていて、ジャンプ攻撃していないとき
            if (_objdistance < _attackdistance && _isjumpAttack == false)
            {
                print("剣攻撃");
                _animator.SetBool("Attack", true);
                _animator.SetBool("Jump", false);
                _isSwordAttack = true;
            }

            #endregion

            #region ジャンプ処理
            //一定距離離れている場合
            if (_objdistance >= bossParameter._jumpstartrange && _isjumpAttack == false )
            {
                print("じゃんぷするよ");
                _isSwordAttack = false;
                _animator.SetBool("Attack", false);
                _animator.SetBool("Jump", true);
                _isjumpAttack = true;
                //プレイヤーのポジション取得　ジャンプまでの目的地
                _playerpos = _playerObj.transform.position;
                _jumpStartPos = transform.position.y;
            }

            //ジャンプ攻撃開始
            if (_isjumpAttack == true)
            {
                _jumpAnimationDeray += Time.fixedDeltaTime;
                //ジャンプを始める時間まで待機させる
                if (_jumpAnimationDeray > bossParameter._jumpStartTime)
                {
                    //ジャンプする
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
    /// ジャンプ処理
    /// </summary>
    private void Jump()
    {
        //高さ制限
        if (transform.position.y < _heightrange)
        {
            transform.Translate(Vector2.up * bossParameter._speed * Time.deltaTime);
        }
        else
        {
            //制限した高さ以上に行かないようにする
            transform.position = new Vector2(transform.position.x, _heightrange);
        }

        //プレイヤーのいる位置まで行ってない場合
        if (transform.position.x > _playerpos.x)
        {
            //右上に移動
            transform.Translate(Vector2.left * bossParameter._speed * Time.deltaTime);
        }
        else if (transform.position.x < _playerpos.x)
        {
            //左上に移動
            transform.Translate(Vector2.right * bossParameter._speed * Time.deltaTime);
        }
    

        //プレイヤーとボスのx軸が同じになったら
        if (Mathf.Round(transform.position.x) == Mathf.Round(_playerpos.x))
        {
            _animator.SetBool("Jump", false);
            _animator.SetBool("Attack", true);
            //落下攻撃開始
            DownAttack();
          
            transform.position = new Vector2(_playerpos.x, transform.position.y);
        }

    }

    /// <summary>
    /// スタンプ攻撃
    /// </summary>
    private void DownAttack()
    {
        if (Mathf.Round(transform.position.y) > _jumpStartPos && _isDownAttack == false)
        {
            //下に落っこちていく
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
    /// ジャンプのディレイ時間
    /// </summary>
    private void ActionDeray()
    {
        _animator.SetBool("Jump", false);
        _animator.SetBool("Attack", false);

        //ディレイ時間
        _jumpAnimationDeray += Time.fixedDeltaTime;
        //アクションし終わった時間経過で再度アクション可能にする
        if (_jumpAnimationDeray >= bossParameter.jumpderaytime)
        {
            print("全部リセットするよん");
            _isDownAttack = false;
            _isjumpAttack = false;
            _jumpAnimationDeray = 0;
        }
    }

    /// <summary>
    /// 向いてる方向変更
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
    /// 死んでいるかの判定
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
