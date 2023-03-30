using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerMotionManager
{
    //SE
    [SerializeField, Header("SE")]
    private GameObject _SE;
    //EffectPoint
    [SerializeField, Header("SE")]
    private GameObject _effectPoint;

    //����ł̍U������̃Ɗp
    [SerializeField, Header("����U������p�x(��)")]
    private float _groundAttackAngle;

    //�󒆂ł̍U������̃Ɗp
    [SerializeField, Header("�󒆍U������p�x(��)")]
    private float _airAttackAngle;

    //����ł̍U���̒���
    [SerializeField, Header("����U�����蒷(m)")]
    private float _groundAttackRange;

    //�󒆂ł̍U���̒���
    [SerializeField, Header("�󒆍U�����蒷(m)")]
    private float _airAttackRange;

    //�ʏ펞�ɍU���J�n��������ۂɍU�����肪�o��܂ł̎���
    [SerializeField, Header("�ʏ펞�U���J�n���画�肪�o��܂ł̎���(s)")]
    private float _defaultAttackStandByTime;

    //�ʏ펞�ɍU�����肪�o�ăA�C�h���ɖ߂�܂ł̎���
    [SerializeField, Header("�ʏ펞�U�����肪�o�鎞��(s)")]
    private float _defaultAttackActionTime;

    //�I�C���������ɍU���J�n��������ۂɍU�����肪�o��܂ł̎���
    [SerializeField, Header("�I�C���������U���J�n���画�肪�o��܂ł̎���(s)")]
    private float _insertOilAttackStandByTime;

    //�I�C���������ɍU�����肪�o�ăA�C�h���ɖ߂�܂ł̎���
    [SerializeField, Header("�I�C���������U�����肪�o�鎞��(s)")]
    private float _insertOilAttackActionTime;

    //�U���J�n���t���O
    private bool _isAttackStart = true;

    //���݂̍U���J�n����̎��Ԏ擾�p
    private float _nowAttackTime = 0;

    //�U���J�n��������ۂɍU�����肪�o��܂ł̎��Ԋi�[�p
    private float _attackStandByTime = default;

    //�U�����肪�o�ăA�C�h���ɖ߂�܂ł̎��Ԋi�[�p
    private float _attackActionTime = default;

    //�v���C���[�̃R���C�_�[�̑傫���i�[�p
    private Vector2 _playerColliderSize;

    //�v���C���[�̉E�����t���O
    private bool _isPlayerDirectionRight = default;

    //���ɍU�������G�i�[�p
    private GameObject[] _hitEnemys = default;

    //�R���{�X�N���v�g�i�[�p
    private PlayerCombo _playerCombo;



    private void Start()
    {
        //�v���C���[�̃R���C�_�[�擾�p
        BoxCollider2D playerCollider;


        //�v���C���[�̃R���C�_�[�擾
        playerCollider = GetComponent<BoxCollider2D>();

        //�v���C���[�̃R���C�_�[�̑傫���i�[
        _playerColliderSize = transform.lossyScale * playerCollider.size;

        //�R���{�X�N���v�g�i�[
        _playerCombo = GetComponent<PlayerCombo>();
    }



    public void Attack(bool isRight, bool isInsertOil, bool isAir)
    {
        //�U���J�n����
        if (_isAttackStart)
        {
            //���݂̍U���J�n����̎��ԏ�����
            _nowAttackTime = 0;

            //���ɍU�������G�i�[�p�ϐ�������
            _hitEnemys = new GameObject[0];

            //�v���C���[�̌����i�[
            _isPlayerDirectionRight = isRight;

            //��������Ԃ�
            if (isInsertOil)
            {
                //���������ɍU���J�n��������ۂɍU�����肪�o��܂ł̎��Ԋi�[
                _attackStandByTime = _insertOilAttackStandByTime;

                //���������ɍU�����肪�o�ăA�C�h���ɖ߂�܂ł̎��Ԋi�[
                _attackActionTime = _insertOilAttackActionTime;
            }
            else
            {
                //�ʏ펞�ɍU���J�n��������ۂɍU�����肪�o��܂ł̎��Ԋi�[
                _attackStandByTime = _defaultAttackStandByTime;

                //�ʏ펞�ɍU�����肪�o�ăA�C�h���ɖ߂�܂ł̎��Ԋi�[
                _attackActionTime = _defaultAttackActionTime;
            }

            //�U���J�n���t���Ofalse
            _isAttackStart = false;
        }

        //���݂̍U���J�n����̎��ԍX�V
        _nowAttackTime += GetFixedDeltaTime();

        //���ۂɍU��������o����
        if (_nowAttackTime >= _attackStandByTime)
        {
            /*�U������*/

            //�U���͈͂ɓ������I�u�W�F�N�g�擾�p
            RaycastHit2D[] rayHits;

            //�U���͈͌��_�擾�p
            Vector2 attackOrigin;

            //�U�����擾�p
            float attackRange;

            //�U������̃Ɗp�擾�p
            float attackAngle;

            //�U������̊p�̎��擾�p
            float attackShaft;


            //�󒆂ɂ��邩
            if (isAir)
            {
                //�󒆂ł̍U�����ƍU������̃Ɗp�A�p�̎��擾
                attackRange = _airAttackRange;
                attackAngle = _airAttackAngle;
                attackShaft = 270;
            }
            else
            {
                //����ł̍U�����ƍU������̃Ɗp�擾
                attackRange = _groundAttackRange;
                attackAngle = _groundAttackAngle;

                //�v���C���[���E������
                if (_isPlayerDirectionRight)
                {
                    //�U������̊p�̎����E��
                    attackShaft = 0;
                }
                else
                {
                    //�U������̊p�̎�������
                    attackShaft = 180;
                }
            }

            //�U���͈͌��_�擾
            attackOrigin = new Vector2(transform.position.x, transform.position.y - _playerColliderSize.y / 2);

            //�U���͈͂ɓ������I�u�W�F�N�g�擾
            rayHits = Physics2D.CircleCastAll(attackOrigin, attackRange, Vector2.zero, 0);

            //�U���͈͂ɓ������I�u�W�F�N�g�T��
            foreach (RaycastHit2D rayHit in rayHits)
            {
                //�U���ڐG�t���O
                bool isHit = false;


                //�ڐG�����̂��G��
                if (rayHit.collider.gameObject.tag == "Enemy")
                {
                    //�U���ڐG�t���O�擾
                    isHit = GetHitEnemyFlag(rayHit.collider.gameObject, _isPlayerDirectionRight,
                                                attackOrigin, attackAngle, attackShaft);
                }

                //�U���ڐG������
                if (isHit)
                {
                    /*�G��U������(�G��rayHit.collider.gameObject)*/

                    //�ڐG�����G�擾�p
                    GameObject enemy;

                    //�ڐG�����G��EnemyManager�擾�p
                    EnemyManager enemyManager;

                    //�U���͎擾�p
                    int attackPoint;

                    //�I�C���h���b�v���擾�p
                    int dropOilAmount;


                    //�ڐG�����G�擾
                    enemy = rayHit.collider.gameObject;

                    //�ڐG�����G��CharacterManager�擾
                    enemyManager = enemy.GetComponent<EnemyManager>();

                    //�R���{�X�V
                    _playerCombo.Combo();

                    //�U���͎擾
                    attackPoint = _playerCombo.GetAttackPoint();

                    //�I�C���h���b�v���擾
                    dropOilAmount = _playerCombo.GetDropOilAmount();

                    //�K�[�h���ꂽ��
                    if (enemyManager.PlayerAttackEnemy())
                    {
                        //�͂�����čU�����~
                        _playerControler.DefaultMotion();

                        _SE.GetComponent<SE_Controller>().PlaySE(_effectPoint.transform.position, "�K�[�h");
                    }
                    else
                    {
                        //�G��CharacterManager�擾�p
                        CharacterManager characterManager;

                        switch(Random.Range(1, 3)){
                            case 1:
                                _SE.GetComponent<SE_Controller>().PlaySE(_effectPoint.transform.position, "�U��A");
                                break;
                            case 2:
                                _SE.GetComponent<SE_Controller>().PlaySE(_effectPoint.transform.position, "�U��B");
                                break;
                            case 3:
                                _SE.GetComponent<SE_Controller>().PlaySE(_effectPoint.transform.position, "�U��C");
                                break;
                        }

                        //�G��CharacterManager�擾
                        characterManager = enemy.GetComponent<CharacterManager>();
                        //�_���[�W����
                        characterManager.TakesDamage(attackPoint);
                    }

                    //�U�������G���i�[
                    _hitEnemys = new GameObject[_hitEnemys.Length + 1];
                    _hitEnemys[_hitEnemys.Length - 1] = enemy;
                }
            }
        }
    }



    /// <summary>
    /// �U���I�����\�b�h
    /// </summary>
    public void AttackEnd()
    {
        //�U���J�n���t���O������
        _isAttackStart = true;
    }



    /// <summary>
    /// �U�����Ԃ�Ԃ����\�b�h
    /// </summary>
    /// <returns>�U������</returns>
    public float GetAttackTime()
    {
        return _attackActionTime + _attackStandByTime;
    }



    /// <summary>
    /// �G�ڐG�t���O�擾���\�b�h
    /// </summary>
    /// <param name="enemy">�G�I�u�W�F�N�g</param>
    /// <param name="isRight">�v���C���[�E�����t���O</param>
    /// <param name="attackOrigin">�U���͈͌��_</param>
    /// <param name="attackAngle">�U�����莲�p�x</param>
    /// <param name="attackShaft">�U���͈͊p�x</param>
    /// <returns>�G�ڐG�t���O</returns>
    private bool GetHitEnemyFlag(GameObject enemy, bool isRight, Vector2 attackOrigin,
                                    float attackAngle, float attackShaft)
    {
        //�G�ڐG�t���O
        bool isHit = true;


        //�ڐG�ς݂̓G�T��
        foreach(GameObject hitEnemy in _hitEnemys)
        {
            if (hitEnemy == enemy)
            {
                //�G�ڐG�t���Ofalse
                isHit = false;

                break;
            }
        }

        //�Ώۂ̓G���ڐG�ς݂̓G���X�g�ɂ��Ȃ�������
        if (isHit)
        {
            //�G�̊p�x�擾�p
            float enemyAngle;

            //�͈͍Œ�p�x�擾�p
            float minAngle;

            //�͈͍ő�p�x�擾�p
            float maxAngle;

            //�p�x�I�[�o�[�t���O
            bool isOverAngle = false;


            //�G�̊p�x�擾
            enemyAngle = GetTwoPointAngle(attackOrigin, enemy.transform.position);

            //�v���C���[���E������
            if (isRight)
            {
                //�͈͍Œ�p�x�擾
                minAngle = attackShaft;

                //�͈͍ő�p�x�擾
                maxAngle = attackShaft + attackAngle;

                //�ő�p�x���P���𒴂�����
                if (maxAngle >= 360)
                {
                    //�p�x��0�`360�x�ɕϊ�
                    maxAngle -= 360;

                    //�p�x�I�[�o�[�t���Otrue
                    isOverAngle = true;
                }
            }
            else
            {
                //�͈͍Œ�p�x�擾
                minAngle = attackShaft - attackAngle;

                //�͈͍ő�p�x�擾
                maxAngle = attackShaft;

                //�Œ�p�x��0�x������
                if (minAngle < 0)
                {
                    //�p�x��0�`360�x�ɕϊ�
                    maxAngle += 360;

                    //�p�x�I�[�o�[�t���Otrue
                    isOverAngle = true;
                }
            }

            //�ő�p�x���P���𒴂�����
            if (isOverAngle)
            {
                //�G���U���͈͊O��
                if(!(enemyAngle >= minAngle || enemyAngle <= maxAngle))
                {
                    //�G�ڐG�t���Ofalse
                    isHit = false;
                }
            }
            else
            {
                //�G���U���͈͊O��
                if (!(enemyAngle >= minAngle && enemyAngle <= maxAngle))
                {
                    //�G�ڐG�t���Ofalse
                    isHit = false;
                }
            }
        }

        return isHit;
    }



    /// <summary>
    /// �Q�_�Ԃ̊p�x���擾���郁�\�b�h
    /// </summary>
    /// <param name="startPosition">�J�n���W</param>
    /// <param name="targetPosition">�ڕW���W</param>
    /// <returns>�Q�_�Ԃ̊p�x</returns>
    private float GetTwoPointAngle(Vector2 startPosition, Vector2 targetPosition)
    {
        //�^�[�Q�b�g�܂ł̃x�N�g���擾�p
        Vector2 targetVector;

        //�^�[�Q�b�g�܂ł̊p�x�擾�p
        float angle;


        //�^�[�Q�b�g�܂ł̃x�N�g���擾
        targetVector = targetPosition - startPosition;

        //�^�[�Q�b�g�܂ł̊p�x(���W�A��)�擾
        angle = Mathf.Atan2(targetVector.y, targetVector.x);

        //���W�A����x�ɕϊ�
        angle *= Mathf.Rad2Deg;

        //�p�x��0�x������
        if (angle < 0)
        {
            //�p�x��0�`360�x�ɕϊ�
            angle += 360;
        }

        return angle;
    }
}
