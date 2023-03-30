using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotionManager : NomaBehaviour
{
    //プレイヤーのCharacterManager格納用
    protected CharacterManager _characterManager;

    //PlayerControler格納用
    protected PlayerControler _playerControler;

    //プレイヤーのクラス取得済みフラグ
    private bool _isGetPlayerClass = false;

    //コライダー端enum
    protected enum COLLIDER_END
    {
        RIGHT,
        LEFT,
        TOP,
        LOWEST,
    };



    /// <summary>
    /// プレイヤーのクラス取得メソッド
    /// </summary>
    /// <param name="collisionCast">プレイヤーのCollisionCast</param>
    /// /// <param name="playerControler">プレイヤーのPlayerControler</param>
    public void GetPlayerClass(CharacterManager characterManager,
                                    PlayerControler playerControler)
    {
        //プレイヤーのCollisionCastをまだ取得していないか
        if (!_isGetPlayerClass)
        {
            //プレイヤーのCharacterManager取得
            _characterManager = characterManager;

            //プレイヤーのPlayerControler取得
            _playerControler = playerControler;

            //プレイヤーのクラス取得済みフラグtrue
            _isGetPlayerClass = true;
        }
    }
}
