using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpNoma : PlayerMotionManager
{
    //ジャンプ開始から最高点まで到達にかかる時間
    [SerializeField, Header("ジャンプ最高高度到達までの時間(s)")]
    private float _jumpMaxTime;

    //ジャンプの最高高度
    [SerializeField, Header("ジャンプ最高高度(m)")]
    private float _jumpMaxHeight;

    //ジャンプ開始から最低点まで到達にかかる時間
    [SerializeField, Header("ジャンプ最低高度到達までの時間(s)")]
    private float _jumpMinTime;

    //ジャンプの最低高度
    [SerializeField, Header("ジャンプ最低高度(m)")]
    private float _jumpMinHeight;

    //現在のジャンプ高度格納用
    private float _jumpNowHeight = 0;

    //現在のジャンプ時間格納用
    private float _jumpNowTime = 0;

    //ジャンプ終了フラグ
    private bool _isJumpEnd = false;

    //ジャンプ開始フラグ
    private bool _isJumpStart = false;

    //ジャンプ中フラグ
    private bool _isJumpNow = false;



    private void Update()
    {
        //ジャンプ中か
        if (_isJumpNow)
        {
            //天井に当たったか
            if (_characterManager.GetIsCollision(CharacterManager.COLLISION.ROOF))
            {
                _isJumpEnd = true;
            }
        }
    }



    /// <summary>
    /// ジャンプしつつジャンプの高度を返すメソッド
    /// </summary>
    /// <param name="isInputJump">ジャンプ入力フラグ</param>
    public float Jump(bool isInputJump)
    {
        /*-------------------*/
        //二次関数方程式
        //y=a(x-p)^2+q
        /*-------------------*/

        //計算用
        float a, x, y;

        //最新のジャンプ高度取得用
        float jumpUpHeight;

        //ジャンプ時間取得用
        float jumpTime;

        //ジャンプ高度取得用
        float jumpHeight;


        //ジャンプ開始していないか
        if (!_isJumpStart)
        {
            //現在のジャンプ高度、時間初期化
            _jumpNowHeight = 0;
            _jumpNowTime = 0;

            //ジャンプ終了フラグfalse
            _isJumpEnd = false;

            //ジャンプ開始フラグtrue
            _isJumpStart = true;

            //ジャンプ中フラグ
            _isJumpNow = true;
        }

        //ジャンプ終了するか
        if (_isJumpEnd)
        {
            //ジャンプ高度、時間取得
            jumpTime = _jumpMinTime;
            jumpHeight = _jumpMinHeight;
        }
        else
        {
            //ジャンプ入力されているか
            if (isInputJump)
            {
                //ジャンプ高度、時間取得
                jumpTime = _jumpMaxTime;
                jumpHeight = _jumpMaxHeight;
            }
            else
            {
                //ジャンプ終了フラグtrue
                _isJumpEnd = true;

                //現在のジャンプ高度、時間初期化
                _jumpNowHeight = 0;
                _jumpNowTime = 0;

                //ジャンプ高度、時間取得
                jumpTime = _jumpMinTime;
                jumpHeight = _jumpMinHeight;
            }
        }

        //二次関数計算
        x = Mathf.Pow(jumpTime * 2, 2) / 2;
        y = jumpHeight * -2;
        a = y / x;

        //現在のジャンプ時間更新
        _jumpNowTime += GetFixedDeltaTime();

        //最新のジャンプ高度取得
        jumpUpHeight = a * Mathf.Pow(_jumpNowTime - jumpTime, 2) 
                            + jumpHeight;

        //落下し始めたか
        if(jumpUpHeight - _jumpNowHeight < 0)
        {
            /*落下開始*/

            //ジャンプ開始フラグfalse
            _isJumpStart = false;

            //ジャンプ中フラグfalse
            _isJumpNow = false;

            //ジャンプ高度取得
            jumpHeight = 0;

            //落下開始
            _playerControler.FallStart();
        }
        else
        {
            /*ジャンプ*/

            //ジャンプ高度取得
            jumpHeight = jumpUpHeight - _jumpNowHeight;

            //ジャンプさせる
            transform.position += Vector3.up * jumpHeight;

            //現在のジャンプ高度更新
            _jumpNowHeight = jumpUpHeight;
        }

        return jumpHeight;
    }



    /// <summary>
    /// ジャンプの最高高度を返すメソッド
    /// </summary>
    /// <returns>ジャンプの最高高度</returns>
    public float ReturnJumpMaxHeight()
    {
        return _jumpMaxHeight;
    }
}
