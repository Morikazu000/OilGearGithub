using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : PlayerMotionManager
{
    //通常時1m当たりの移動にかかる秒数
    [SerializeField, Header("通常時移動速度(s/m)")]
    private float _defaultWalkTime;

    //油さし時1m当たりの移動にかかる秒数
    [SerializeField, Header("油さし時移動速度(s/m)")]
    private float _insertOilWalkTime;

    //空中時1m当たりの移動にかかる秒数
    [SerializeField, Header("空中時移動速度(s/m)")]
    private float _airWalkTime;

    //右入力値
    const float RIGHT_INPUT = 1;

    //左入力値
    const float LEFT_INPUT = -1;

    //移動なし入力値
    const float NO_INPUT = 0;



    /// <summary>
    /// 移動メソッド
    /// </summary>
    /// <param name="isInsertOil">油さしフラグ</param>
    /// <param name="isAir">空中フラグ</param>
    /// <param name="inputValue">入力値</param>
    public bool Walk(bool isInsertOil,　bool isAir, float inputValue)
    {
        //歩きフラグ
        bool isWalk;

        //入力値を移動方向に変換
        if (inputValue > 0)
        {
            //右入力
            inputValue = RIGHT_INPUT;
        }
        else if (inputValue < 0)
        {
            //左入力
            inputValue = LEFT_INPUT;
        }
        else
        {
            //入力なし
            inputValue = NO_INPUT;
        }

        //移動方向により分岐
        switch (inputValue)
        {
            //右
            case RIGHT_INPUT:
                //右壁に当たっているか
                if (_characterManager.GetIsCollision
                        (CharacterManager.COLLISION.RIGHT_WALL))
                {
                    //移動しないようにする為入力を消す
                    inputValue = NO_INPUT;
                }

                //右向きに
                _playerControler.GetPlayerDirection(true);

                break;


            //左
            case LEFT_INPUT:
                //左壁に当たっているか
                if (_characterManager.GetIsCollision
                        (CharacterManager.COLLISION.LEFT_WALL))
                {
                    //移動しないようにする為入力を消す
                    inputValue = NO_INPUT;
                }

                //左向きに
                _playerControler.GetPlayerDirection(false);

                break;
        }

        //移動するか
        if (inputValue != NO_INPUT)
        {
            //1フレームに対する移動距離取得用
            float oneFrameWalkRange;

            //移動速度取得用
            float walkTime;


            //空中にいるか
            if (isAir)
            {
                //空中時移動速度格納
                walkTime = _airWalkTime;
            }
            //油がさされているか
            else if (isInsertOil)
            {
                //油さし時移動速度格納
                walkTime = _insertOilWalkTime;
            }
            else
            {
                //通常時移動速度格納
                walkTime = _defaultWalkTime;
            }

            //1フレームに対する移動距離取得
            oneFrameWalkRange = GetFixedDeltaTime() / walkTime;

            //移動
            transform.position += Vector3.right * 
                                    inputValue * oneFrameWalkRange;

            //接地しておらず、かつ空中にいないか
            if (!_characterManager.GetIsCollision
                        (CharacterManager.COLLISION.GROUND) && !isAir)
            {
                //落下開始
                _playerControler.FallStart();
            }

            //移動フラグtrue
            isWalk = true;
        }
        else
        {
            //移動フラグfalse
            isWalk = false;
        }

        return isWalk;
    }



    /// <summary>
    /// 移動速度倍率取得メソッド
    /// </summary>
    /// <returns>移動速度倍率</returns>
    public float GetWalkSpeedMagnification()
    {
        //移動速度倍率取得用
        float walkSpeedMagnification;


        //移動速度倍率取得
        walkSpeedMagnification = _defaultWalkTime / _insertOilWalkTime;

        return walkSpeedMagnification;
    }
}
