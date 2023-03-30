using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOil : PlayerMotionManager
{
    [Header("HP�I�C���Q�[�W�i�X���C�_�[�j")]
    [SerializeField]
    private Slider _hpSlider = default;

    [Header("�I�C�������c�莞�ԃQ�[�W�i�X���C�_�[�j")]
    [SerializeField]
    private Slider _oilSlider = default;

    [SerializeField, Header("�I�C�������Ɏg��HP����(��)"), Range(0, 100)]
    private float _insertOilUseHpRatio;

    [SerializeField, Header("�I�C���Q�[�W�̂P�b���̌�������(��)"), Range(0, 100)]
    private float _oilDecreaceRatio;

    //�I�C�������t���O
    private bool _isOilInsert = false;

    //�I�C�������o�ߎ��Ԋi�[�p
    private float _oilInsertTime = 0;



    private void Start()
    {
        //�ő�A�Œ�HP�擾
        _hpSlider.maxValue = _characterManager.GetMaxHp();
        _hpSlider.minValue = 0;

        //�ő�A�Œ�I�C���Q�[�W�l�擾
        _oilSlider.maxValue = 100 / _oilDecreaceRatio;
        _oilSlider.minValue = 0;

        //HP�Q�[�W������
        _hpSlider.value = _hpSlider.maxValue;

        //�I�C���Q�[�W������
        _oilSlider.value = 0;
    }



    private void FixedUpdate()
    {

        _hpSlider.value = _characterManager.GetNowHp();

        //�I�C����������Ă��邩
        if (_isOilInsert)
        {
            //�I�C�������o�ߎ��ԍX�V
            _oilInsertTime += GetFixedDeltaTime();
            
            //�I�C�������炷��
            if (_oilInsertTime >= 1)
            {
                //�I�C�������炷
                _oilSlider.value--;
                
                //�I�C�������o�ߎ��ԏ�����
                _oilInsertTime = 0;

                //�I�C����0�ɂȂ�����
                if (_oilSlider.value <= 0)
                {
                    //�I�C���Q�[�W������
                    _oilSlider.value = 0;

                    //�I�C�������t���O������
                    _isOilInsert = false;
                }
            }
        }
    }



    /// <summary>
    /// �I�C���g�p���\�b�h
    /// </summary>
    /// <param name="useOilAmount">�I�C���g�p��</param>
    /// <param name="isHit">��U���t���O</param>
    public void UseOil(int useOilAmount, bool isHit)
    {
        //HP�Q�[�W�X�V
        _hpSlider.value -= useOilAmount;

        //�U�����󂯂���
        if (isHit)
        {
            //�_���[�W���󂯂�
            _characterManager.TakesDamage(useOilAmount);
        }
        else
        {
            //�I�C�����g�p
            _characterManager.DecreaceHp(useOilAmount);
        }
    }



    /// <summary>
    /// �I�C���������\�b�h
    /// </summary>
    public void InsertOil()
    {
        //����HP�擾�p
        int nowHp;

        //��������HP�擾�p
        int decreaceHp;


        //����HP�擾
        nowHp = _characterManager.GetNowHp();

        //��������HP�擾
        decreaceHp = (int)((float)_characterManager.GetMaxHp() / 100 * _insertOilUseHpRatio);

        //���̂܂�HP��������HP��0�ȉ��ɂȂ邩
        if (nowHp - decreaceHp <= 0)
        {
            //HP���P�c��悤�Ɍ����l�𒲐�
            decreaceHp = nowHp - 1;
        }

        //HP�����炷
        _characterManager.DecreaceHp(decreaceHp);

        //HP�X���C�_�\�X�V
        _hpSlider.value = nowHp - decreaceHp;

        //�I�C���Q�[�W�𖞃^���ɂ���
        _oilSlider.value = _oilSlider.maxValue;

        //�I�C�������t���Otrue
        _isOilInsert = true;
    }



    /// <summary>
    /// �I�C�������t���O��Ԃ����\�b�h
    /// </summary>
    /// <returns>�I�C�������t���O</returns>
    public bool GetIsInsertOil()
    {
        return _isOilInsert;
    }
}
