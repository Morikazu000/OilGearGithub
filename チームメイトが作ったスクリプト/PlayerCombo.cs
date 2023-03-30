using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombo : NomaBehaviour
{
    //�����ʏ펞�̍U����
    [SerializeField, Header("�����U����")]
    private int _defaultAttackPoint;

    //�����ʏ펞�̃I�C���h���b�v��
    [SerializeField, Header("�����I�C���h���b�v��")]
    private int _defaultDropOilAmount;

    //�v���C���[�����������R���{�����i�[����z��
    [SerializeField, Header("�v���C���[�����R���{���i�[�z��")]
    private int[] _strengthenComboCnts;

    //�R���{�����̃R���{���p�����鎞��(s)
    [SerializeField, Header("�R���{�p������(s)")]
    private float[] _durationTimes;

    //�R���{�����̍U����
    [SerializeField, Header("�R���{���U����")]
    private int[] _attackPoints;

    //�R���{�����̃I�C���^���N�̃h���b�v��
    [SerializeField, Header("�R���{���I�C���^���N�h���b�v��")]
    private int[] _dropOilAmounts;

    //���݂̃R���{���i�[�p
    public int _comboCnt = 0;

    //�R���{���t���O
    private bool _isCombo = false;

    //�R���{����̌o�ߎ��Ԋi�[�p
    private float _comboTime = 0;

    //�R���{�̔z��ԍ��i�[�p
    private int _comboValue = 0;

    //�v���C���[�̍U���X�N���v�g�i�[�p
    private PlayerAttack _playerAttack;

    //�v���C���[�̍U���X�N���v�g�i�[�p
    public ComboUI _comboUI;



    private void Start()
    {
        //�v���C���[�̍U���X�N���v�g�i�[
        _playerAttack = GetComponent<PlayerAttack>();

        //�e�z��̗v�f�����v���C���[�����������R���{�����i�[����z��̗v�f���ɍ��킹��
        //_durationTimes = new float[_strengthenComboCnts.Length];
        //_attackPoints = new int[_strengthenComboCnts.Length];
        //_dropOilAmounts = new int[_strengthenComboCnts.Length];
    }



    void FixedUpdate()
    {
        //�R���{����
        if (_isCombo)
        {
            //�R���{�o�ߎ��ԍX�V
            _comboTime += GetFixedDeltaTime();

            //�R���{�����Ԑ؂�ɂȂ�����
            if (_comboTime >= _durationTimes[_comboValue])
            {
                //�R���{���t���Ofalse
                _isCombo = false;

                //�R���{��������
                _comboCnt = 0;
                _comboUI.SendMessage("Combo_UI");

                //�R���{�̔z��ԍ�������
                _comboValue = 0;
            }
        }
    }



    /// <summary>
    /// �R���{���\�b�h
    /// </summary>
    public void Combo()
    {
        //�R���{���t���Otrue
        _isCombo = true;

        //�R���{���X�V
        _comboCnt++;
        _comboUI.SendMessage("Combo_UI");

        //�R���{�o�ߎ��ԏ�����
        _comboTime = 0;

        //�R���{�������������R���{���ɓ��B������
        if (_comboCnt < _strengthenComboCnts[_strengthenComboCnts.Length - 1] 
                && _comboCnt >= _strengthenComboCnts[_comboValue])
        {
            //�R���{�̔z��ԍ��X�V
            _comboValue++;
        }
    }



    /// <summary>
    /// �U���͂�Ԃ����\�b�h
    /// </summary>
    /// <returns>�U����</returns>
    public int GetAttackPoint()
    {
        //�U���͎擾�p
        int attackPoint;


        //�ŏ��̍U���͏㏸�R���{���ɓ��B���Ă��Ȃ���
        if (_comboCnt < _strengthenComboCnts[0])
        {
            //�����U���͑��
            attackPoint = _defaultAttackPoint;
        }
        else
        {
            //�R���{���U���͑��
            attackPoint = _attackPoints[_comboValue];
        }

        return attackPoint;
    }



    /// <summary>
    /// �I�C���̃h���b�v����Ԃ����\�b�h
    /// </summary>
    /// <returns>�I�C���̃h���b�v��</returns>
    public int GetDropOilAmount()
    {
        //�I�C���h���b�v���擾�p
        int dropOilAmount;


        //�ŏ��̃I�C���h���b�v���㏸�R���{���ɓ��B���Ă��Ȃ���
        if (_comboCnt < _strengthenComboCnts[0])
        {
            //�����I�C���h���b�v�����
            dropOilAmount = _defaultDropOilAmount;
        }
        else
        {
            //�R���{���U���͑��
            dropOilAmount = _dropOilAmounts[_comboValue];
        }

        return dropOilAmount;
    }
}
