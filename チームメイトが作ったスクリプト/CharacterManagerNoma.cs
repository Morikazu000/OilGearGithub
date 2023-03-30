using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManagerNoma : NomaBehaviour
{
    //�����蔻���Ray���g�p����ۂ́AStart���\�b�h��CollisionStart()
    //�̗͂��g�p����ۂ́AUpdate�܂���FixedUpdate��InvincibleTimeUpdate()�����s���Ă�������

    [SerializeField, Header("�L�����N�^�[�̍ő�HP")]
    private int _characterMaxHP;

    //���ۂɂ����Ă���L�����N�^�[��HP
    private int _nowCharacterHP;

    [SerializeField, Header("�U��������������̖��G����")]
    private float _invincibleTime = 2.5f;

    //��U���t���O
    protected bool _isDamage = false;

    //�U�������ꂽ�Ƃ��̖��G���̃t���O
    private bool _isInvincible;
    private float _elapsedTime;

    #region �Փ˔���Ɋւ���ϐ�

    //���ꂼ��̕�����ray���΂����ۂ̏Փ˃t���O
    protected bool _isGroundTouch = default;
    protected bool _isRoofTouch = default;
    protected bool _isRightWallTouch = default;
    protected bool _isLeftWallTouch = default;

    //Collider�̃T�C�Y�̔���
    private Vector2 _halfColliderSize = default;
    //�Ƃ���������蔻��̒��S(Collider����œ������悤�ȂƂ��Ɏg���Ă�������)
    private Vector2 _colliderCenter = default;

    //Ray�̒���
    float _Raydistance = 0.2f;
    //Ray�̎n�_�ł���Collider�̒[��������ɂ��炷����
    float _shiftEdgePos = 0.03f;

    //Ray�̎n�_���ˏo�����ɂ��炷����
    float _shiftInjectionPos = 0.02f;
    //���炵����Ray�������������ɓ������Ȃ��Ⴂ���Ȃ�����
    float _shiftTransformPos = 0;
    #endregion


    private void OnEnable()
    {
        InisializedHP();

        Collider collider;
    }

    #region �Փˊ֘A

    /// <summary>
    /// ���\�b�h���g���ۂ͕K�������Start�Ŏ��s���Ă�������
    /// </summary>
    protected void CollisionStart()
    {
        BoxCollider2D boxCollider = this.GetComponent<BoxCollider2D>();

        //���I�u�W�F�N�gCollider�̒��S(���[�J�����W)�Ƒ傫�����Ƃ�
        _colliderCenter = boxCollider.offset;
        _halfColliderSize = this.transform.lossyScale * boxCollider.size / 2;
    }

    /// <summary>
    /// �ڒn�t���O��Ԃ��A�ڒn�ʒu���߂荞�܂Ȃ��悤�ɂ��郁�\�b�h
    /// </summary>
    public GameObject GroundRay()
    {
        //2�{��Ray�̎n�_���v�Z����
        Vector2 rightRayPosition = new Vector2(this.gameObject.transform.position.x + _halfColliderSize.x - _shiftEdgePos, this.gameObject.transform.position.y - _halfColliderSize.y - _shiftInjectionPos);
        Vector2 leftRayPosition = new Vector2(this.gameObject.transform.position.x - _halfColliderSize.x + _shiftEdgePos, this.gameObject.transform.position.y - _halfColliderSize.y - _shiftInjectionPos);

        //Ray�̓��������I�u�W�F�N�g�̏����i�[
        RaycastHit2D rightRayhit = Physics2D.Raycast(rightRayPosition, Vector2.down, _Raydistance);
        RaycastHit2D leftRayhit = Physics2D.Raycast(leftRayPosition, Vector2.down, _Raydistance);

        //�f�o�b�O�p
        Debug.DrawRay(rightRayPosition, Vector2.down * _Raydistance, Color.red);
        Debug.DrawRay(leftRayPosition, Vector2.down * _Raydistance, Color.red);

        //�E��Ray�������������̏���
        if (rightRayhit.collider != null && (rightRayhit.collider.CompareTag("Ground") || rightRayhit.collider.CompareTag("GimikGround")))
        {
            _isGroundTouch = true;
            ShiftTransformPos(rightRayhit.point, rightRayPosition);

            /*-------------------------------*/
            //<��ԒǋL>

            //�n�ʂ̃R���C�_�[��[�̍��W�擾�p
            float groundTopPositionY;

            //�n�ʎ擾�p
            GameObject rayHitGround;

            //�n�ʂ̃R���C�_�[�擾�p
            BoxCollider2D groundCollider;


            //�n�ʎ擾
            rayHitGround = rightRayhit.collider.gameObject;

            //�n�ʂ̃R���C�_�[�擾
            groundCollider = rayHitGround.GetComponent<BoxCollider2D>();

            //�n�ʂ̃R���C�_�[��[�̍��W�擾
            groundTopPositionY = rayHitGround.transform.position.y + rayHitGround.transform.lossyScale.y * groundCollider.size.y / 2;

            /*-------------------------------*/

            //�����̃I�u�W�F�N�g��Ray�̓��������ʒu�ɕύX
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, groundTopPositionY + _halfColliderSize.y, 0);

            return rightRayhit.collider.gameObject;
        }
        //����Ray�������������̏���
        if (leftRayhit.collider != null && (leftRayhit.collider.CompareTag("Ground") || leftRayhit.collider.CompareTag("GimikGround")))
        {
            /*-------------------------------*/
            //<��ԒǋL>

            //�n�ʂ̃R���C�_�[��[�̍��W�擾�p
            float groundTopPositionY;

            //�n�ʎ擾�p
            GameObject rayHitGround;

            //�n�ʂ̃R���C�_�[�擾�p
            BoxCollider2D groundCollider;


            //�n�ʎ擾
            rayHitGround = leftRayhit.collider.gameObject;

            //�n�ʂ̃R���C�_�[�擾
            groundCollider = rayHitGround.GetComponent<BoxCollider2D>();

            //�n�ʂ̃R���C�_�[��[�̍��W�擾
            groundTopPositionY = rayHitGround.transform.position.y + rayHitGround.transform.lossyScale.y * groundCollider.size.y / 2;

            /*-------------------------------*/

            _isGroundTouch = true;
            ShiftTransformPos(leftRayhit.point, leftRayPosition);

            //�����̃I�u�W�F�N�g��Ray�̓��������ʒu�ɕύX
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, groundTopPositionY + _halfColliderSize.y, 0);

            return leftRayhit.collider.gameObject;
        }
        //�������ĂȂ��Ƃ��̏���
        _isGroundTouch = false;

        return null;
    }

    /// <summary>
    /// �V��ڐG�t���O��Ԃ��A�ڐG�ʒu���߂荞�܂Ȃ��悤�ɂ��郁�\�b�h
    /// </summary>
    protected void RoofRay()
    {
        //2�{��Ray�̎n�_���v�Z����
        Vector2 rightRayPosition = new Vector2(this.gameObject.transform.position.x + _halfColliderSize.x - _shiftEdgePos, this.gameObject.transform.position.y + _halfColliderSize.y + _shiftInjectionPos);
        Vector2 leftRayPosition = new Vector2(this.gameObject.transform.position.x - _halfColliderSize.x + _shiftEdgePos, this.gameObject.transform.position.y + _halfColliderSize.y + _shiftInjectionPos);

        //Ray�̓��������I�u�W�F�N�g�̏����i�[
        RaycastHit2D rightRayhit = Physics2D.Raycast(rightRayPosition, Vector2.up, _Raydistance);
        RaycastHit2D leftRayhit = Physics2D.Raycast(leftRayPosition, Vector2.up, _Raydistance);

        //�E��Ray�������������̏���
        if (rightRayhit.collider != null && rightRayhit.collider.CompareTag("Ground"))
        {
            _isRoofTouch = true;
            ShiftTransformPos(rightRayhit.point, rightRayPosition);

            //�����̃I�u�W�F�N�g��Ray�̓��������ʒu�ɕύX
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, rightRayhit.point.y - _halfColliderSize.y - _shiftTransformPos, 0);

            return;
        }
        //����Ray�������������̏���
        if (leftRayhit.collider != null && leftRayhit.collider.CompareTag("Ground"))
        {
            _isRoofTouch = true;
            ShiftTransformPos(leftRayhit.point, leftRayPosition);

            //�����̃I�u�W�F�N�g��Ray�̓��������ʒu�ɕύX
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, leftRayhit.point.y - _halfColliderSize.y - _shiftTransformPos, 0);

            return;
        }

        //�������ĂȂ��Ƃ��̏���
        _isRoofTouch = false;
    }

    /// <summary>
    /// �E�̕ǂւ̐ڐG�t���O��Ԃ��A�ڐG�ʒu���߂荞�܂Ȃ��悤�ɂ��郁�\�b�h
    /// </summary>
    protected void RightWallRay()
    {
        //2�{��Ray�̎n�_���v�Z����
        Vector2 upperRayPosition = new Vector2(this.gameObject.transform.position.x + _halfColliderSize.x + _shiftInjectionPos, this.gameObject.transform.position.y + _halfColliderSize.y - _shiftEdgePos);
        Vector2 underRayPosition = new Vector2(this.gameObject.transform.position.x + _halfColliderSize.x + _shiftInjectionPos, this.gameObject.transform.position.y - _halfColliderSize.y + _shiftEdgePos);

        //Ray�̓��������I�u�W�F�N�g�̏����i�[
        RaycastHit2D upperRayhit = Physics2D.Raycast(upperRayPosition, Vector2.right, _Raydistance);
        RaycastHit2D underRayhit = Physics2D.Raycast(underRayPosition, Vector2.right, _Raydistance);

        //���Ray�������������̏���
        if (upperRayhit.collider != null && upperRayhit.collider.CompareTag("Ground"))
        {
            _isRightWallTouch = true;
            ShiftTransformPos(upperRayhit.point, upperRayPosition);

            //�����̃I�u�W�F�N�g��Ray�̓��������ʒu�ɕύX
            this.gameObject.transform.position = new Vector3(upperRayhit.point.x - _halfColliderSize.x - _shiftTransformPos, this.gameObject.transform.position.y, 0);

            return;
        }
        if (underRayhit.collider != null && underRayhit.collider.CompareTag("Ground"))
        {
            _isRightWallTouch = true;
            ShiftTransformPos(underRayhit.point, underRayPosition);

            //�����̃I�u�W�F�N�g��Ray�̓��������ʒu�ɕύX
            this.gameObject.transform.position = new Vector3(underRayhit.point.x - _halfColliderSize.x - _shiftTransformPos, this.gameObject.transform.position.y, 0);

            return;
        }

        //�������ĂȂ��Ƃ��̏���
        _isRightWallTouch = false;
    }

    /// <summary>
    /// ���̕ǂւ̐ڐG�t���O��Ԃ��A�ڐG�ʒu���߂荞�܂Ȃ��悤�ɂ��郁�\�b�h
    /// </summary>
    protected void LeftWallRay()
    {
        //2�{��Ray�̎n�_���v�Z����
        Vector2 upperRayPosition = new Vector2(this.gameObject.transform.position.x - _halfColliderSize.x - _shiftInjectionPos, this.gameObject.transform.position.y + _halfColliderSize.y - _shiftEdgePos);
        Vector2 underRayPosition = new Vector2(this.gameObject.transform.position.x - _halfColliderSize.x - _shiftInjectionPos, this.gameObject.transform.position.y - _halfColliderSize.y + _shiftEdgePos);

        //Ray�̓��������I�u�W�F�N�g�̏����i�[
        RaycastHit2D upperRayhit = Physics2D.Raycast(upperRayPosition, Vector2.left, _Raydistance);
        RaycastHit2D underRayhit = Physics2D.Raycast(underRayPosition, Vector2.left, _Raydistance);

        //���Ray�������������̏���
        if (upperRayhit.collider != null && upperRayhit.collider.CompareTag("Ground"))
        {
            _isLeftWallTouch = true;
            ShiftTransformPos(upperRayhit.point, upperRayPosition);

            //�����̃I�u�W�F�N�g��Ray�̓��������ʒu�ɕύX
            this.gameObject.transform.position = new Vector3(upperRayhit.point.x + _halfColliderSize.x + _shiftTransformPos, this.gameObject.transform.position.y, 0);

            return;
        }
        //����Ray�������������̏���
        if (underRayhit.collider != null && underRayhit.collider.CompareTag("Ground"))
        {
            _isLeftWallTouch = true;
            ShiftTransformPos(underRayhit.point, underRayPosition);

            //�����̃I�u�W�F�N�g��Ray�̓��������ʒu�ɕύX
            this.gameObject.transform.position = new Vector3(underRayhit.point.x + _halfColliderSize.x + _shiftTransformPos, this.gameObject.transform.position.y, 0);

            return;
        }

        //�������ĂȂ��Ƃ��̏���
        _isLeftWallTouch = false;
    }

    /// <summary>
    /// Ray�̔��˒n�_�����炵�����A�߂��Ȃ���΂Ȃ�Ȃ��I�u�W�F�N�g�̋������i�[���郁�\�b�h
    /// </summary>
    private void ShiftTransformPos(Vector2 rayHitPoint, Vector2 rayStartPos)
    {
        if (rayHitPoint == rayStartPos)
        {
            _shiftTransformPos = _shiftInjectionPos;
        }
        else
        {
            _shiftTransformPos = 0;
        }
    }

    /*---------------��ԒǋL---------------*/

    //�Փ˔���enum
    public enum COLLISION
    {
        GROUND,
        ROOF,
        RIGHT_WALL,
        LEFT_WALL,
    }



    /// <summary>
    /// �l���̎�肽���Փ˔�����擾���郁�\�b�h
    /// </summary>
    /// <param name="collision">�擾�������Փ˔���</param>
    /// <returns>�Փ˃t���O</returns>
    public bool GetIsCollision(COLLISION collision)
    {
        //�Փ˃t���O�擾�p
        bool isCollision = default;


        //�Ƃ肽������ɂ�蕪��
        switch (collision)
        {
            case COLLISION.GROUND:
                //�ڒn����
                GroundRay();

                //�ڒn�t���O�擾
                isCollision = _isGroundTouch;

                break;


            case COLLISION.ROOF:
                //�V�䔻��
                RoofRay();

                //�V��t���O�擾
                isCollision = _isRoofTouch;

                break;


            case COLLISION.RIGHT_WALL:
                //�E�ǔ���
                RightWallRay();

                //�E�ǃt���O�擾
                isCollision = _isRightWallTouch;

                break;


            case COLLISION.LEFT_WALL:
                //���ǔ���
                LeftWallRay();

                //���ǃt���O�擾
                isCollision = _isLeftWallTouch;

                break;
        }

        return isCollision;
    }

    #endregion


    #region HP�֘A
    /// <summary>
    /// HP��������������AHP��0�ȉ��ɂȂ������A�N�e�B�u�ɂ���
    /// </summary>
    public void TakesDamage(int damageValue)
    {
        if(_isInvincible == false)
        {
            //��U���t���Otrue
            _isDamage = true;

            //�_���[�W���v�Z����
            _nowCharacterHP -= damageValue;

            //�̗͂��Ȃ��Ȃ����炱�̃L������|��
            if (_nowCharacterHP <= 0)
            {
                this.gameObject.SetActive(false);
            }

            //���G���Ԃɂ͂���
            _isInvincible = true;
            _elapsedTime = 0;
        }
    }

    /// <summary>
    /// HP�̏�����
    /// </summary>
    private void InisializedHP()
    {
        _nowCharacterHP = _characterMaxHP;
        _isInvincible = false;
    }

    /// <summary>
    /// ���G���Ԓ��̏���(�̗͂������ꍇ�AUpdate��FixedUpdate�Ŏ��s���Ă�������)
    /// </summary>
    protected void InvincibleTimeUpdate()
    {
        if(_isInvincible == true)
        {
            _elapsedTime += Time.fixedDeltaTime;
            //�v�����Ă��鎞�Ԃ����G���Ԃ𒴂����疳�G����������
            if (_elapsedTime >= _invincibleTime)
            {
                _isInvincible = false;
            }
        }
    }



    /// <summary>
    /// ���݂�HP��Ԃ����\�b�h
    /// </summary>
    /// <returns>���݂�HP</returns>
    public int GetNowHp()
    {
        return _nowCharacterHP;
    }



    /// <summary>
    /// HP�������\�b�h
    /// </summary>
    /// <param name="decreaceValue">�����l</param>
    public void DecreaceHp(int decreaceValue)
    {
        //HP�����炷
        _nowCharacterHP -= decreaceValue;
    }



    /// <summary>
    /// �ő�HP��Ԃ����\�b�h
    /// </summary>
    /// <returns>�ő�HP</returns>
    public int GetMaxHp()
    {
        return _characterMaxHP;
    }

    #endregion
}

