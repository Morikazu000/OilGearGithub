using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/// --------------------------------------------------------- 
/// #SmallEnemyManager.cs#
/// 
/// Base.Start()��K���p�����Start���\�b�h�Ŏ��s���Ă�������
/// 
/// �v���C���[�Ƃ̓����蔻����g�p����ꍇ�́A
/// PlayerBoxCollisionStart()��Start���\�b�h�Ŏ��s��
/// PlayerBoxCollision()��FixedUpdate�Ŏ��s���Ă�������
/// ---------------------------------------------------------

public class SmallEnemyManager : EnemyManager
{
    // SmallEnemyManager�ɓ����Ă���G�̃p�����[�^
    private EnemyCommonParameter _enemyParameter;

    // �v���C���[�ƓG(���L����)�̃R���C�_�[�̒��S�ɑ΂��钸�_�̍��W
    private Vector2 _playerLocalTopPosMax = default;
    private Vector2 _playerLocalTopPosMin = default;
    private Vector2 _myLocalTopPosMax = default;
    private Vector2 _myLocalTopPosMin = default;

    // �����n�߂Ă���̎��Ԃ��v������ϐ�
    private float _fallTime = default;

    // �v���C���[�I�u�W�F�N�g
    protected GameObject _playerObj;


    /// <summary>
    /// Scene�J�n���Ɏ擾���ׂ��ϐ����擾���郁�\�b�h
    /// </summary>
    protected void Start()
    {
        //EnemyCommonParameter���擾
        GetSmallEnemyParameter();

        //�v���C���[�̃I�u�W�F�N�g���擾
        _playerObj = GameObject.FindWithTag("Player");

        //�v���C���[��CharacterManager���擾
        _playerCharacterManager = _playerObj.GetComponent<CharacterManager>();
        
        // ���L�����̃R���C�_�[�̑傫�����擾����
        CollisionStart();
    }


    /// <summary>
    /// ���������郁�\�b�h(FixedUpdate�ɋL�q)
    /// </summary>
    protected void UseGravity()
    {
        // �n�ʂɐG�ꂽ�Ƃ��ڒn���鏈��
        GroundRay();

        // �n�ʂɐG��Ă��Ȃ��Ƃ��̏���
        if (!_isGroundTouch)
        {
            // �����Ă��鎞�Ԃ����Z����
            _fallTime += Time.fixedDeltaTime;

            // �������������߂�
            float nowFallDistance = _enemyParameter._fallGravity * _fallTime;

            // �������x���ő�l�𒴂��Ă�����ő�l�ɂ���
            if (nowFallDistance > _enemyParameter._maxFallSpeed)
            {
                nowFallDistance = _enemyParameter._maxFallSpeed;
            }
            // ���ۂɗ��������鏈��
            this.transform.Translate(new Vector3(0, -nowFallDistance * Time.deltaTime, 0));
        }

        // �n�ʂɐG��Ă���Ƃ��̏���
        else
        {
            // ������n�ʂɂ����Ƃ��A�������Ԃ�����������
            if (_fallTime != 0)
            {
                _fallTime = 0;
            }
        }
    }


    #region �G�ƃv���C���[�Ƃ̒��ړI�ȏՓ˔���Ɋւ������

    /// <summary>
    /// �G(���L����)�ƃv���C���[�̒��ړI�ȏՓ˔�����Ƃ�Ƃ��̏����v�Z
    /// </summary>
    protected void EnemyCollisionStart()
    {
        BoxCollider2D playerCollider = _playerObj.GetComponent<BoxCollider2D>();

        // �v���C���[�̃R���C�_�[�̃T�C�Y�ƒ��S����̂�����擾����
        Vector2 playerHalfSize = _playerObj.transform.localScale * playerCollider.size / 2;
        Vector2 playerOffset = _playerObj.transform.localScale * playerCollider.offset;

        // �v���C���[�ƓG(���L����)�̃R���C�_�[�̒��S�ɑ΂��钸�_�̍��W�����߂�
        _playerLocalTopPosMax = new Vector2(playerOffset.x + playerHalfSize.x, playerOffset.y + playerHalfSize.y);
        _playerLocalTopPosMin = new Vector2(playerOffset.x - playerHalfSize.x, playerOffset.y - playerHalfSize.y);
        _myLocalTopPosMax = new Vector2(_myColliderOffset.x + _myHalfColliderSize.x, _myColliderOffset.y + _myHalfColliderSize.y);
        _myLocalTopPosMin = new Vector2(_myColliderOffset.x - _myHalfColliderSize.x, _myColliderOffset.y - _myHalfColliderSize.y);

        // ���L�����̃R���C�_�[�̑傫�����擾����
        CollisionStart();
    }

    /// <summary>
    /// �G�ƃv���C���[�̓����蔻����Ƃ�Ƃ���fixedUpdate�Ŏg�����\�b�h
    /// </summary>
    protected void PlayerBoxCollision()
    {
        // �v���C���[��Collider�̒��_�̍��W�����߂�
        Vector2 playerColliderPosMax = new Vector2(_playerObj.transform.position.x + _playerLocalTopPosMax.x, _playerObj.transform.position.y + _playerLocalTopPosMax.y);
        Vector2 playerColliderPosMin = new Vector2(_playerObj.transform.position.x + _playerLocalTopPosMin.x, _playerObj.transform.position.y + _playerLocalTopPosMin.y);

        // ���I�u�W�F�N�g��Collider�̒��_�̍��W�����߂�
        Vector2 myColliderPosMax = new Vector2(this.transform.position.x + _myLocalTopPosMax.x, this.transform.position.y + _myLocalTopPosMax.y);
        Vector2 myColliderPosMin = new Vector2(this.transform.position.x + _myLocalTopPosMin.x, this.transform.position.y + _myLocalTopPosMin.y);

        // X���̔�����s��
        if (playerColliderPosMax.x <= myColliderPosMax.x && playerColliderPosMax.x >= myColliderPosMin.x) { }
        else if (playerColliderPosMin.x <= myColliderPosMax.x && playerColliderPosMin.x >= myColliderPosMin.x) { }
        else if (playerColliderPosMax.x >= myColliderPosMax.x && playerColliderPosMin.x <= myColliderPosMin.x) { }

        // X���ɓ������Ă��Ȃ��Ƃ���������߂�
        else { return; }

        // Y���̔�����s��
        if (playerColliderPosMax.y <= myColliderPosMax.y && playerColliderPosMax.y >= myColliderPosMin.y) { }
        else if (playerColliderPosMin.y <= myColliderPosMax.y && playerColliderPosMin.y >= myColliderPosMin.y) { }
        else if (playerColliderPosMax.y >= myColliderPosMax.y && playerColliderPosMin.y <= myColliderPosMin.y) { }

        // X���ɓ������Ă��Ȃ��Ƃ���������߂�
        else { return; }

        // �����蔻�肪�d�Ȃ��Ă���Ƃ��_���[�W��^����
        _playerCharacterManager.TakesDamage(_enemyParameter._collisionDamage);

    }

    #endregion


    /// <summary>
    /// EnemyCommonParameter���擾���郁�\�b�h
    /// </summary>
    protected void GetSmallEnemyParameter()
    {
        //EnemyCommonParameter���擾
        _enemyParameter = Resources.Load<EnemyCommonParameter>(characterCommonParameter._myParameterObjName);

        //���[�h�o���Ȃ������ꍇ�̓G���[���O��\��
        if (_enemyParameter == null)
        {
            Debug.LogError(characterCommonParameter._myParameterObjName + " Not Found�@�A�N�Z�X�G���[�ł�");
        }
    }

}
