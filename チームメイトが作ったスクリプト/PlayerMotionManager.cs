using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotionManager : NomaBehaviour
{
    //�v���C���[��CharacterManager�i�[�p
    protected CharacterManager _characterManager;

    //PlayerControler�i�[�p
    protected PlayerControler _playerControler;

    //�v���C���[�̃N���X�擾�ς݃t���O
    private bool _isGetPlayerClass = false;

    //�R���C�_�[�[enum
    protected enum COLLIDER_END
    {
        RIGHT,
        LEFT,
        TOP,
        LOWEST,
    };



    /// <summary>
    /// �v���C���[�̃N���X�擾���\�b�h
    /// </summary>
    /// <param name="collisionCast">�v���C���[��CollisionCast</param>
    /// /// <param name="playerControler">�v���C���[��PlayerControler</param>
    public void GetPlayerClass(CharacterManager characterManager,
                                    PlayerControler playerControler)
    {
        //�v���C���[��CollisionCast���܂��擾���Ă��Ȃ���
        if (!_isGetPlayerClass)
        {
            //�v���C���[��CharacterManager�擾
            _characterManager = characterManager;

            //�v���C���[��PlayerControler�擾
            _playerControler = playerControler;

            //�v���C���[�̃N���X�擾�ς݃t���Otrue
            _isGetPlayerClass = true;
        }
    }
}
