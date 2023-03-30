using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlink : PlayerMotionManager
{
    //�u�����N�ňړ��ł���ő勗��
    [SerializeField, Header("�u�����N�ő�ړ�����(m)")]
    private float _blinkMaxRange;

    //�u�����N�ō����ňړ����鋗��
    [SerializeField, Header("�u�����N�����ړ�����(m)")]
    private float _blinkHighSpeedRange;

    //�u�����N�ōő勗���܂ł̈ړ��ɂ����鎞��
    [SerializeField, Header("�u�����N�ړ�����(s)")]
    private float _blinkMaxTime;

    //�u�����N�ō����ňړ����鎞��
    [SerializeField, Header("�u�����N�����ړ�����(s)")]
    private float _blinkHighSpeedTime;

    //�u�����N�ŏ����HP(����Ȃ�������u�����N�s��)
    [SerializeField, Header("�u�����N�Ŏg�p����HP")]
    private int _blinkUseHp;

    //�u�����N�ŕǂ⏰�A�V��ɐڐG�����ꍇ�ɗP�\�������߂ɋ󂯂錄��
    [SerializeField, Header("�u�����N�ŕǂɓ��������ꍇ�ɋ󂯂錄��(m)")]
    private float _collisionWallVacateSpace;

    //�����ړ��t���O
    private bool _isHighSpeed = true;

    //���݂̃u�����N�ړ�����
    private float _blinkNowRange = 0;

    //���݂̃u�����N�ړ�����
    private float _blinkNowTime = 0;

    //�u�����N�J�n���t���O
    private bool _isBlinkStart = true;

    //�u�����N�̌����x�N�g���i�[�p
    private Vector2 _blinkDirectionVector;

    //�v���C���[�̃R���C�_�[�̑傫���i�[�p
    private Vector2 _playerColliderSize;

    //�I�C���X�N���v�g�i�[�p
    private PlayerOil _playerOil;



    private void Start()
    {
        //�v���C���[�̃R���C�_�[�擾�p
        BoxCollider2D playerCollider;


        //�v���C���[�̃R���C�_�[�擾
        playerCollider = GetComponent<BoxCollider2D>();

        //�v���C���[�̃R���C�_�[�̑傫���i�[
        _playerColliderSize.x = transform.lossyScale.x * playerCollider.size.x;
        _playerColliderSize.y = transform.lossyScale.y * playerCollider.size.y;

        //�I�C���X�N���v�g�i�[
        _playerOil = GetComponent<PlayerOil>();
    }



    /// <summary>
    /// �u�����N���u�����N�̈ړ��x�N�g����Ԃ����\�b�h
    /// </summary>
    /// <param name="stickInput">�X�e�B�b�N���͒l</param>
    /// <param name="isPlayerDirectionRight">�E�����t���O</param>
    /// <returns>�u�����N�̈ړ��x�N�g��</returns>
    public Vector2 Blink(Vector2 stickInput, bool isPlayerDirectionRight)
    {
        //�u�����N�J�n����
        if (_isBlinkStart)
        {
            //HP����
            _playerOil.UseOil(_blinkUseHp, false);
            
            //�u�����N�̌����x�N�g���擾
            _blinkDirectionVector = InputConversion(stickInput, isPlayerDirectionRight);

            //���݂̃u�����N�ړ������A���ԏ�����
            _blinkNowRange = 0;
            _blinkNowTime = 0;

            //�����ړ��t���Otrue
            _isHighSpeed = true;

            //�u�����N�J�n���t���Ofalse
            _isBlinkStart = false;
        }

        /*----------------------*/
        //�ꎟ�֐�
        //y=ax+b
        //
        //�O�����̒藝
        //a^2+b^2=c^2
        /*----------------------*/

        //�v�Z�p
        float tilt;

        //�ŐV�̃u�����N�ړ������擾�p
        float blinkRange;

        //�u�����N�I���t���O
        bool isBlinkEnd = false;

        //���C�ɐڐG�����I�u�W�F�N�g�擾�p
        RaycastHit2D[] rayHits;

        //�ڐG���̏��擾�p
        GameObject touchGround = default;

        //�u�����N�̈ړ������W�擾�p
        Vector3 blinkPosition = Vector3.zero;

        //�u�����N�̃x�N�g��X�AY�̊����擾�p
        float blinkRatio;


        //���݂̃u�����N���ԍX�V
        _blinkNowTime += GetFixedDeltaTime();

        //�����ړ�����
        if (_isHighSpeed)
        {
            //�����ړ����̈ꎟ�֐��̌X���擾
            tilt = _blinkHighSpeedRange / _blinkHighSpeedTime;

            //�ŐV�̃u�����N�ړ������擾
            blinkRange = tilt * _blinkNowTime;

            //�����ړ������ȏ�ɐi�񂾂�
            if (blinkRange >= _blinkHighSpeedRange)
            {
                //�����ړ��t���Ofalse
                _isHighSpeed = false;
            }
        }
        else
        {
            //�ʏ�ړ����̈ꎟ�֐��̌X���擾
            tilt = (_blinkMaxRange - _blinkHighSpeedRange) / _blinkMaxTime;

            //�ŐV�̃u�����N�ړ������擾
            blinkRange = tilt * _blinkNowTime + _blinkHighSpeedRange;

            //�u�����N�̍ő勗���ȏ�ɐi�񂾂�
            if (blinkRange >= _blinkMaxRange)
            {
                //�u�����N�I���t���Otrue
                isBlinkEnd = true;
            }
        }

        //�O��������u�����N�̃x�N�g��X�AY�̊����擾
        blinkRatio = Mathf.Pow(blinkRange - _blinkNowRange, 2) 
                        / (Mathf.Abs(_blinkDirectionVector.x) + Mathf.Abs(_blinkDirectionVector.y));
        
        //�u�����N�̈ړ������W�擾
        blinkPosition.x = Mathf.Sqrt(blinkRatio * Mathf.Abs(_blinkDirectionVector.x)) * Mathf.Sign(_blinkDirectionVector.x);
        blinkPosition.y = Mathf.Sqrt(blinkRatio * Mathf.Abs(_blinkDirectionVector.y)) * Mathf.Sign(_blinkDirectionVector.y);

        //�ڒn���Ă��邩
        if (_characterManager.GetIsCollision(CharacterManager.COLLISION.GROUND))
        {
            //���擾
            touchGround = _characterManager.GroundRay();
        }

        //�u�����N�̈ړ������Ƀ��C���΂�
        rayHits = Physics2D.BoxCastAll(transform.position, _playerColliderSize, 0,
                                        _blinkDirectionVector, blinkRange - _blinkNowRange);

        rayHits = Physics2D.RaycastAll(transform.position, _blinkDirectionVector, blinkRange - _blinkNowRange);

        //���C�Ŏ擾�����I�u�W�F�N�g�T��
        foreach (RaycastHit2D rayHit in rayHits)
        {
            //�ǁA�n�ʂɐڐG���A���ڒn���Ă�����ڐG�����̂������̏��ł͂Ȃ���
            if (rayHit.collider.gameObject.tag == "Ground")
            {
                //�ڐG�����[�擾�p
                COLLIDER_END hitColliderEnd;

                //�ڐG�����R���C�_�[�擾�p
                BoxCollider2D hitCollider;

                //�ڐG�����I�u�W�F�N�g�̍��W�擾�p
                Vector2 hitObjPos;

                //�ڐG�����R���C�_�[�̑傫���擾�p
                Vector2 hitColliderSize;


                //�ڐG�����R���C�_�[�擾
                hitCollider = rayHit.collider.gameObject.GetComponent<BoxCollider2D>();

                //�ڐG�����I�u�W�F�N�g�̍��W�擾
                hitObjPos = new Vector2(rayHit.collider.gameObject.transform.position.x,
                                            rayHit.collider.gameObject.transform.position.y);

                //�ڐG�����R���C�_�[�̑傫���擾
                hitColliderSize = new Vector2(rayHit.collider.gameObject.transform.lossyScale.x,
                                                rayHit.collider.gameObject.transform.lossyScale.y)
                                                    * hitCollider.size;

                //�ڐG�����[�擾
                hitColliderEnd = GetColliderEnd(rayHit.point, hitColliderSize, hitObjPos);

                //�ڕW���W�X�V
                blinkPosition = GetCollisionedBlinkRange(hitColliderEnd, _blinkDirectionVector, rayHit.point, 
                                                            _playerColliderSize, blinkPosition, transform.position);

                

                //�u�����N�I���t���Otrue
                isBlinkEnd = true;

                break;
            }
        }

        //���W�X�V
        transform.position += blinkPosition;

        //�ړ������X�V
        _blinkNowRange = blinkRange;

        //�u�����N�I�����邩
        if (isBlinkEnd)
        {
            //�u�����N�J�n�t���Otrue
            _isBlinkStart = true;

            //�ʏ��Ԃ�
            _playerControler.DefaultMotion();

            //�ڒn���Ă��Ȃ���
            if (!_characterManager.GetIsCollision(CharacterManager.COLLISION.GROUND))
            {
                //�����J�n
                _playerControler.FallStart();
            }
        }

        return _blinkDirectionVector;
    }



    /// <summary>
    /// �u�����N�g�p�\�t���O��Ԃ����\�b�h
    /// </summary>
    /// <returns>�u�����N�g�p�\�t���O</returns>
    public bool GetIsBlink()
    {
        //�u�����N�g�p�\�t���O
        bool isBlink = true;


        //�u�����N����̂�HP������Ȃ���
        if (_characterManager.GetNowHp() - _blinkUseHp <= 0)
        {
            //�u�����N�g�p�\�t���O
            isBlink = false;
        }

        return isBlink;
    }



    /// <summary>
    /// ���͒l�ϊ����\�b�h
    /// </summary>
    /// <param name="stickInput">���͒l</param>
    /// <param name="isPlayerDirectionRight">�E�����t���O</param>
    /// <returns>�ϊ�����͒l</returns>
    private Vector2 InputConversion(Vector2 stickInput, bool isPlayerDirectionRight)
    {
        //X���͂��Ȃ���
        if (stickInput.x == 0)
        {
            //Y���͂��Ȃ���
            if (stickInput.y == 0)
            {
                /*�@���͂Ȃ��@*/

                //�v���C���[���E������
                if (isPlayerDirectionRight)
                {
                    //�E���͈����ɂ���
                    stickInput = Vector2.right;
                }
                else
                {
                    //�����͈����ɂ���
                    stickInput = Vector2.left;
                }
            }
            else
            {
                /*�@Y���͂���@*/

                //Y���͂�1��-1�ɕϊ�����
                stickInput.y = Mathf.Sign(stickInput.y);
            }
        }
        else
        {
            //Y���͂��Ȃ���
            if (stickInput.y == 0)
            {
                /*�@X���͂���@*/

                //X���͂�1��-1�ɕϊ�����
                stickInput.x = Mathf.Sign(stickInput.x);
            }
            else
            {
                /*�@X,Y���͂���@*/

                /*-------------------*/
                //�ꎟ�֐�������
                //y=ax
                //a=�X��
                /*-------------------*/

                //�X���擾�p
                float tilt;


                //�X���擾
                tilt = stickInput.y / stickInput.x;

                //���͒l�̂w���x���傫����
                if (Mathf.Abs(stickInput.x) > Mathf.Abs(stickInput.y))
                {
                    //�x���P�Ɖ��肵�ē��͒l�𒲐�
                    stickInput.y = Mathf.Sign(stickInput.y);
                    stickInput.x = stickInput.y / tilt;
                }
                else
                {
                    //�w���P�Ɖ��肵�ē��͒l�𒲐�
                    stickInput.x = Mathf.Sign(stickInput.x);
                    stickInput.y = tilt * stickInput.x;
                }
            }
        }

        return stickInput;
    }



    /// <summary>
    /// �ڐG�����|�C���g���㉺���E�ǂ̒[�Ɉ�ԋ߂�����Ԃ����\�b�h
    /// </summary>
    /// <param name="hitPosition">�ڐG�����|�C���g���W</param>
    /// <param name="colliderSize">�R���C�_�[�̑傫��</param>
    /// <param name="targetPosition">�ΏۃI�u�W�F�N�g���W</param>
    /// <returns>��ԋ߂��[</returns>
    private COLLIDER_END GetColliderEnd(Vector2 hitPosition, Vector2 colliderSize,
                                            Vector2 targetPosition)
    {
        //�R���C�_�[��[�Ƃ̋����擾�p
        float topRange;

        //�R���C�_�[�E�[�Ƃ̋����擾�p
        float rightRange;

        //�R���C�_�[���[�Ƃ̋����擾�p
        float leftRange;

        //�R���C�_�[���[�Ƃ̋����擾�p
        float lowestRange;

        //��ԋ߂��[�擾�p
        float nearEnd;

        //�ڐG�����R���C�_�[�[�擾�p
        COLLIDER_END hitColliderEnd;


        //���ꂼ��̃R���C�_�[�[�Ƃ̋����擾
        topRange = Mathf.Abs(hitPosition.y - (targetPosition.y + colliderSize.y / 2));
        rightRange = Mathf.Abs(hitPosition.x - (targetPosition.x + colliderSize.x / 2));
        leftRange = Mathf.Abs(hitPosition.x - (targetPosition.x - colliderSize.x / 2));
        lowestRange = Mathf.Abs(hitPosition.y - (targetPosition.y - colliderSize.y / 2));

        //�ŏ��ɂ܂���[����
        hitColliderEnd = COLLIDER_END.TOP;
        nearEnd = topRange;

        //�����ԋ߂��[���E�[�̂ق����߂���
        if (nearEnd > rightRange)
        {
            //�E�[���
            hitColliderEnd = COLLIDER_END.RIGHT;
            nearEnd = rightRange;
        }

        //�����ԋ߂��[��荶�[�̂ق����߂���
        if (nearEnd > leftRange)
        {
            //���[���
            hitColliderEnd = COLLIDER_END.LEFT;
            nearEnd = leftRange;
        }

        //�����ԋ߂��[��艺�[�̂ق����߂���
        if (nearEnd > lowestRange)
        {
            //���[���
            hitColliderEnd = COLLIDER_END.LOWEST;
        }
        
        return hitColliderEnd;
    }



    /// <summary>
    /// �ǂɓ��������ۂɃu�����N�ڕW���W�܂ł̋����x�N�g����Ԃ����\�b�h
    /// </summary>
    /// <param name="targetEnd">�ڐG����ǂ̎l�[</param>
    /// <param name="blinkVector">�u�����N�̈ړ��x�N�g��</param>
    /// <param name="rayHitPoint">���C�̐ڐG���W</param>
    /// <param name="playerColliderSize">�v���C���[�̃R���C�_�[�̑傫��</param>
    /// <param name="blinkPosition">�u�����N��̍��W</param>
    /// <param name="nowPosition">���ݍ��W</param>
    /// <returns>�u�����N�ڕW���W�܂ł̋����x�N�g��</returns>
    private Vector3 GetCollisionedBlinkRange(COLLIDER_END targetEnd, Vector2 blinkVector, Vector2 rayHitPoint,
                                        Vector2 playerColliderSize, Vector2 blinkPosition, Vector2 nowPosition)
    {
        //�X���擾�p
        float tilt;

        //�C�����̍��W�擾�p
        Vector2 fixPosition;

        //�u�����N��̃R���C�_�[�̊p���W�擾�p
        float playerColliderEnd = default;


        //�u�����N��̍��W�擾
        blinkPosition += nowPosition;

        //���͒lX,Y�ǂ��炩��0��
        if (blinkVector.x == 0 || blinkVector.y == 0)
        {
            //�X���Ȃ�
            tilt = 0;
        }
        else
        {
            //�X���擾
            tilt = (blinkPosition.y - nowPosition.y) / (blinkPosition.x - nowPosition.x);
        }

        //�擾�����[�ɂ�蕪��
        switch (targetEnd)
        {
            //���[
            case COLLIDER_END.LEFT:
                //�u�����N��̃R���C�_�[�̊p���W�擾
                playerColliderEnd = rayHitPoint.x - playerColliderSize.x / 2 - _collisionWallVacateSpace;

                break;


            //�E�[
            case COLLIDER_END.RIGHT:
                //�u�����N��̃R���C�_�[�̊p���W�擾
                playerColliderEnd = rayHitPoint.x + playerColliderSize.x / 2 + _collisionWallVacateSpace;

                break;


            //��[
            case COLLIDER_END.TOP:
                //�u�����N��̃R���C�_�[�̊p���W�擾
                playerColliderEnd = rayHitPoint.y + playerColliderSize.y / 2 + _collisionWallVacateSpace;

                break;


            //���[
            case COLLIDER_END.LOWEST:
                //�u�����N��̃R���C�_�[�̊p���W�擾
                playerColliderEnd = rayHitPoint.y - playerColliderSize.y / 2 - _collisionWallVacateSpace;

                break;
        }
        
        //�擾�����[�����E��
        if (targetEnd == COLLIDER_END.RIGHT || targetEnd == COLLIDER_END.LEFT)
        {
            //�C��X���W�擾
            fixPosition.x = playerColliderEnd;

            //�C������Y���W�擾
            fixPosition.y = tilt * fixPosition.x;
        }
        else
        {
            //�C��Y���W�擾
            fixPosition.y = playerColliderEnd;

            //�C����X�̍��W�擾
            fixPosition.x = fixPosition.y / tilt;
        }

        fixPosition -= nowPosition;

        return fixPosition;
    }
}
