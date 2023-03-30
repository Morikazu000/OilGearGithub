using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFall : PlayerMotionManager
{
    //落下から加速し続ける高さ
    [SerializeField, Header("最大加速高度(m)")]
    private float _fallMaxAcceralationHeight;

    //ジャンプしたときに最高高度から接地までの時間
    [SerializeField, Header("接地時間(s)")]
    private float _fallMaxTime;

    //現在の落下高度
    private float _fallNowHeight;

    //現在の落下時間
    private float _fallNowTime = 0;

    //落下等速フラグ
    private bool _isFallVelocity = false;

    //等速時傾き
    private float _velocityTilt = default;

    //等速時切片
    private float _velocityIntercept = default;

    //落下開始フラグ
    private bool _isFallStart = true;

    //プレイヤーのコライダー格納用
    private BoxCollider2D _playerCollider;



    private void Start()
    {
        //プレイヤーのコライダー格納
        _playerCollider = GetComponent<BoxCollider2D>();
    }



    /// <summary>
    /// 落下メソッド
    /// </summary>
    public float Fall(float jumpMaxHeight)
    {
        //接地しているか
        if (_characterManager.GetIsCollision(CharacterManager.COLLISION.GROUND))
        {
            //パラメータ初期化
            ResetParamater();

            //落下終了
            _playerControler.DefaultMotion();

            return 0;
        }

        /*-------------------*/
        //二次関数方程式
        //y=a(x-p)^2+q
        /*-------------------*/

        //計算用
        float a;

        //落下距離
        float fallDownHeight;

        //地面判定取得用
        RaycastHit2D[] rayHits;

        //地面判定フラグ
        bool isGround = false;

        //プレイヤーの座標Vector2型変換用
        Vector2 playerPosition;

        //落下距離取得用
        float fallDownheight;


        //落下開始時か
        if (_isFallStart)
        {
            //落下初期高度取得
            _fallNowHeight = jumpMaxHeight;

            //落下開始フラグfalse
            _isFallStart = false;
        }

        //a算出
        a = -jumpMaxHeight * 2 / (Mathf.Pow(_fallMaxTime, 2) * 2);

        //落下時間更新
        _fallNowTime += GetFixedDeltaTime();

        //等速落下するか
        if (_isFallVelocity)
        {
            //落下距離取得
            fallDownHeight = _velocityTilt * _fallNowTime + _velocityIntercept;
        }
        else
        {
            //落下距離取得
            fallDownHeight = a * Mathf.Pow(_fallNowTime, 2) + jumpMaxHeight;
            
            //現在の落下高度が加速し続ける高さを超えたか
            if (fallDownHeight <= jumpMaxHeight - _fallMaxAcceralationHeight)
            {
                /*-------------------*/
                //一次関数方程式
                //y=ax+b
                /*-------------------*/

                //前フレームの落下高度取得用
                float fallBeforeHeight;

                //前フレームの落下時間取得用
                float fallBeforeTime;

                
                //前フレームの落下時間取得
                fallBeforeTime = _fallNowTime - GetFixedDeltaTime();

                //前フレームの落下高度取得
                fallBeforeHeight = a * Mathf.Pow(fallBeforeTime, 2) + jumpMaxHeight;
                
                //傾き算出
                _velocityTilt = (fallDownHeight - fallBeforeHeight) 
                                        / (_fallNowTime - fallBeforeTime);
                
                //切片算出
                _velocityIntercept = fallDownHeight - _velocityTilt * _fallNowTime;
                
                //落下等速フラグtrue
                _isFallVelocity = true;
            }
        }
        
        //プレイヤーの座標をVector2型に変換
        playerPosition = new Vector2(transform.position.x, transform.position.y);

        //落下する予定の高度までのRay取得
        rayHits = Physics2D.RaycastAll(playerPosition, Vector2.down, _fallNowHeight - fallDownHeight
                                        + transform.lossyScale.y * _playerCollider.size.y / 2);

        //地面のタグを探索
        foreach(RaycastHit2D rayHitObj in rayHits)
        {
            //Rayに接触したオブジェクトが地面タグか
            if (rayHitObj.collider.gameObject.tag == "Ground")
            {
                //地面のコライダー取得用
                BoxCollider2D groundCollider;

                //地面のTransform取得用
                Transform groundTransform;

                //地面のコライダーのYの大きさ取得用
                float groundSizeY;

                //プレイヤーのコライダーのYの大きさ取得用
                float playerSizeY;


                //地面判定フラグtrue
                isGround = true;

                //地面のコライダー取得
                groundCollider = rayHitObj.collider.gameObject.GetComponent<BoxCollider2D>();

                //地面のTransform取得
                groundTransform = rayHitObj.collider.gameObject.transform;

                //地面のコライダーのYの大きさ取得
                groundSizeY = groundTransform.lossyScale.y * groundCollider.size.y;

                //プレイヤーのコライダーのYの大きさ取得
                playerSizeY = transform.lossyScale.y * _playerCollider.size.y;

                //落下距離取得
                fallDownHeight = _fallNowHeight - Mathf.Abs(transform.position.y 
                                    - groundTransform.position.y) + groundSizeY / 2 + playerSizeY / 2;

                break;
            }
        }
        
        //落下距離取得
        fallDownheight = _fallNowHeight - fallDownHeight;
        
        //落下
        transform.position += Vector3.down * fallDownheight;

        //現在の落下高度更新
        _fallNowHeight = fallDownHeight;

        return -fallDownheight;
    }



    /// <summary>
    /// パラメータ初期化メソッド
    /// </summary>
    public void ResetParamater()
    {
        //落下開始フラグ初期化
        _isFallStart = true;

        //現在の落下時間初期化
        _fallNowTime = 0;

        //等速落下フラグ初期化
        _isFallVelocity = false;
    }
}
