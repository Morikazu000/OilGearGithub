using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeHit : PlayerMotionManager
{
    //ひるむ時間(s)
    [SerializeField, Header("ひるみ時間(s)")]
    private float _stanTime;

    //ノックバックする距離
    [SerializeField, Header("ノックバック距離(m)")]
    private float _knockBackRange;

    //現在のひるみ時間
    private float _stanNowTime = 0;

    //ノックバック方向フラグ
    private bool _isKnockBackRight;

    //ノックバック開始フラグ
    private bool _isKnockBackStart = true;



    public void TakeHit(bool isKnockBackRight)
    {
        //ノックバック開始時か
        if (_isKnockBackStart)
        {
            //ノックバック方向フラグ取得
            _isKnockBackRight = isKnockBackRight;

            //現在のスタン時間初期化
            _stanNowTime = 0;

            _isKnockBackStart = false;
        }

        //現在のひるみ時間更新
        _stanNowTime += GetFixedDeltaTime();

        //ひるみ時間中か
        if (_stanNowTime < _stanTime)
        {
            //ノックバック距離取得用
            float knockBackRange;

            //ノックバック方向取得用
            Vector3 knockBackDirection;


            //ノックバック距離取得
            knockBackRange = _knockBackRange / (_stanTime / GetFixedDeltaTime());

            //ノックバックが左方向か
            if (_isKnockBackRight)
            {
                //ノックバックを右に
                knockBackDirection = Vector3.right;
            }
            else
            {
                //ノックバックを左に
                knockBackDirection = Vector3.left;
            }

            //ノックバックさせる
            transform.position += knockBackDirection * knockBackRange;
        }
        else
        {
            _isKnockBackStart = true;

            _playerControler.DefaultMotion();
        }
    }
}
