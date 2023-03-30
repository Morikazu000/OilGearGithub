using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFall : PlayerMotionManager
{
    //������������������鍂��
    [SerializeField, Header("�ő�������x(m)")]
    private float _fallMaxAcceralationHeight;

    //�W�����v�����Ƃ��ɍō����x����ڒn�܂ł̎���
    [SerializeField, Header("�ڒn����(s)")]
    private float _fallMaxTime;

    //���݂̗������x
    private float _fallNowHeight;

    //���݂̗�������
    private float _fallNowTime = 0;

    //���������t���O
    private bool _isFallVelocity = false;

    //�������X��
    private float _velocityTilt = default;

    //�������ؕ�
    private float _velocityIntercept = default;

    //�����J�n�t���O
    private bool _isFallStart = true;

    //�v���C���[�̃R���C�_�[�i�[�p
    private BoxCollider2D _playerCollider;



    private void Start()
    {
        //�v���C���[�̃R���C�_�[�i�[
        _playerCollider = GetComponent<BoxCollider2D>();
    }



    /// <summary>
    /// �������\�b�h
    /// </summary>
    public float Fall(float jumpMaxHeight)
    {
        //�ڒn���Ă��邩
        if (_characterManager.GetIsCollision(CharacterManager.COLLISION.GROUND))
        {
            //�p�����[�^������
            ResetParamater();

            //�����I��
            _playerControler.DefaultMotion();

            return 0;
        }

        /*-------------------*/
        //�񎟊֐�������
        //y=a(x-p)^2+q
        /*-------------------*/

        //�v�Z�p
        float a;

        //��������
        float fallDownHeight;

        //�n�ʔ���擾�p
        RaycastHit2D[] rayHits;

        //�n�ʔ���t���O
        bool isGround = false;

        //�v���C���[�̍��WVector2�^�ϊ��p
        Vector2 playerPosition;

        //���������擾�p
        float fallDownheight;


        //�����J�n����
        if (_isFallStart)
        {
            //�����������x�擾
            _fallNowHeight = jumpMaxHeight;

            //�����J�n�t���Ofalse
            _isFallStart = false;
        }

        //a�Z�o
        a = -jumpMaxHeight * 2 / (Mathf.Pow(_fallMaxTime, 2) * 2);

        //�������ԍX�V
        _fallNowTime += GetFixedDeltaTime();

        //�����������邩
        if (_isFallVelocity)
        {
            //���������擾
            fallDownHeight = _velocityTilt * _fallNowTime + _velocityIntercept;
        }
        else
        {
            //���������擾
            fallDownHeight = a * Mathf.Pow(_fallNowTime, 2) + jumpMaxHeight;
            
            //���݂̗������x�������������鍂���𒴂�����
            if (fallDownHeight <= jumpMaxHeight - _fallMaxAcceralationHeight)
            {
                /*-------------------*/
                //�ꎟ�֐�������
                //y=ax+b
                /*-------------------*/

                //�O�t���[���̗������x�擾�p
                float fallBeforeHeight;

                //�O�t���[���̗������Ԏ擾�p
                float fallBeforeTime;

                
                //�O�t���[���̗������Ԏ擾
                fallBeforeTime = _fallNowTime - GetFixedDeltaTime();

                //�O�t���[���̗������x�擾
                fallBeforeHeight = a * Mathf.Pow(fallBeforeTime, 2) + jumpMaxHeight;
                
                //�X���Z�o
                _velocityTilt = (fallDownHeight - fallBeforeHeight) 
                                        / (_fallNowTime - fallBeforeTime);
                
                //�ؕЎZ�o
                _velocityIntercept = fallDownHeight - _velocityTilt * _fallNowTime;
                
                //���������t���Otrue
                _isFallVelocity = true;
            }
        }
        
        //�v���C���[�̍��W��Vector2�^�ɕϊ�
        playerPosition = new Vector2(transform.position.x, transform.position.y);

        //��������\��̍��x�܂ł�Ray�擾
        rayHits = Physics2D.RaycastAll(playerPosition, Vector2.down, _fallNowHeight - fallDownHeight
                                        + transform.lossyScale.y * _playerCollider.size.y / 2);

        //�n�ʂ̃^�O��T��
        foreach(RaycastHit2D rayHitObj in rayHits)
        {
            //Ray�ɐڐG�����I�u�W�F�N�g���n�ʃ^�O��
            if (rayHitObj.collider.gameObject.tag == "Ground")
            {
                //�n�ʂ̃R���C�_�[�擾�p
                BoxCollider2D groundCollider;

                //�n�ʂ�Transform�擾�p
                Transform groundTransform;

                //�n�ʂ̃R���C�_�[��Y�̑傫���擾�p
                float groundSizeY;

                //�v���C���[�̃R���C�_�[��Y�̑傫���擾�p
                float playerSizeY;


                //�n�ʔ���t���Otrue
                isGround = true;

                //�n�ʂ̃R���C�_�[�擾
                groundCollider = rayHitObj.collider.gameObject.GetComponent<BoxCollider2D>();

                //�n�ʂ�Transform�擾
                groundTransform = rayHitObj.collider.gameObject.transform;

                //�n�ʂ̃R���C�_�[��Y�̑傫���擾
                groundSizeY = groundTransform.lossyScale.y * groundCollider.size.y;

                //�v���C���[�̃R���C�_�[��Y�̑傫���擾
                playerSizeY = transform.lossyScale.y * _playerCollider.size.y;

                //���������擾
                fallDownHeight = _fallNowHeight - Mathf.Abs(transform.position.y 
                                    - groundTransform.position.y) + groundSizeY / 2 + playerSizeY / 2;

                break;
            }
        }
        
        //���������擾
        fallDownheight = _fallNowHeight - fallDownHeight;
        
        //����
        transform.position += Vector3.down * fallDownheight;

        //���݂̗������x�X�V
        _fallNowHeight = fallDownHeight;

        return -fallDownheight;
    }



    /// <summary>
    /// �p�����[�^���������\�b�h
    /// </summary>
    public void ResetParamater()
    {
        //�����J�n�t���O������
        _isFallStart = true;

        //���݂̗������ԏ�����
        _fallNowTime = 0;

        //���������t���O������
        _isFallVelocity = false;
    }
}
