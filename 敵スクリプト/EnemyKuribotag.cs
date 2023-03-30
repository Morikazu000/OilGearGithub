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

        // 現在のHP取得
        _kuriboNowHp = GetNowHp();

        // 重力処理
        UseGravity();

        //今の体力とその前の体力が同じであるなら移動できる
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

        //HPが０以下になったら死ぬ
        if (_kuriboNowHp < 0)
        {
            gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// 左右移動
    /// </summary>
    private void LeftRight()
    {
        // 地面に接触しているときのみ
        if (_isGroundTouch == true)
        {

            //アニメーション
            _isanime.SetBool("Walking", true);
            _isanime.SetBool("Attack2", false);

            //自分のポジションが移動範囲以上になっていたら
            if (this.gameObject.transform.position.x > pos.x + kuriboParameter._moverange)
            {

                _moveLeft  = true;
                _moveRight = false;
                //敵が移動範囲を超えたら、戻す
                transform.position = new Vector2(pos.x + kuriboParameter._moverange, transform.position.y);
                TurnAngle();
            }

            else if (this.gameObject.transform.position.x < pos.x - kuriboParameter._moverange)
            {

                _moveRight = true;
                _moveLeft = false;

                //敵が移動範囲を超えたら、戻す
                transform.position = new Vector2(pos.x - kuriboParameter._moverange, transform.position.y);
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
    private Quaternion TurnAngle()
    {

        //進行方向の変更
        _moveSpeed = -(_moveSpeed);
        if (_moveRight == true || _isLeftWallTouch == true)
        {
            //敵の向き回転
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (_moveLeft == true  || _isRightWallTouch == true)
        {
            //敵の向き回転
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        return this.transform.rotation;
    }

    /// <summary>
    /// 止まって攻撃するクリボー
    /// </summary>
    private void KuriboAttackmove()
    {
        if (_isGroundTouch == true)
        {
            //プレイヤーとの距離を取得
            float _objdistance = Mathf.Abs(_playerObj.transform.position.x - transform.position.x);
            //距離が遠い場合
            if (_objdistance >= kuriboParameter._kuribo_distance)
            {
                //左右移動
                LeftRight();
            }
            //近い場合攻撃
            else if (_isAttack == false)
            {

                //アニメーション
                _isanime.SetBool("Walking", false);
                _isanime.SetBool("Attack2", true);
                //攻撃判定
                Invoke("Attackderay", kuriboParameter._attackderay);
                _isAttack = true;


            }
        }
    }

    /// <summary>
    /// 攻撃の遅延
    /// </summary>
    private bool Attackderay()
    {
        return _isAttack = false;
    }

    /// <summary>
    /// スタンする時間
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
