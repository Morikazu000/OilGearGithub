using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemypoolSingle : MonoBehaviour
{
    [SerializeField, Header("生成したい敵")]
    private GameObject _enemyvariety;

    [SerializeField, Header("生成する敵の上限")]
    private int _enemycount = 5;

    [SerializeField, Header("ステージにいる敵の上限")]
    private int _inStageEnemy = 2;
    [SerializeField]
    private int _instagecount = 0;
    [SerializeField]
    private int _instantCount = 0;

    /// <summary>
    /// 敵生成
    /// </summary>
    public void InstantiateEnemy()
    {


        //敵の生成数が上限ではない場合
        if (_instagecount < _inStageEnemy && _instantCount < _enemycount)
        {
            //オブジェクトプールで生成する
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                //アクティブがfalseのものがあったら位置を再設定して、trueにする。
                if (gameObject.transform.GetChild(i).gameObject.activeSelf == false)
                {
                    //生成数保持変数とステージにいる敵の変数カウントアップしてsetavtiveをtrueにする
                    _instagecount++;
                    _instantCount++;
                    //生成ポイントにポジション移動
                    gameObject.transform.GetChild(i).gameObject.transform.position = gameObject.transform.position;
                    gameObject.transform.GetChild(i).gameObject.SetActive(true);
                    return;
                }

            }

            //生成数保持変数とステージにいる敵の変数カウントアップして敵生成
            _instagecount++;
            _instantCount++;
            Instantiate(_enemyvariety, transform.position, Quaternion.identity, this.gameObject.transform);
        }
        //子オブジェクトがactive = falseになったらステージにいる敵の変数を減算する
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.activeSelf == false && _instagecount > 0)
            {
                _instagecount--;
            }
        }
    }
}
