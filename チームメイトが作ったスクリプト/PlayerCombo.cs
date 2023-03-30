using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombo : NomaBehaviour
{
    //初期通常時の攻撃力
    [SerializeField, Header("初期攻撃力")]
    private int _defaultAttackPoint;

    //初期通常時のオイルドロップ数
    [SerializeField, Header("初期オイルドロップ数")]
    private int _defaultDropOilAmount;

    //プレイヤーが強化されるコンボ数を格納する配列
    [SerializeField, Header("プレイヤー強化コンボ数格納配列")]
    private int[] _strengthenComboCnts;

    //コンボ数毎のコンボが継続する時間(s)
    [SerializeField, Header("コンボ継続時間(s)")]
    private float[] _durationTimes;

    //コンボ数毎の攻撃力
    [SerializeField, Header("コンボ毎攻撃力")]
    private int[] _attackPoints;

    //コンボ数毎のオイルタンクのドロップ数
    [SerializeField, Header("コンボ毎オイルタンクドロップ数")]
    private int[] _dropOilAmounts;

    //現在のコンボ数格納用
    public int _comboCnt = 0;

    //コンボ中フラグ
    private bool _isCombo = false;

    //コンボからの経過時間格納用
    private float _comboTime = 0;

    //コンボの配列番号格納用
    private int _comboValue = 0;

    //プレイヤーの攻撃スクリプト格納用
    private PlayerAttack _playerAttack;

    //プレイヤーの攻撃スクリプト格納用
    public ComboUI _comboUI;



    private void Start()
    {
        //プレイヤーの攻撃スクリプト格納
        _playerAttack = GetComponent<PlayerAttack>();

        //各配列の要素数をプレイヤーが強化されるコンボ数を格納する配列の要素数に合わせる
        //_durationTimes = new float[_strengthenComboCnts.Length];
        //_attackPoints = new int[_strengthenComboCnts.Length];
        //_dropOilAmounts = new int[_strengthenComboCnts.Length];
    }



    void FixedUpdate()
    {
        //コンボ中か
        if (_isCombo)
        {
            //コンボ経過時間更新
            _comboTime += GetFixedDeltaTime();

            //コンボが時間切れになったか
            if (_comboTime >= _durationTimes[_comboValue])
            {
                //コンボ中フラグfalse
                _isCombo = false;

                //コンボ数初期化
                _comboCnt = 0;
                _comboUI.SendMessage("Combo_UI");

                //コンボの配列番号初期化
                _comboValue = 0;
            }
        }
    }



    /// <summary>
    /// コンボメソッド
    /// </summary>
    public void Combo()
    {
        //コンボ中フラグtrue
        _isCombo = true;

        //コンボ数更新
        _comboCnt++;
        _comboUI.SendMessage("Combo_UI");

        //コンボ経過時間初期化
        _comboTime = 0;

        //コンボ数が強化されるコンボ数に到達したか
        if (_comboCnt < _strengthenComboCnts[_strengthenComboCnts.Length - 1] 
                && _comboCnt >= _strengthenComboCnts[_comboValue])
        {
            //コンボの配列番号更新
            _comboValue++;
        }
    }



    /// <summary>
    /// 攻撃力を返すメソッド
    /// </summary>
    /// <returns>攻撃力</returns>
    public int GetAttackPoint()
    {
        //攻撃力取得用
        int attackPoint;


        //最初の攻撃力上昇コンボ数に到達していないか
        if (_comboCnt < _strengthenComboCnts[0])
        {
            //初期攻撃力代入
            attackPoint = _defaultAttackPoint;
        }
        else
        {
            //コンボ中攻撃力代入
            attackPoint = _attackPoints[_comboValue];
        }

        return attackPoint;
    }



    /// <summary>
    /// オイルのドロップ数を返すメソッド
    /// </summary>
    /// <returns>オイルのドロップ数</returns>
    public int GetDropOilAmount()
    {
        //オイルドロップ数取得用
        int dropOilAmount;


        //最初のオイルドロップ数上昇コンボ数に到達していないか
        if (_comboCnt < _strengthenComboCnts[0])
        {
            //初期オイルドロップ数代入
            dropOilAmount = _defaultDropOilAmount;
        }
        else
        {
            //コンボ中攻撃力代入
            dropOilAmount = _dropOilAmounts[_comboValue];
        }

        return dropOilAmount;
    }
}
