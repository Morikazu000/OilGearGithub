using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInCameraSingle : MonoBehaviour
{

    [SerializeField, Header("�X�|�[���|�C���g�̐e")]
    private GameObject _enemySpawn;
    private Bossmove bossmove;

    #region �I�u�W�F�N�g�̔z��

    [field: SerializeField, Header("�G�̃X�|�[���|�C���g")]
    public List<GameObject> _spawnChildren { get; private set; }

    [field: SerializeField, Header("�J�������̃X�|�[���|�C���g�`�F�b�N")]
    public bool[] _insideCamera { get; private set; }

    [field: SerializeField, Header("�X�|�[���|�C���g���琶�����ꂽ�G")]
    public List<GameObject> _spawnGrandChildren { get; private set; }


    public int _spawnCount { get; private set; }

    #endregion

    #region �J�����̐ݒ�

    [SerializeField, Header("�G�������p�̘g�̍ő�l(_maxPoint�g���傫���ݒ肵�Ă�������)")]
    private Vector2 _deathMaxpoint;
    [SerializeField, Header("�G�������p�̘g�̍ŏ��l(_minPoint�g���傫���ݒ肵�Ă�������)")]
    private Vector2 _deathMinpoint;

    [SerializeField, Header("�g�̍ő�l")]
    private Vector2 _maxPoint;
    [SerializeField, Header("�g�̍ŏ��l")]
    private Vector2 _minPoint;

    //----�f�o�b�N�p�@---- 
    public Vector2 _rightTopPoint { get; private set; }
    public Vector2 _leftUnderPoint { get; private set; }
    public Vector2 _deathRightTopPoint { get; private set; }
    public Vector2 _deathLeftUnderPoint { get; private set; }

    #endregion


    protected void Start()
    {
        //�z�񏉊���q
        _spawnChildren = new List<GameObject>();
        //�q�I�u�W�F�N�g�̐�
        _spawnCount = _enemySpawn.transform.childCount;

        //�z��̒������q�I�u�W�F�N�g�Ɠ����ɂ���
        _insideCamera = new bool[_spawnCount];

        //_enemyspawn�̎q�I�u�W�F�N�g�S�Ă�_spawnchildren�̒��ɓ����
        foreach (Transform child in _enemySpawn.transform)
        {
            _spawnChildren.Add(child.gameObject);
        }
    }
    protected void Update()
    {
        //�E��̒��_
        _rightTopPoint = new Vector2(transform.position.x + _maxPoint.x, transform.position.y + _maxPoint.y);
        //����̒��_
        _leftUnderPoint = new Vector2(transform.position.x + _minPoint.x, transform.position.y + _minPoint.y);
        //�G�������E��̒��_
        _deathRightTopPoint = new Vector2(transform.position.x + _deathMaxpoint.x, transform.position.y + _deathMaxpoint.y);
        //�G����������̒��_
        _deathLeftUnderPoint = new Vector2(transform.position.x + _deathMinpoint.x, transform.position.y + _deathMinpoint.y);


        //�@" �X�|�[���|�C���g��" �ݒ肵���g�̒��ɓ����Ă��邩�ǂ���
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 spawnPointPosition = _spawnChildren[i].transform.position;
            //�G�������p�͈̔͂̒��ɓ����Ă��邩�ǂ���
            if (_deathRightTopPoint.x > spawnPointPosition.x && _deathLeftUnderPoint.x < spawnPointPosition.x
                && _deathRightTopPoint.y > spawnPointPosition.y && _deathLeftUnderPoint.y < spawnPointPosition.y)
            {
                //�J�����̕`��͈͂̒��ɂ��邩�ǂ���
                if (_rightTopPoint.x > spawnPointPosition.x && spawnPointPosition.x > _leftUnderPoint.x
                && _rightTopPoint.y > spawnPointPosition.y && spawnPointPosition.y > _leftUnderPoint.y)
                {
                    //�J�����̕`��͈͂̊O�ɏo����false�ɂ���
                    _insideCamera[i] = false;
                }
                else
                {
                    _insideCamera[i] = true;
                    _spawnChildren[i].SetActive(true);
                }
            }
            else
            {
                //�G�������p�̘g�̊O�ɏo����false�ɂ���
                _insideCamera[i] = false;
            }

            //�@" ��������Ă���G��" �������p�̘g����o�Ă��邩�ǂ���
            for (int j = 0; j < _spawnChildren[i].transform.childCount; j++)
            {
                GameObject instageEnemy = _spawnChildren[i].transform.GetChild(j).gameObject;
                //�q�I�u�W�F�N�g�����G�������g�ɓ����Ă��邩�ǂ���
                if (_deathRightTopPoint.x > instageEnemy.transform.position.x && _deathLeftUnderPoint.x < instageEnemy.transform.position.x
                    && _deathRightTopPoint.y > instageEnemy.transform.position.y && _deathLeftUnderPoint.y < instageEnemy.transform.position.y)
                {
                }
                //�`��͈͊O�ł��A�폜����p�̘g�̓����ɂ��邩�ǂ���
                else
                {
                    _spawnChildren[i].transform.GetChild(j).gameObject.SetActive(false);

                }


            }

        }

    }

}
