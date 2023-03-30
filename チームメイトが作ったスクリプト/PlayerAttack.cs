using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerMotionManager
{
    //SE
    [SerializeField, Header("SE")]
    private GameObject _SE;
    //EffectPoint
    [SerializeField, Header("SE")]
    private GameObject _effectPoint;

    //陸上での攻撃判定のθ角
    [SerializeField, Header("陸上攻撃判定角度(°)")]
    private float _groundAttackAngle;

    //空中での攻撃判定のθ角
    [SerializeField, Header("空中攻撃判定角度(°)")]
    private float _airAttackAngle;

    //陸上での攻撃の長さ
    [SerializeField, Header("陸上攻撃判定長(m)")]
    private float _groundAttackRange;

    //空中での攻撃の長さ
    [SerializeField, Header("空中攻撃判定長(m)")]
    private float _airAttackRange;

    //通常時に攻撃開始時から実際に攻撃判定が出るまでの時間
    [SerializeField, Header("通常時攻撃開始から判定が出るまでの時間(s)")]
    private float _defaultAttackStandByTime;

    //通常時に攻撃判定が出てアイドルに戻るまでの時間
    [SerializeField, Header("通常時攻撃判定が出る時間(s)")]
    private float _defaultAttackActionTime;

    //オイル差し時に攻撃開始時から実際に攻撃判定が出るまでの時間
    [SerializeField, Header("オイル差し時攻撃開始から判定が出るまでの時間(s)")]
    private float _insertOilAttackStandByTime;

    //オイル差し時に攻撃判定が出てアイドルに戻るまでの時間
    [SerializeField, Header("オイル差し時攻撃判定が出る時間(s)")]
    private float _insertOilAttackActionTime;

    //攻撃開始時フラグ
    private bool _isAttackStart = true;

    //現在の攻撃開始からの時間取得用
    private float _nowAttackTime = 0;

    //攻撃開始時から実際に攻撃判定が出るまでの時間格納用
    private float _attackStandByTime = default;

    //攻撃判定が出てアイドルに戻るまでの時間格納用
    private float _attackActionTime = default;

    //プレイヤーのコライダーの大きさ格納用
    private Vector2 _playerColliderSize;

    //プレイヤーの右向きフラグ
    private bool _isPlayerDirectionRight = default;

    //既に攻撃した敵格納用
    private GameObject[] _hitEnemys = default;

    //コンボスクリプト格納用
    private PlayerCombo _playerCombo;



    private void Start()
    {
        //プレイヤーのコライダー取得用
        BoxCollider2D playerCollider;


        //プレイヤーのコライダー取得
        playerCollider = GetComponent<BoxCollider2D>();

        //プレイヤーのコライダーの大きさ格納
        _playerColliderSize = transform.lossyScale * playerCollider.size;

        //コンボスクリプト格納
        _playerCombo = GetComponent<PlayerCombo>();
    }



    public void Attack(bool isRight, bool isInsertOil, bool isAir)
    {
        //攻撃開始時か
        if (_isAttackStart)
        {
            //現在の攻撃開始からの時間初期化
            _nowAttackTime = 0;

            //既に攻撃した敵格納用変数初期化
            _hitEnemys = new GameObject[0];

            //プレイヤーの向き格納
            _isPlayerDirectionRight = isRight;

            //油差し状態か
            if (isInsertOil)
            {
                //油さし時に攻撃開始時から実際に攻撃判定が出るまでの時間格納
                _attackStandByTime = _insertOilAttackStandByTime;

                //油さし時に攻撃判定が出てアイドルに戻るまでの時間格納
                _attackActionTime = _insertOilAttackActionTime;
            }
            else
            {
                //通常時に攻撃開始時から実際に攻撃判定が出るまでの時間格納
                _attackStandByTime = _defaultAttackStandByTime;

                //通常時に攻撃判定が出てアイドルに戻るまでの時間格納
                _attackActionTime = _defaultAttackActionTime;
            }

            //攻撃開始時フラグfalse
            _isAttackStart = false;
        }

        //現在の攻撃開始からの時間更新
        _nowAttackTime += GetFixedDeltaTime();

        //実際に攻撃判定を出すか
        if (_nowAttackTime >= _attackStandByTime)
        {
            /*攻撃処理*/

            //攻撃範囲に入ったオブジェクト取得用
            RaycastHit2D[] rayHits;

            //攻撃範囲原点取得用
            Vector2 attackOrigin;

            //攻撃長取得用
            float attackRange;

            //攻撃判定のθ角取得用
            float attackAngle;

            //攻撃判定の角の軸取得用
            float attackShaft;


            //空中にいるか
            if (isAir)
            {
                //空中での攻撃長と攻撃判定のθ角、角の軸取得
                attackRange = _airAttackRange;
                attackAngle = _airAttackAngle;
                attackShaft = 270;
            }
            else
            {
                //陸上での攻撃長と攻撃判定のθ角取得
                attackRange = _groundAttackRange;
                attackAngle = _groundAttackAngle;

                //プレイヤーが右向きか
                if (_isPlayerDirectionRight)
                {
                    //攻撃判定の角の軸を右に
                    attackShaft = 0;
                }
                else
                {
                    //攻撃判定の角の軸を左に
                    attackShaft = 180;
                }
            }

            //攻撃範囲原点取得
            attackOrigin = new Vector2(transform.position.x, transform.position.y - _playerColliderSize.y / 2);

            //攻撃範囲に入ったオブジェクト取得
            rayHits = Physics2D.CircleCastAll(attackOrigin, attackRange, Vector2.zero, 0);

            //攻撃範囲に入ったオブジェクト探索
            foreach (RaycastHit2D rayHit in rayHits)
            {
                //攻撃接触フラグ
                bool isHit = false;


                //接触したのが敵か
                if (rayHit.collider.gameObject.tag == "Enemy")
                {
                    //攻撃接触フラグ取得
                    isHit = GetHitEnemyFlag(rayHit.collider.gameObject, _isPlayerDirectionRight,
                                                attackOrigin, attackAngle, attackShaft);
                }

                //攻撃接触したか
                if (isHit)
                {
                    /*敵被攻撃処理(敵→rayHit.collider.gameObject)*/

                    //接触した敵取得用
                    GameObject enemy;

                    //接触した敵のEnemyManager取得用
                    EnemyManager enemyManager;

                    //攻撃力取得用
                    int attackPoint;

                    //オイルドロップ数取得用
                    int dropOilAmount;


                    //接触した敵取得
                    enemy = rayHit.collider.gameObject;

                    //接触した敵のCharacterManager取得
                    enemyManager = enemy.GetComponent<EnemyManager>();

                    //コンボ更新
                    _playerCombo.Combo();

                    //攻撃力取得
                    attackPoint = _playerCombo.GetAttackPoint();

                    //オイルドロップ数取得
                    dropOilAmount = _playerCombo.GetDropOilAmount();

                    //ガードされたか
                    if (enemyManager.PlayerAttackEnemy())
                    {
                        //はじかれて攻撃中止
                        _playerControler.DefaultMotion();

                        _SE.GetComponent<SE_Controller>().PlaySE(_effectPoint.transform.position, "ガード");
                    }
                    else
                    {
                        //敵のCharacterManager取得用
                        CharacterManager characterManager;

                        switch(Random.Range(1, 3)){
                            case 1:
                                _SE.GetComponent<SE_Controller>().PlaySE(_effectPoint.transform.position, "攻撃A");
                                break;
                            case 2:
                                _SE.GetComponent<SE_Controller>().PlaySE(_effectPoint.transform.position, "攻撃B");
                                break;
                            case 3:
                                _SE.GetComponent<SE_Controller>().PlaySE(_effectPoint.transform.position, "攻撃C");
                                break;
                        }

                        //敵のCharacterManager取得
                        characterManager = enemy.GetComponent<CharacterManager>();
                        //ダメージ判定
                        characterManager.TakesDamage(attackPoint);
                    }

                    //攻撃した敵を格納
                    _hitEnemys = new GameObject[_hitEnemys.Length + 1];
                    _hitEnemys[_hitEnemys.Length - 1] = enemy;
                }
            }
        }
    }



    /// <summary>
    /// 攻撃終了メソッド
    /// </summary>
    public void AttackEnd()
    {
        //攻撃開始時フラグ初期化
        _isAttackStart = true;
    }



    /// <summary>
    /// 攻撃時間を返すメソッド
    /// </summary>
    /// <returns>攻撃時間</returns>
    public float GetAttackTime()
    {
        return _attackActionTime + _attackStandByTime;
    }



    /// <summary>
    /// 敵接触フラグ取得メソッド
    /// </summary>
    /// <param name="enemy">敵オブジェクト</param>
    /// <param name="isRight">プレイヤー右向きフラグ</param>
    /// <param name="attackOrigin">攻撃範囲原点</param>
    /// <param name="attackAngle">攻撃判定軸角度</param>
    /// <param name="attackShaft">攻撃範囲角度</param>
    /// <returns>敵接触フラグ</returns>
    private bool GetHitEnemyFlag(GameObject enemy, bool isRight, Vector2 attackOrigin,
                                    float attackAngle, float attackShaft)
    {
        //敵接触フラグ
        bool isHit = true;


        //接触済みの敵探索
        foreach(GameObject hitEnemy in _hitEnemys)
        {
            if (hitEnemy == enemy)
            {
                //敵接触フラグfalse
                isHit = false;

                break;
            }
        }

        //対象の敵が接触済みの敵リストにいなかったか
        if (isHit)
        {
            //敵の角度取得用
            float enemyAngle;

            //範囲最低角度取得用
            float minAngle;

            //範囲最大角度取得用
            float maxAngle;

            //角度オーバーフラグ
            bool isOverAngle = false;


            //敵の角度取得
            enemyAngle = GetTwoPointAngle(attackOrigin, enemy.transform.position);

            //プレイヤーが右向きか
            if (isRight)
            {
                //範囲最低角度取得
                minAngle = attackShaft;

                //範囲最大角度取得
                maxAngle = attackShaft + attackAngle;

                //最大角度が１周を超えたか
                if (maxAngle >= 360)
                {
                    //角度を0～360度に変換
                    maxAngle -= 360;

                    //角度オーバーフラグtrue
                    isOverAngle = true;
                }
            }
            else
            {
                //範囲最低角度取得
                minAngle = attackShaft - attackAngle;

                //範囲最大角度取得
                maxAngle = attackShaft;

                //最低角度が0度未満か
                if (minAngle < 0)
                {
                    //角度を0～360度に変換
                    maxAngle += 360;

                    //角度オーバーフラグtrue
                    isOverAngle = true;
                }
            }

            //最大角度が１周を超えたか
            if (isOverAngle)
            {
                //敵が攻撃範囲外か
                if(!(enemyAngle >= minAngle || enemyAngle <= maxAngle))
                {
                    //敵接触フラグfalse
                    isHit = false;
                }
            }
            else
            {
                //敵が攻撃範囲外か
                if (!(enemyAngle >= minAngle && enemyAngle <= maxAngle))
                {
                    //敵接触フラグfalse
                    isHit = false;
                }
            }
        }

        return isHit;
    }



    /// <summary>
    /// ２点間の角度を取得するメソッド
    /// </summary>
    /// <param name="startPosition">開始座標</param>
    /// <param name="targetPosition">目標座標</param>
    /// <returns>２点間の角度</returns>
    private float GetTwoPointAngle(Vector2 startPosition, Vector2 targetPosition)
    {
        //ターゲットまでのベクトル取得用
        Vector2 targetVector;

        //ターゲットまでの角度取得用
        float angle;


        //ターゲットまでのベクトル取得
        targetVector = targetPosition - startPosition;

        //ターゲットまでの角度(ラジアン)取得
        angle = Mathf.Atan2(targetVector.y, targetVector.x);

        //ラジアンを度に変換
        angle *= Mathf.Rad2Deg;

        //角度が0度未満か
        if (angle < 0)
        {
            //角度を0～360度に変換
            angle += 360;
        }

        return angle;
    }
}
