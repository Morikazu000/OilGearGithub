using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlink : PlayerMotionManager
{
    //ブリンクで移動できる最大距離
    [SerializeField, Header("ブリンク最大移動距離(m)")]
    private float _blinkMaxRange;

    //ブリンクで高速で移動する距離
    [SerializeField, Header("ブリンク高速移動距離(m)")]
    private float _blinkHighSpeedRange;

    //ブリンクで最大距離までの移動にかかる時間
    [SerializeField, Header("ブリンク移動時間(s)")]
    private float _blinkMaxTime;

    //ブリンクで高速で移動する時間
    [SerializeField, Header("ブリンク高速移動時間(s)")]
    private float _blinkHighSpeedTime;

    //ブリンクで消費するHP(たりなかったらブリンク不発)
    [SerializeField, Header("ブリンクで使用するHP")]
    private int _blinkUseHp;

    //ブリンクで壁や床、天井に接触した場合に猶予を持つために空ける隙間
    [SerializeField, Header("ブリンクで壁に当たった場合に空ける隙間(m)")]
    private float _collisionWallVacateSpace;

    //高速移動フラグ
    private bool _isHighSpeed = true;

    //現在のブリンク移動距離
    private float _blinkNowRange = 0;

    //現在のブリンク移動時間
    private float _blinkNowTime = 0;

    //ブリンク開始時フラグ
    private bool _isBlinkStart = true;

    //ブリンクの向きベクトル格納用
    private Vector2 _blinkDirectionVector;

    //プレイヤーのコライダーの大きさ格納用
    private Vector2 _playerColliderSize;

    //オイルスクリプト格納用
    private PlayerOil _playerOil;



    private void Start()
    {
        //プレイヤーのコライダー取得用
        BoxCollider2D playerCollider;


        //プレイヤーのコライダー取得
        playerCollider = GetComponent<BoxCollider2D>();

        //プレイヤーのコライダーの大きさ格納
        _playerColliderSize.x = transform.lossyScale.x * playerCollider.size.x;
        _playerColliderSize.y = transform.lossyScale.y * playerCollider.size.y;

        //オイルスクリプト格納
        _playerOil = GetComponent<PlayerOil>();
    }



    /// <summary>
    /// ブリンクしつつブリンクの移動ベクトルを返すメソッド
    /// </summary>
    /// <param name="stickInput">スティック入力値</param>
    /// <param name="isPlayerDirectionRight">右向きフラグ</param>
    /// <returns>ブリンクの移動ベクトル</returns>
    public Vector2 Blink(Vector2 stickInput, bool isPlayerDirectionRight)
    {
        //ブリンク開始時か
        if (_isBlinkStart)
        {
            //HP消費
            _playerOil.UseOil(_blinkUseHp, false);
            
            //ブリンクの向きベクトル取得
            _blinkDirectionVector = InputConversion(stickInput, isPlayerDirectionRight);

            //現在のブリンク移動距離、時間初期化
            _blinkNowRange = 0;
            _blinkNowTime = 0;

            //高速移動フラグtrue
            _isHighSpeed = true;

            //ブリンク開始時フラグfalse
            _isBlinkStart = false;
        }

        /*----------------------*/
        //一次関数
        //y=ax+b
        //
        //三平方の定理
        //a^2+b^2=c^2
        /*----------------------*/

        //計算用
        float tilt;

        //最新のブリンク移動距離取得用
        float blinkRange;

        //ブリンク終了フラグ
        bool isBlinkEnd = false;

        //レイに接触したオブジェクト取得用
        RaycastHit2D[] rayHits;

        //接触中の床取得用
        GameObject touchGround = default;

        //ブリンクの移動分座標取得用
        Vector3 blinkPosition = Vector3.zero;

        //ブリンクのベクトルX、Yの割合取得用
        float blinkRatio;


        //現在のブリンク時間更新
        _blinkNowTime += GetFixedDeltaTime();

        //高速移動中か
        if (_isHighSpeed)
        {
            //高速移動時の一次関数の傾き取得
            tilt = _blinkHighSpeedRange / _blinkHighSpeedTime;

            //最新のブリンク移動距離取得
            blinkRange = tilt * _blinkNowTime;

            //高速移動距離以上に進んだか
            if (blinkRange >= _blinkHighSpeedRange)
            {
                //高速移動フラグfalse
                _isHighSpeed = false;
            }
        }
        else
        {
            //通常移動時の一次関数の傾き取得
            tilt = (_blinkMaxRange - _blinkHighSpeedRange) / _blinkMaxTime;

            //最新のブリンク移動距離取得
            blinkRange = tilt * _blinkNowTime + _blinkHighSpeedRange;

            //ブリンクの最大距離以上に進んだか
            if (blinkRange >= _blinkMaxRange)
            {
                //ブリンク終了フラグtrue
                isBlinkEnd = true;
            }
        }

        //三平方からブリンクのベクトルX、Yの割合取得
        blinkRatio = Mathf.Pow(blinkRange - _blinkNowRange, 2) 
                        / (Mathf.Abs(_blinkDirectionVector.x) + Mathf.Abs(_blinkDirectionVector.y));
        
        //ブリンクの移動分座標取得
        blinkPosition.x = Mathf.Sqrt(blinkRatio * Mathf.Abs(_blinkDirectionVector.x)) * Mathf.Sign(_blinkDirectionVector.x);
        blinkPosition.y = Mathf.Sqrt(blinkRatio * Mathf.Abs(_blinkDirectionVector.y)) * Mathf.Sign(_blinkDirectionVector.y);

        //接地しているか
        if (_characterManager.GetIsCollision(CharacterManager.COLLISION.GROUND))
        {
            //床取得
            touchGround = _characterManager.GroundRay();
        }

        //ブリンクの移動方向にレイを飛ばす
        rayHits = Physics2D.BoxCastAll(transform.position, _playerColliderSize, 0,
                                        _blinkDirectionVector, blinkRange - _blinkNowRange);

        rayHits = Physics2D.RaycastAll(transform.position, _blinkDirectionVector, blinkRange - _blinkNowRange);

        //レイで取得したオブジェクト探索
        foreach (RaycastHit2D rayHit in rayHits)
        {
            //壁、地面に接触し、かつ接地していたら接触したのが足元の床ではないか
            if (rayHit.collider.gameObject.tag == "Ground")
            {
                //接触した端取得用
                COLLIDER_END hitColliderEnd;

                //接触したコライダー取得用
                BoxCollider2D hitCollider;

                //接触したオブジェクトの座標取得用
                Vector2 hitObjPos;

                //接触したコライダーの大きさ取得用
                Vector2 hitColliderSize;


                //接触したコライダー取得
                hitCollider = rayHit.collider.gameObject.GetComponent<BoxCollider2D>();

                //接触したオブジェクトの座標取得
                hitObjPos = new Vector2(rayHit.collider.gameObject.transform.position.x,
                                            rayHit.collider.gameObject.transform.position.y);

                //接触したコライダーの大きさ取得
                hitColliderSize = new Vector2(rayHit.collider.gameObject.transform.lossyScale.x,
                                                rayHit.collider.gameObject.transform.lossyScale.y)
                                                    * hitCollider.size;

                //接触した端取得
                hitColliderEnd = GetColliderEnd(rayHit.point, hitColliderSize, hitObjPos);

                //目標座標更新
                blinkPosition = GetCollisionedBlinkRange(hitColliderEnd, _blinkDirectionVector, rayHit.point, 
                                                            _playerColliderSize, blinkPosition, transform.position);

                

                //ブリンク終了フラグtrue
                isBlinkEnd = true;

                break;
            }
        }

        //座標更新
        transform.position += blinkPosition;

        //移動距離更新
        _blinkNowRange = blinkRange;

        //ブリンク終了するか
        if (isBlinkEnd)
        {
            //ブリンク開始フラグtrue
            _isBlinkStart = true;

            //通常状態へ
            _playerControler.DefaultMotion();

            //接地していないか
            if (!_characterManager.GetIsCollision(CharacterManager.COLLISION.GROUND))
            {
                //落下開始
                _playerControler.FallStart();
            }
        }

        return _blinkDirectionVector;
    }



    /// <summary>
    /// ブリンク使用可能フラグを返すメソッド
    /// </summary>
    /// <returns>ブリンク使用可能フラグ</returns>
    public bool GetIsBlink()
    {
        //ブリンク使用可能フラグ
        bool isBlink = true;


        //ブリンクするのにHPが足りないか
        if (_characterManager.GetNowHp() - _blinkUseHp <= 0)
        {
            //ブリンク使用可能フラグ
            isBlink = false;
        }

        return isBlink;
    }



    /// <summary>
    /// 入力値変換メソッド
    /// </summary>
    /// <param name="stickInput">入力値</param>
    /// <param name="isPlayerDirectionRight">右向きフラグ</param>
    /// <returns>変換後入力値</returns>
    private Vector2 InputConversion(Vector2 stickInput, bool isPlayerDirectionRight)
    {
        //X入力がないか
        if (stickInput.x == 0)
        {
            //Y入力がないか
            if (stickInput.y == 0)
            {
                /*　入力なし　*/

                //プレイヤーが右向きか
                if (isPlayerDirectionRight)
                {
                    //右入力扱いにする
                    stickInput = Vector2.right;
                }
                else
                {
                    //左入力扱いにする
                    stickInput = Vector2.left;
                }
            }
            else
            {
                /*　Y入力あり　*/

                //Y入力を1か-1に変換する
                stickInput.y = Mathf.Sign(stickInput.y);
            }
        }
        else
        {
            //Y入力がないか
            if (stickInput.y == 0)
            {
                /*　X入力あり　*/

                //X入力を1か-1に変換する
                stickInput.x = Mathf.Sign(stickInput.x);
            }
            else
            {
                /*　X,Y入力あり　*/

                /*-------------------*/
                //一次関数方程式
                //y=ax
                //a=傾き
                /*-------------------*/

                //傾き取得用
                float tilt;


                //傾き取得
                tilt = stickInput.y / stickInput.x;

                //入力値のＸがＹより大きいか
                if (Mathf.Abs(stickInput.x) > Mathf.Abs(stickInput.y))
                {
                    //Ｙを１と仮定して入力値を調整
                    stickInput.y = Mathf.Sign(stickInput.y);
                    stickInput.x = stickInput.y / tilt;
                }
                else
                {
                    //Ｘを１と仮定して入力値を調整
                    stickInput.x = Mathf.Sign(stickInput.x);
                    stickInput.y = tilt * stickInput.x;
                }
            }
        }

        return stickInput;
    }



    /// <summary>
    /// 接触したポイントが上下左右どの端に一番近いかを返すメソッド
    /// </summary>
    /// <param name="hitPosition">接触したポイント座標</param>
    /// <param name="colliderSize">コライダーの大きさ</param>
    /// <param name="targetPosition">対象オブジェクト座標</param>
    /// <returns>一番近い端</returns>
    private COLLIDER_END GetColliderEnd(Vector2 hitPosition, Vector2 colliderSize,
                                            Vector2 targetPosition)
    {
        //コライダー上端との距離取得用
        float topRange;

        //コライダー右端との距離取得用
        float rightRange;

        //コライダー左端との距離取得用
        float leftRange;

        //コライダー下端との距離取得用
        float lowestRange;

        //一番近い端取得用
        float nearEnd;

        //接触したコライダー端取得用
        COLLIDER_END hitColliderEnd;


        //それぞれのコライダー端との距離取得
        topRange = Mathf.Abs(hitPosition.y - (targetPosition.y + colliderSize.y / 2));
        rightRange = Mathf.Abs(hitPosition.x - (targetPosition.x + colliderSize.x / 2));
        leftRange = Mathf.Abs(hitPosition.x - (targetPosition.x - colliderSize.x / 2));
        lowestRange = Mathf.Abs(hitPosition.y - (targetPosition.y - colliderSize.y / 2));

        //最初にまず上端を代入
        hitColliderEnd = COLLIDER_END.TOP;
        nearEnd = topRange;

        //現状一番近い端より右端のほうが近いか
        if (nearEnd > rightRange)
        {
            //右端代入
            hitColliderEnd = COLLIDER_END.RIGHT;
            nearEnd = rightRange;
        }

        //現状一番近い端より左端のほうが近いか
        if (nearEnd > leftRange)
        {
            //左端代入
            hitColliderEnd = COLLIDER_END.LEFT;
            nearEnd = leftRange;
        }

        //現状一番近い端より下端のほうが近いか
        if (nearEnd > lowestRange)
        {
            //下端代入
            hitColliderEnd = COLLIDER_END.LOWEST;
        }
        
        return hitColliderEnd;
    }



    /// <summary>
    /// 壁に当たった際にブリンク目標座標までの距離ベクトルを返すメソッド
    /// </summary>
    /// <param name="targetEnd">接触する壁の四つ端</param>
    /// <param name="blinkVector">ブリンクの移動ベクトル</param>
    /// <param name="rayHitPoint">レイの接触座標</param>
    /// <param name="playerColliderSize">プレイヤーのコライダーの大きさ</param>
    /// <param name="blinkPosition">ブリンク後の座標</param>
    /// <param name="nowPosition">現在座標</param>
    /// <returns>ブリンク目標座標までの距離ベクトル</returns>
    private Vector3 GetCollisionedBlinkRange(COLLIDER_END targetEnd, Vector2 blinkVector, Vector2 rayHitPoint,
                                        Vector2 playerColliderSize, Vector2 blinkPosition, Vector2 nowPosition)
    {
        //傾き取得用
        float tilt;

        //修正分の座標取得用
        Vector2 fixPosition;

        //ブリンク後のコライダーの角座標取得用
        float playerColliderEnd = default;


        //ブリンク後の座標取得
        blinkPosition += nowPosition;

        //入力値X,Yどちらかが0か
        if (blinkVector.x == 0 || blinkVector.y == 0)
        {
            //傾きなし
            tilt = 0;
        }
        else
        {
            //傾き取得
            tilt = (blinkPosition.y - nowPosition.y) / (blinkPosition.x - nowPosition.x);
        }

        //取得した端により分岐
        switch (targetEnd)
        {
            //左端
            case COLLIDER_END.LEFT:
                //ブリンク後のコライダーの角座標取得
                playerColliderEnd = rayHitPoint.x - playerColliderSize.x / 2 - _collisionWallVacateSpace;

                break;


            //右端
            case COLLIDER_END.RIGHT:
                //ブリンク後のコライダーの角座標取得
                playerColliderEnd = rayHitPoint.x + playerColliderSize.x / 2 + _collisionWallVacateSpace;

                break;


            //上端
            case COLLIDER_END.TOP:
                //ブリンク後のコライダーの角座標取得
                playerColliderEnd = rayHitPoint.y + playerColliderSize.y / 2 + _collisionWallVacateSpace;

                break;


            //下端
            case COLLIDER_END.LOWEST:
                //ブリンク後のコライダーの角座標取得
                playerColliderEnd = rayHitPoint.y - playerColliderSize.y / 2 - _collisionWallVacateSpace;

                break;
        }
        
        //取得した端が左右か
        if (targetEnd == COLLIDER_END.RIGHT || targetEnd == COLLIDER_END.LEFT)
        {
            //修正X座標取得
            fixPosition.x = playerColliderEnd;

            //修正分のY座標取得
            fixPosition.y = tilt * fixPosition.x;
        }
        else
        {
            //修正Y座標取得
            fixPosition.y = playerColliderEnd;

            //修正分Xの座標取得
            fixPosition.x = fixPosition.y / tilt;
        }

        fixPosition -= nowPosition;

        return fixPosition;
    }
}
