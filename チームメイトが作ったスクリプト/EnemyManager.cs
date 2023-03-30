using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// --------------------------------------------------------- 
/// #EnemyManager.cs#
/// 体力をもつすべての敵に共通しているものをまとめています
/// ---------------------------------------------------------

public class EnemyManager : CharacterManager
{
    protected CharacterManager _playerCharacterManager;

    #region 継承先で使うフラグ

    //ガード(盾)できるのかを判定する
    protected bool _canGuard;

    //ガード中(盾)に攻撃されたか
    protected bool _isAttackedGuard;

    #endregion



    /// <summary>
    /// プレイヤーが攻撃したとき、敵がガードできたかどうかを返すメソッド
    /// </summary>
    public bool PlayerAttackEnemy()
    {
        //ガードしていたら、ガードに攻撃されたフラグをtrueにする
        if(_canGuard)
        {
            _isAttackedGuard = true;
        }

        return _canGuard;
    }
}
