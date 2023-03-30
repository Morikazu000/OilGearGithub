using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOil : PlayerMotionManager
{
    [Header("HPオイルゲージ（スライダー）")]
    [SerializeField]
    private Slider _hpSlider = default;

    [Header("オイル差し残り時間ゲージ（スライダー）")]
    [SerializeField]
    private Slider _oilSlider = default;

    [SerializeField, Header("オイル差しに使うHP割合(％)"), Range(0, 100)]
    private float _insertOilUseHpRatio;

    [SerializeField, Header("オイルゲージの１秒毎の減少割合(％)"), Range(0, 100)]
    private float _oilDecreaceRatio;

    //オイル差しフラグ
    private bool _isOilInsert = false;

    //オイル差し経過時間格納用
    private float _oilInsertTime = 0;



    private void Start()
    {
        //最大、最低HP取得
        _hpSlider.maxValue = _characterManager.GetMaxHp();
        _hpSlider.minValue = 0;

        //最大、最低オイルゲージ値取得
        _oilSlider.maxValue = 100 / _oilDecreaceRatio;
        _oilSlider.minValue = 0;

        //HPゲージ初期化
        _hpSlider.value = _hpSlider.maxValue;

        //オイルゲージ初期化
        _oilSlider.value = 0;
    }



    private void FixedUpdate()
    {

        _hpSlider.value = _characterManager.GetNowHp();

        //オイルがさされているか
        if (_isOilInsert)
        {
            //オイル差し経過時間更新
            _oilInsertTime += GetFixedDeltaTime();
            
            //オイルを減らすか
            if (_oilInsertTime >= 1)
            {
                //オイルを減らす
                _oilSlider.value--;
                
                //オイル差し経過時間初期化
                _oilInsertTime = 0;

                //オイルが0になったか
                if (_oilSlider.value <= 0)
                {
                    //オイルゲージ初期化
                    _oilSlider.value = 0;

                    //オイル差しフラグ初期化
                    _isOilInsert = false;
                }
            }
        }
    }



    /// <summary>
    /// オイル使用メソッド
    /// </summary>
    /// <param name="useOilAmount">オイル使用量</param>
    /// <param name="isHit">被攻撃フラグ</param>
    public void UseOil(int useOilAmount, bool isHit)
    {
        //HPゲージ更新
        _hpSlider.value -= useOilAmount;

        //攻撃を受けたか
        if (isHit)
        {
            //ダメージを受ける
            _characterManager.TakesDamage(useOilAmount);
        }
        else
        {
            //オイルを使用
            _characterManager.DecreaceHp(useOilAmount);
        }
    }



    /// <summary>
    /// オイル差しメソッド
    /// </summary>
    public void InsertOil()
    {
        //現在HP取得用
        int nowHp;

        //減少分のHP取得用
        int decreaceHp;


        //現在HP取得
        nowHp = _characterManager.GetNowHp();

        //減少分のHP取得
        decreaceHp = (int)((float)_characterManager.GetMaxHp() / 100 * _insertOilUseHpRatio);

        //このままHPを消費するとHPが0以下になるか
        if (nowHp - decreaceHp <= 0)
        {
            //HPが１残るように減少値を調整
            decreaceHp = nowHp - 1;
        }

        //HPを減らす
        _characterManager.DecreaceHp(decreaceHp);

        //HPスライダ―更新
        _hpSlider.value = nowHp - decreaceHp;

        //オイルゲージを満タンにする
        _oilSlider.value = _oilSlider.maxValue;

        //オイル差しフラグtrue
        _isOilInsert = true;
    }



    /// <summary>
    /// オイル差しフラグを返すメソッド
    /// </summary>
    /// <returns>オイル差しフラグ</returns>
    public bool GetIsInsertOil()
    {
        return _isOilInsert;
    }
}
