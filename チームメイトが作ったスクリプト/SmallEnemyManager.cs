using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/// --------------------------------------------------------- 
/// #SmallEnemyManager.cs#
/// 
/// Base.Start()を必ず継承先のStartメソッドで実行してください
/// 
/// プレイヤーとの当たり判定を使用する場合は、
/// PlayerBoxCollisionStart()をStartメソッドで実行し
/// PlayerBoxCollision()をFixedUpdateで実行してください
/// ---------------------------------------------------------

public class SmallEnemyManager : EnemyManager
{
    // SmallEnemyManagerに入っている敵のパラメータ
    private EnemyCommonParameter _enemyParameter;

    // プレイヤーと敵(自キャラ)のコライダーの中心に対する頂点の座標
    private Vector2 _playerLocalTopPosMax = default;
    private Vector2 _playerLocalTopPosMin = default;
    private Vector2 _myLocalTopPosMax = default;
    private Vector2 _myLocalTopPosMin = default;

    // 落ち始めてからの時間を計測する変数
    private float _fallTime = default;

    // プレイヤーオブジェクト
    protected GameObject _playerObj;


    /// <summary>
    /// Scene開始時に取得すべき変数を取得するメソッド
    /// </summary>
    protected void Start()
    {
        //EnemyCommonParameterを取得
        GetSmallEnemyParameter();

        //プレイヤーのオブジェクトを取得
        _playerObj = GameObject.FindWithTag("Player");

        //プレイヤーのCharacterManagerを取得
        _playerCharacterManager = _playerObj.GetComponent<CharacterManager>();
        
        // 自キャラのコライダーの大きさを取得する
        CollisionStart();
    }


    /// <summary>
    /// 落下させるメソッド(FixedUpdateに記述)
    /// </summary>
    protected void UseGravity()
    {
        // 地面に触れたとき接地する処理
        GroundRay();

        // 地面に触れていないときの処理
        if (!_isGroundTouch)
        {
            // 落ちている時間を加算する
            _fallTime += Time.fixedDeltaTime;

            // 落下距離を求める
            float nowFallDistance = _enemyParameter._fallGravity * _fallTime;

            // 落下速度が最大値を超えていたら最大値にする
            if (nowFallDistance > _enemyParameter._maxFallSpeed)
            {
                nowFallDistance = _enemyParameter._maxFallSpeed;
            }
            // 実際に落下させる処理
            this.transform.Translate(new Vector3(0, -nowFallDistance * Time.deltaTime, 0));
        }

        // 地面に触れているときの処理
        else
        {
            // 落下後地面についたとき、落下時間を初期化する
            if (_fallTime != 0)
            {
                _fallTime = 0;
            }
        }
    }


    #region 敵とプレイヤーとの直接的な衝突判定に関するもの

    /// <summary>
    /// 敵(自キャラ)とプレイヤーの直接的な衝突判定をとるときの初期計算
    /// </summary>
    protected void EnemyCollisionStart()
    {
        BoxCollider2D playerCollider = _playerObj.GetComponent<BoxCollider2D>();

        // プレイヤーのコライダーのサイズと中心からのずれを取得する
        Vector2 playerHalfSize = _playerObj.transform.localScale * playerCollider.size / 2;
        Vector2 playerOffset = _playerObj.transform.localScale * playerCollider.offset;

        // プレイヤーと敵(自キャラ)のコライダーの中心に対する頂点の座標を求める
        _playerLocalTopPosMax = new Vector2(playerOffset.x + playerHalfSize.x, playerOffset.y + playerHalfSize.y);
        _playerLocalTopPosMin = new Vector2(playerOffset.x - playerHalfSize.x, playerOffset.y - playerHalfSize.y);
        _myLocalTopPosMax = new Vector2(_myColliderOffset.x + _myHalfColliderSize.x, _myColliderOffset.y + _myHalfColliderSize.y);
        _myLocalTopPosMin = new Vector2(_myColliderOffset.x - _myHalfColliderSize.x, _myColliderOffset.y - _myHalfColliderSize.y);

        // 自キャラのコライダーの大きさを取得する
        CollisionStart();
    }

    /// <summary>
    /// 敵とプレイヤーの当たり判定をとるときにfixedUpdateで使うメソッド
    /// </summary>
    protected void PlayerBoxCollision()
    {
        // プレイヤーのColliderの頂点の座標を求める
        Vector2 playerColliderPosMax = new Vector2(_playerObj.transform.position.x + _playerLocalTopPosMax.x, _playerObj.transform.position.y + _playerLocalTopPosMax.y);
        Vector2 playerColliderPosMin = new Vector2(_playerObj.transform.position.x + _playerLocalTopPosMin.x, _playerObj.transform.position.y + _playerLocalTopPosMin.y);

        // 自オブジェクトのColliderの頂点の座標を求める
        Vector2 myColliderPosMax = new Vector2(this.transform.position.x + _myLocalTopPosMax.x, this.transform.position.y + _myLocalTopPosMax.y);
        Vector2 myColliderPosMin = new Vector2(this.transform.position.x + _myLocalTopPosMin.x, this.transform.position.y + _myLocalTopPosMin.y);

        // X軸の判定を行う
        if (playerColliderPosMax.x <= myColliderPosMax.x && playerColliderPosMax.x >= myColliderPosMin.x) { }
        else if (playerColliderPosMin.x <= myColliderPosMax.x && playerColliderPosMin.x >= myColliderPosMin.x) { }
        else if (playerColliderPosMax.x >= myColliderPosMax.x && playerColliderPosMin.x <= myColliderPosMin.x) { }

        // X軸に当たっていないとき処理をやめる
        else { return; }

        // Y軸の判定を行う
        if (playerColliderPosMax.y <= myColliderPosMax.y && playerColliderPosMax.y >= myColliderPosMin.y) { }
        else if (playerColliderPosMin.y <= myColliderPosMax.y && playerColliderPosMin.y >= myColliderPosMin.y) { }
        else if (playerColliderPosMax.y >= myColliderPosMax.y && playerColliderPosMin.y <= myColliderPosMin.y) { }

        // X軸に当たっていないとき処理をやめる
        else { return; }

        // 当たり判定が重なっているときダメージを与える
        _playerCharacterManager.TakesDamage(_enemyParameter._collisionDamage);

    }

    #endregion


    /// <summary>
    /// EnemyCommonParameterを取得するメソッド
    /// </summary>
    protected void GetSmallEnemyParameter()
    {
        //EnemyCommonParameterを取得
        _enemyParameter = Resources.Load<EnemyCommonParameter>(characterCommonParameter._myParameterObjName);

        //ロード出来なかった場合はエラーログを表示
        if (_enemyParameter == null)
        {
            Debug.LogError(characterCommonParameter._myParameterObjName + " Not Found　アクセスエラーです");
        }
    }

}
